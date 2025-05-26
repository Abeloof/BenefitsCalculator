using Api.Data.Entities;
using Api.Data.Entities.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

/// init migrations: dotnet ef migrations add --context EmployeesDbContext initial -n Data.Migrations
public class EmployeesDbContext(DbContextOptions<EmployeesDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Dependent> Dependents { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new DependentsEntityTypeConfiguration());
        builder.ApplyConfiguration(new EmployeesEntityTypeConfiguration());
    }
}

