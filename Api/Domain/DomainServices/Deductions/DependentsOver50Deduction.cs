using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;
using Api.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Api.Domain.DomainServices.Deductions;

public class DependentsOver50Deduction(IOptions<EarningsOptions> options) : IEarningPeriodDeduction
{
    private const int DaysInAYear = 365;
    private const int Age = 50;
    private const string Description = "{0} employee dependents over age {1} years.";

    protected DateTime Over50BirthDateTime => DateTime.Now.Subtract(TimeSpan.FromDays(DaysInAYear * Age)); // leap years ?

    protected decimal DependentOver50CostPerMonth => options.Value.Deductions.DependentOver50CostPerMonth;

    protected decimal DependentOver50CostPerYear => DependentOver50CostPerMonth * 12;

    protected decimal DependentOver50CostPerEarningPeriod => DependentOver50CostPerYear / options.Value.PayPeriodsPerYear;

    public virtual bool IsDeductable(GetEmployeeDto employee)
    {
        return employee.Dependents.Any(d => d.DateOfBirth <= Over50BirthDateTime);
    }

    public virtual EarningPeriodDeductionDto RetrieveDeduction(GetEmployeeDto employee)
    {
        int count = employee.Dependents.Count(d => d.DateOfBirth <= Over50BirthDateTime);
        return new EarningPeriodDeductionDto
        {
            Amount = Math.Round(DependentOver50CostPerEarningPeriod * count, 2, MidpointRounding.AwayFromZero),
            Description = string.Format(Description, count, Age)
        };
    }
}
