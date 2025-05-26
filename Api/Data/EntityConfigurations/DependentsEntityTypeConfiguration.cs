using Api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Data.Entities.EntityConfigurations;

public class DependentsEntityTypeConfiguration : IEntityTypeConfiguration<Dependent>
{
    public void Configure(EntityTypeBuilder<Dependent> b)
    {
        b.Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .HasColumnType("integer");
        b.Property(p => p.FirstName)
            .HasColumnType("text");
        b.Property(p => p.LastName)
            .HasColumnType("text");
        b.Property(p => p.DateOfBirth)
            .HasColumnType("date");
        b.Property(p => p.Relationship)
            .HasConversion(v => v.ToString(), v => (Relationship)Enum.Parse(typeof(Relationship), v));  //bad data ??
        b.HasOne(p => p.Employee).WithOne()
            .HasForeignKey<Employee>(p => p.Id);
        b.Navigation(p => p.Employee);
        b.HasKey(p => p.Id);
        b.ToTable("Dependents");
    }
}
