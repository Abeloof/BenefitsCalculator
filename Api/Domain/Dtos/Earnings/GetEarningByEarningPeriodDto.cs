namespace Api.Domain.Dtos.Earnings;

public class GetEarningByEarningPeriodDto
{
    public int EmployeeId { get; set; }
    public int EarningsPeriodId { get; set; }
    public decimal GrossEarnings { get; set; }
    public decimal TotalDeductions { get; set; }
    public IList<EarningPeriodDeductionDto> Deductions { get; set; } = new List<EarningPeriodDeductionDto>();
    public decimal NetEarnings { get; set; }
}
