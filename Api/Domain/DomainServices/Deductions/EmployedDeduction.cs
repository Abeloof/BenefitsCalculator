using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;
using Api.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Api.Domain.DomainServices.Deductions;

public class EmployedDeduction(IOptions<EarningsOptions> options) : IEarningPeriodDeduction
{
    protected virtual decimal EmployeeCostPerMonth => options.Value.Deductions.EmployeeCostPerMonth;

    protected virtual decimal EmployeeCostPerYear => EmployeeCostPerMonth * 12;

    protected virtual decimal EmployeeCostPerEarningPeriod => EmployeeCostPerYear / options.Value.PayPeriodsPerYear;

    public const string Description = "Employed.";
    
    public virtual EarningPeriodDeductionDto RetrieveDeduction(GetEmployeeDto employee)
    {
        return new EarningPeriodDeductionDto
        {
            Amount = Math.Round(EmployeeCostPerEarningPeriod, 2, MidpointRounding.AwayFromZero),
            Description = Description
        };
    }

    public virtual bool IsDeductable(GetEmployeeDto employee) => employee.Salary > EmployeeCostPerEarningPeriod;
}
