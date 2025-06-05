using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;
using Api.Domain.Interfaces;
using DurableTask.Core;
using Microsoft.Extensions.Options;

namespace Api.Domain.DomainServices.Deductions;

public class EmployedDeduction(IOptions<EarningsOptions> options) 
        : TaskActivity<GetEmployeeDto, EarningPeriodDeductionDto?>,IEarningPeriodDeduction
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

    protected override EarningPeriodDeductionDto? Execute(TaskContext context, GetEmployeeDto input)
    {
        if (!this.IsDeductable(input))
            return new EarningPeriodDeductionDto
            {
                Amount = 0m,
                Description = "SKIP"
            };
        return this.RetrieveDeduction(input);
    }
}
