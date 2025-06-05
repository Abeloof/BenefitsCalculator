using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;
using Api.Domain.Interfaces;
using Microsoft.Extensions.Options;
using DurableTask.Core;
using Api.Domain.DomainServices.Deductions;

namespace Api.Domain.DomainServices;

public class EarningsCalulator(IEnumerable<IEarningPeriodDeduction> earningPeriodDeduction, IOptions<EarningsOptions> options)
            : TaskOrchestration<GetEarningByEarningPeriodDto, (int earningPeriodId, GetEmployeeDto employee)>,IEarningsCalculator
{
    private static IList<string> ACTIVITES = new List<string>
    {
        nameof(DependentsDeduction),
        nameof(DependentsOver50Deduction),
        nameof(EmployedDeduction),
        nameof(SalaryBasedDeduction)
    };

    public GetEarningByEarningPeriodDto CalculateEarnings(int earningPeriodId, GetEmployeeDto employee)
    {
        var earning = new GetEarningByEarningPeriodDto()
        {
            EarningsPeriodId = earningPeriodId,
            EmployeeId = employee.Id
        };
        foreach (var earningsDeduction in earningPeriodDeduction)
        {
            if (earningsDeduction.IsDeductable(employee))
            {
                var result = earningsDeduction.RetrieveDeduction(employee, earningPeriodId);
                earning.Deductions.Add(result);
                earning.TotalDeductions += result.Amount;
            }
        }
        earning.GrossEarnings = Math.Round(employee.Salary / options.Value.PayPeriodsPerYear, 2, MidpointRounding.AwayFromZero);
        earning.NetEarnings = earning.GrossEarnings - earning.TotalDeductions;
        return earning;
    }

    public override async Task<GetEarningByEarningPeriodDto> RunTask(OrchestrationContext context,
                                                                        (int earningPeriodId, GetEmployeeDto employee) input)
    {
        var earning = new GetEarningByEarningPeriodDto()
        {
            EarningsPeriodId = input.earningPeriodId,
            EmployeeId = input.employee.Id
        };
        foreach (var activity in ACTIVITES)
        {
            var earningDeduction = await context.ScheduleTask<EarningPeriodDeductionDto?>(activity, string.Empty, input.employee);
            if (earningDeduction == null) continue; // err ?
            if (earningDeduction.Description == "SKIP") continue; //SKIPED
            earning.Deductions.Add(earningDeduction);
            earning.TotalDeductions += earningDeduction.Amount;
        }
        earning.GrossEarnings = Math.Round(input.employee.Salary / options.Value.PayPeriodsPerYear, 2, MidpointRounding.AwayFromZero);
        earning.NetEarnings = earning.GrossEarnings - earning.TotalDeductions;
        return earning;
    }
}