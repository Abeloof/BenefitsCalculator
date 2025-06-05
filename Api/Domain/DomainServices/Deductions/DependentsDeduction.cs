using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;
using Api.Domain.Interfaces;
using DurableTask.Core;
using Microsoft.Extensions.Options;

namespace Api.Domain.DomainServices.Deductions;

public class DependentsDeduction(IOptions<EarningsOptions> options) 
                : TaskActivity<GetEmployeeDto, EarningPeriodDeductionDto?>, IEarningPeriodDeduction
{
    protected virtual decimal DependentCostPerMonth => options.Value.Deductions.DependentCostPerMonth;

    protected virtual decimal DependentCostPerYear => DependentCostPerMonth * 12;

    protected virtual decimal DependentCostPerEarningPeriod => DependentCostPerYear / options.Value.PayPeriodsPerYear;

    private const string Description = "{0} employee dependents.";

    public virtual EarningPeriodDeductionDto RetrieveDeduction(GetEmployeeDto employee)
    {        
        return new EarningPeriodDeductionDto
        {
            Amount = Math.Round(DependentCostPerEarningPeriod * employee.Dependents.Count, 2, MidpointRounding.AwayFromZero),
            Description = string.Format(Description, employee.Dependents.Count)
        };
    }

    public virtual bool IsDeductable(GetEmployeeDto employee) => employee.Dependents.Any();

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
