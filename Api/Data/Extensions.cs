using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public static class Extensions
{
    public static void AddNpgsqlDbContext<TContext>(this IHostApplicationBuilder builder) where TContext : DbContext
    {
        var connectionString = builder.Configuration.GetConnectionString("EmployeesDB");
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
    where TContext : EmployeesDbContext
    where TDbSeeder : EmployeesContextSeed
    {
        services.AddScoped<TDbSeeder>();
        services.AddHostedService<EmployeesContextMigrationHostedService<TContext, TDbSeeder>>();
        return services;
    }
}


public class EmployeesContextMigrationHostedService<TContext, TDbSeeder>(IServiceProvider serviceProvider)
    : BackgroundService where TContext : EmployeesDbContext
                        where TDbSeeder : EmployeesContextSeed
{
    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var scopeServices = scope.ServiceProvider; //?
        var context = scopeServices.GetService<TContext>();
        var seeder = scopeServices.GetService<TDbSeeder>();
        try
        {
            var strategy = context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await context.Database.MigrateAsync();
                await seeder.SeedAsync(context);
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