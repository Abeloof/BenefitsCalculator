using Api.Domain.Dtos.Dependent;
using Api.Domain.Interfaces;
using Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/Employees/{employeeId}/[controller]")]
public class DependentsController(IEmployeesService employeesService) : ControllerBase
{
    [SwaggerOperation(Summary = "Get employee dependent by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetDependentDto>>> Get(int employeeId, int id, CancellationToken cancellationToken)
    {
        var employee = await employeesService.GetEmployeeAsync(employeeId, cancellationToken);
        if (employee is null)
            return NotFound("Employee not found");
        var dependent = employee.Dependents.SingleOrDefault(e => e.Id == id);
        if (dependent is null)
            return NotFound($"Employee {employeeId}'s dependent {id} not found");
        return Ok(new ApiResponse<GetDependentDto>
        {
            Data = dependent,
            Success = true
        });
    }

    [SwaggerOperation(Summary = "Get all employee dependents")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetDependentDto>>>> GetAll(int employeeId,
                    CancellationToken cancellationToken)
    {
        var employee = await employeesService.GetEmployeeAsync(employeeId, cancellationToken);
        if (employee is null)
            return NotFound("Employee not found");
        return new ApiResponse<List<GetDependentDto>>
        {
            Data = [.. employee.Dependents],
            Success = true
        };
    }
}
