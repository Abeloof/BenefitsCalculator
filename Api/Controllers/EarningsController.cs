using Api.Domain.DomainServices;
using Api.Domain.Dtos.Earnings;
using Api.Domain.Interfaces;
using Api.Domain.Models;
using Asp.Versioning;
using DurableTask.Core;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/")]
    public class EarningsController(IEmployeesService employeesService, IEarningsCalculator earningsCalculator, TaskHubClient orchestrationClient) : ControllerBase
    {
        [SwaggerOperation(Summary = "Get calculated employee earnings by earnings pay period")]
        [HttpGet("v1/Employees/{employeeId}/[controller]/{earningPeriodId}")]
        public async Task<ActionResult<ApiResponse<GetEarningByEarningPeriodDto>>> GetEarningsByEarningPeriodV1(int employeeId,
                         int earningPeriodId, CancellationToken cancellationToken = default)
        {
            var employee = await employeesService.GetEmployeeAsync(employeeId, cancellationToken);
            if (employee is null)
                return NotFound("Employee not found");
            return Ok(new ApiResponse<GetEarningByEarningPeriodDto>()
            {
                Data = earningsCalculator.CalculateEarnings(earningPeriodId, employee),
                Success = true
            });
        }

        [SwaggerOperation(Summary = "Get calculated employee earnings by earnings pay period")]
        [HttpGet("{earningPeriodId}")]
        [HttpGet("v2/Employees/{employeeId}/[controller]/{earningPeriodId}")]
        public async Task<ActionResult<ApiResponse<GetEarningByEarningPeriodDto>>> GetEarningsByEarningPeriodV2(int employeeId,
                         int earningPeriodId, CancellationToken cancellationToken = default)
        {
            var employee = await employeesService.GetEmployeeAsync(employeeId, cancellationToken);
            if (employee is null)
                return NotFound("Employee not found");
            var result = await orchestrationClient.GetOrCreateOrchestration(employee, earningPeriodId, cancellationToken);
            return Ok(new ApiResponse<GetEarningByEarningPeriodDto>()
            {
                Data = result.Output.AsT<GetEarningByEarningPeriodDto>(),
                Success = true
            });
        }
    }
}