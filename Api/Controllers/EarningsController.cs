using Api.Domain.Dtos.Earnings;
using Api.Domain.Interfaces;
using Api.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/v1/Employees/{employeeId}/[controller]")]
    public class EarningsController(IEmployeesService employeesService, IEarningsCalculator earningsCalculator) : ControllerBase
    {
        [SwaggerOperation(Summary = "Get calculated employee earnings by earnings pay period")]
        [HttpGet("{earningPeriodId}")]
        public async Task<ActionResult<ApiResponse<GetEarningByEarningPeriodDto>>> GetEarningsByEarningPeriod(int employeeId,
                         int earningPeriodId, CancellationToken cancellationToken = default)
        {
            var employee = await employeesService.GetEmployeeAsync(employeeId, cancellationToken);
            if(employee is null)
                return NotFound("Employee not found");
            return Ok(new ApiResponse<GetEarningByEarningPeriodDto>()
            {
                Data = earningsCalculator.CalculateEarnings(earningPeriodId, employee),
                Success = true
            });
        }
    }
}