using Api.Domain.Dtos.Employee;

namespace Api.Domain.Interfaces;

public interface IEmployeesService
{
    /// <summary>
    /// Returns all employees.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>List of mployees</returns>
    Task<IList<GetEmployeeDto>> GetEmployeesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns an employee whois employee id matchs request's id.
    /// </summary>
    /// <param name="id">Requests employee id</param>
    /// <param name="cancellationToken"></param>
    /// <returns>an employee</returns>
    Task<GetEmployeeDto?> GetEmployeeAsync(int id, CancellationToken cancellationToken = default);
}
