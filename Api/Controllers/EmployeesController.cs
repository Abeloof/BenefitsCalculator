using Api.Domain.Dtos.Employee;
using Api.Domain.Interfaces;
using Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController(IEmployeesService employeesService) : ControllerBase
{
    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id, CancellationToken cancellationToken = default)
    {
        var result = await employeesService.GetEmployeeAsync(id, cancellationToken);
        if (result == null)
            return NotFound("Employee not found");
        return new ApiResponse<GetEmployeeDto>
            {
                Data = result,
                Success = true
            };
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<IList<GetEmployeeDto>>>> GetAll(CancellationToken cancellationToken = default)
    {
        var result = await employeesService.GetEmployeesAsync(cancellationToken);
        return new ApiResponse<IList<GetEmployeeDto>>
        {
            Data = result,
            Success = true
        };
    }
}
