using Api.Data;
using Api.Data.Entities;
using Api.Data.Repository;
using Api.Domain;
using Api.Domain.DomainServices;
using Api.Domain.DomainServices.Deductions;
using Api.Domain.Interfaces;
using DurableTask.AzureStorage;
using DurableTask.Core;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Employee Benefit Cost Calculation Api",
        Description = "Api to support employee benefit cost calculations"
    });
});

var allowLocalhost = "allow localhost";
builder.Services.AddCors(options =>
{
    options.AddPolicy(allowLocalhost,
        policy => { policy.WithOrigins("http://localhost:3000", "http://localhost"); });
});


builder.AddNpgsqlDbContext<EmployeesDbContext>(EmployeesDbContext.ConnectionStringName);
builder.Services.AddMigration<EmployeesDbContext, EmployeesContextSeed>();
builder.Services.AddScoped<IRepository<Employee>, EmployeesRepository>();
builder.Services.AddTransient<IEmployeesService, EmployeesService>();
builder.Services.Configure<EarningsOptions>(
    builder.Configuration.GetSection(EarningsOptions.Earnings)
);
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddTransient<IEarningsCalculator, EarningsCalulator>();
builder.Services.AddTransient<IEarningPeriodDeduction, DependentsDeduction>();
builder.Services.AddTransient<IEarningPeriodDeduction, DependentsOver50Deduction>();
builder.Services.AddTransient<IEarningPeriodDeduction, EmployedDeduction>();
builder.Services.AddTransient<IEarningPeriodDeduction, SalaryBasedDeduction>();

builder.Services.AddTransient<EarningsCalulator>();
builder.Services.AddTransient<DependentsDeduction>();
builder.Services.AddTransient<DependentsOver50Deduction>();
builder.Services.AddTransient<EmployedDeduction>();
builder.Services.AddTransient<SalaryBasedDeduction>();


var settings = new AzureStorageOrchestrationServiceSettings
{
    StorageAccountClientProvider = new StorageAccountClientProvider(builder.Configuration.GetSection("DurableTask:ConnectionString").Value!),
    TaskHubName = builder.Configuration.GetSection("DurableTask:taskHubName").Value!
};

builder.Services.AddSingleton((_) => new AzureStorageOrchestrationService(settings));
builder.Services.AddTransient((sp) => new TaskHubClient(sp.GetService<AzureStorageOrchestrationService>()!));
builder.Services.AddHostedService<SimpleTaskHubWorker>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseCors(allowLocalhost);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


public class SimpleTaskHubWorker : BackgroundService
{
    private readonly AzureStorageOrchestrationService azureStorageOrchestrationService;
    private readonly TaskHubWorker taskHubWorker;

    public SimpleTaskHubWorker(IServiceProvider sp)
    {
        azureStorageOrchestrationService
                    = sp.GetService<AzureStorageOrchestrationService>()!;
        taskHubWorker = new TaskHubWorker(azureStorageOrchestrationService);
        var earningCalc = new SimpleObjectCreator<TaskOrchestration>(nameof(EarningsCalulator), sp.GetService<EarningsCalulator>()!);
        taskHubWorker.AddTaskOrchestrations(earningCalc);
        taskHubWorker.AddTaskActivities([
            new SimpleObjectCreator<TaskActivity>(nameof(DependentsDeduction), sp.GetService<DependentsDeduction>()!),
            new SimpleObjectCreator<TaskActivity>(nameof(DependentsOver50Deduction), sp.GetService<DependentsOver50Deduction>()!),
            new SimpleObjectCreator<TaskActivity>(nameof(EmployedDeduction), sp.GetService<EmployedDeduction>()!),
            new SimpleObjectCreator<TaskActivity>(nameof(SalaryBasedDeduction), sp.GetService<SalaryBasedDeduction>()!),
        ]);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await taskHubWorker.StartAsync();
    }

    public override void Dispose()
    {
        base.Dispose();
        taskHubWorker.StopAsync();
    } 
}