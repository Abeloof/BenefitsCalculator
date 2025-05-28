namespace Api.Domain;

public class EarningsOptions
{
    public const string Earnings = "Earnings";

    public int PayPeriodsPerYear { get; init; }

    public DeductionsOptions Deductions { get; init; } = null!;

}

public record SalaryDeductionOptions(decimal Over, int DeductionPercentPerYear);
public record DeductionsOptions(decimal EmployeeCostPerMonth, decimal DependentCostPerMonth,
                        decimal DependentOver50CostPerMonth, SalaryDeductionOptions SalaryDeduction);