using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;
using Api.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Api.Domain.DomainServices;

public class EarningsCalulator(IEnumerable<IEarningPeriodDeduction> earningPeriodDeduction, IOptions<EarningsOptions> options) : IEarningsCalculator
{
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
}