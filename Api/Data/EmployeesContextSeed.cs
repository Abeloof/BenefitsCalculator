using Api.Data.Entities;
using Api.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public interface IDbSeeder<in TContext> where TContext : DbContext
{
    Task SeedAsync(TContext context);
}

public class EmployeesContextSeed : IDbSeeder<EmployeesDbContext>
{
    public async Task SeedAsync(EmployeesDbContext context)
    {
        context.Employees.AddRange(new List<Employee>
            {
                new Employee() {
                    Id=1,
                    FirstName = "LeBron",
                    LastName = "James",
                    Salary = 75420.99m,
                    DateOfBirth = new DateTime(1984, 12, 30)
                },
                new Employee() {
                    Id=2,
                    FirstName = "Ja",
                    LastName = "Morant",
                    Salary = 92365.22m,
                    DateOfBirth = new DateTime(1999, 8, 10),
                    Dependents = new List<Dependent>(){
                        new Dependent() {
                            Id=1,
                            FirstName = "Spouse",
                            LastName = "Morant",
                            Relationship = Relationship.Spouse,
                            DateOfBirth = new DateTime(1998, 3, 3)
                        },
                        new Dependent() {
                            Id=2,
                            FirstName = "Child1",
                            LastName = "Morant",
                            Relationship = Relationship.Child,
                            DateOfBirth = new DateTime(2020, 6, 23)
                        },
                        new Dependent() {
                            Id=3,
                            FirstName = "Child2",
                            LastName = "Morant",
                            Relationship = Relationship.Child,
                            DateOfBirth = new DateTime(2021, 5, 18)
                        }
                    }
                },
                new Employee() {
                    Id=3,
                    FirstName = "Michael",
                    LastName = "Jordan",
                    Salary = 143211.12m,
                    DateOfBirth = new DateTime(1963, 2, 17),
                    Dependents = new List<Dependent>(){
                        new Dependent() {
                            Id=4,
                            FirstName = "DP",
                            LastName = "Jordan",
                            Relationship = Relationship.DomesticPartner,
                            DateOfBirth = new DateTime(1974, 1, 2)
                        }
                    }
                }
            });
        await context.SaveChangesAsync();
    }
}
