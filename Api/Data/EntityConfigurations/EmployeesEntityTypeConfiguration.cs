using Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.EntityConfigurations;

class EmployeesEntityTypeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> b)
    {
        b.Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("integer");
        b.Property(p => p.FirstName)
            .HasColumnType("text");
        b.Property(p => p.LastName)
            .HasColumnType("text");
        b.Property(p => p.Salary)
            .HasColumnType("numeric(10,2)");
        b.Property(p => p.DateOfBirth)
            .HasColumnType("date");
        b.HasMany(p => p.Dependents)
            .WithOne(e => e.Employee)
            .HasForeignKey(e => e.EmployeeId);
        b.Navigation(p => p.Dependents);
        b.HasKey("Id");
        b.ToTable("Employees");
    }
}