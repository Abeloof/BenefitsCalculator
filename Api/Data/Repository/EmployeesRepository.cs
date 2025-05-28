using Api.Data.Entities;
using Api.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Repository;

public class EmployeesRepository(EmployeesDbContext employeesDbContext) : IRepository<Employee>
{
    public async Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await employeesDbContext.Employees.Include(x => x.Dependents)
                                                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return employee;
    }

    public async Task<IList<Employee>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var employees = await employeesDbContext.Employees.Include(x => x.Dependents)
                                                    .ToArrayAsync(cancellationToken);
        return employees;
    }
}