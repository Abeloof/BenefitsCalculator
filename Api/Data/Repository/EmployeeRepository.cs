using Api.Data.Entities;
using Api.Domain.DomainException;
using Api.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Repository;

public class EmployeeRepository(EmployeesDbContext employeesDbContext) : IEmployeeRepository
{
    public async Task<Employee> GetEmployeeById(int Id)
    {
        var employee = await employeesDbContext.Employees.Include(x => x.Dependents).FirstOrDefaultAsync(x => x.Id == Id)
                                ?? throw new EntityNotFoundException($"No record found. Id = {Id}");
        return employee;
    }

    public async Task<IList<Employee>> GetEmployees()
    {
        var employees = await employeesDbContext.Employees.Include(x => x.Dependents).ToArrayAsync()
                                    ?? throw new EntityNotFoundException($"No record found.");
        return employees;
    }
}
