using Api.Data.Entities;
using Api.Domain.Dtos.Dependent;
using Api.Domain.Dtos.Employee;
using Api.Domain.Interfaces;

namespace Api.Domain.DomainServices;

public class EmployeesService(IRepository<Employee> employeesRepository) : IEmployeesService
{
    public async Task<GetEmployeeDto?> GetEmployeeAsync(int id, CancellationToken cancellationToken = default)
    {
        var employeeEntity = await employeesRepository.GetByIdAsync(id, cancellationToken);
        return MapEmployeeEntityToGetEmpoyeDto(employeeEntity);
    }

    public async Task<IList<GetEmployeeDto>> GetEmployeesAsync(CancellationToken cancellationToken = default)
    {
        var employeesEntities = await employeesRepository.GetAllAsync(cancellationToken);
        if (!employeesEntities.Any()) return new List<GetEmployeeDto>();
        return [.. employeesEntities.Select(MapEmployeeEntityToGetEmpoyeDto)!];
    }

    private GetEmployeeDto? MapEmployeeEntityToGetEmpoyeDto(Employee? employeeEntity)
    {
        if(employeeEntity == null) return null; 
        return new GetEmployeeDto()
        {
            Id = employeeEntity.Id,
            DateOfBirth = employeeEntity.DateOfBirth,
            FirstName = employeeEntity.FirstName,
            LastName = employeeEntity.LastName,
            Salary = employeeEntity.Salary,
            Dependents = [.. employeeEntity.Dependents.Select(n => new GetDependentDto()
            {
                Id = n.Id,
                DateOfBirth = n.DateOfBirth,
                FirstName = n.FirstName,
                LastName = n.LastName,
                Relationship = Enum.GetName(n.Relationship)!
            })]
        };
    }
}
