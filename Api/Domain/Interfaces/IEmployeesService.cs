using Api.Domain.Dtos.Employee;

namespace Api.Domain.Interfaces;

public interface IEmployeesService
{
    Task<IList<GetEmployeeDto>> GetEmployeesAsync(CancellationToken cancellationToken = default);
    Task<GetEmployeeDto?> GetEmployeeAsync(int id, CancellationToken cancellationToken = default);
}
