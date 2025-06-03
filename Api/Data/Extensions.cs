using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public static class Extensions
{
    public static void AddNpgsqlDbContext<TContext>(this IHostApplicationBuilder builder, string connectionStringName) where TContext : DbContext
    {
        var connectionString = builder.Configuration.GetConnectionString(connectionStringName);
        builder.Services.AddDbContextPool<TContext>(builder =>
        {
            builder.UseNpgsql(connectionString, npSql =>
            {
                npSql.EnableRetryOnFailure();
                npSql.CommandTimeout(30);
            });
        });
    }

    public static IServiceCollection AddMigration<TContext, TDbSeeder>(this IServiceCollection services)
    where TContext : DbContext
    where TDbSeeder : class, IDbSeeder<TContext>
    {
        services.AddScoped<TDbSeeder>();
        services.AddHostedService<EmployeesContextMigrationHostedService<TContext, TDbSeeder>>();
        return services;
    }
}


public class EmployeesContextMigrationHostedService<TContext, TDbSeeder>(IServiceProvider serviceProvider, IHostEnvironment env)
    : BackgroundService where TContext : DbContext
                        where TDbSeeder : class, IDbSeeder<TContext>
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var scopeServices = scope.ServiceProvider; 
        var context = scopeServices.GetService<TContext>()
                        ?? throw new ArgumentNullException($"(nameof(EmployeesDbContext)) not found");
        var seeder = scopeServices.GetService<TDbSeeder>()
                        ?? throw new ArgumentNullException($"(nameof(EmployeesContextSeed)) not found");
        try
        {
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                if(env.IsProduction())
                    await context.Database.MigrateAsync();
                if (!env.IsProduction())
                {
                    await context.Database.EnsureDeletedAsync();
                    await context.Database.MigrateAsync();
                    await seeder.SeedAsync(context);
                }
            });
        }
        catch (Exception)
        {
            //pass with log ??
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}