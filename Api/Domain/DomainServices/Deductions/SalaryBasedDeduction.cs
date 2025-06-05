using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;
using Api.Domain.Interfaces;
using DurableTask.Core;
using Microsoft.Extensions.Options;

namespace Api.Domain.DomainServices.Deductions;

public class SalaryBasedDeduction(IOptions<EarningsOptions> options) 
                : TaskActivity<GetEmployeeDto, EarningPeriodDeductionDto?>, IEarningPeriodDeduction
{
    private const string Description = "Employee salary over {0}.";
    protected virtual decimal OverSalary => options.Value.Deductions.SalaryDeduction.Over; //TODO name sounds awful

    protected virtual decimal DeductionPerYear => options.Value.Deductions.SalaryDeduction.DeductionPercentPerYear / 100m;

    public virtual bool IsDeductable(GetEmployeeDto employee)
    {
        return employee.Salary >= OverSalary;
    }

    public virtual EarningPeriodDeductionDto RetrieveDeduction(GetEmployeeDto employee)
    {
        var deductionPerEarningPeriod = employee.Salary * DeductionPerYear / options.Value.PayPeriodsPerYear;
        return new EarningPeriodDeductionDto
        {
            Amount = Math.Round(deductionPerEarningPeriod, 2, MidpointRounding.AwayFromZero),
            Description = string.Format(Description, OverSalary)
        };
    }

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
