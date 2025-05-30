using Api.Data;
using Api.Data.Entities;
using Api.Data.Repository;
using Api.Domain;
using Api.Domain.DomainServices;
using Api.Domain.DomainServices.Deductions;
using Api.Domain.Interfaces;
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