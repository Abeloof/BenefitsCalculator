using System.Net;
using Api.Domain.Dtos.Earnings;

namespace ApiTests.IntegrationTests;

public class EarningsIntegrationTests : IntegrationTest
{
    [Fact]
    public async Task WhenAskedFor_EmployeeEarningsForAPayperiod_WithOver80kSalaryAndDependentOver50_ShouldReturnCorrectEarnings()
    {
        var response = await HttpClient.GetAsync("/api/v1/employees/3/Earnings/1");
        var dependentCost = Math.Round(((600 * 12m) / 26m), 2, MidpointRounding.AwayFromZero);
        var dependentOver50Cost = Math.Round((200 * 12m) / 26m, 2, MidpointRounding.AwayFromZero);
        var employedCost = Math.Round((1000 * 12m) / 26m, 2, MidpointRounding.AwayFromZero);
        var over80kCost = Math.Round((143211.12m * .02m) / 26m, 2, MidpointRounding.AwayFromZero);
        var totoalDeduction = dependentCost + employedCost + over80kCost + dependentOver50Cost;
        var expected = new GetEarningByEarningPeriodDto()
        {
            EmployeeId = 3,
            EarningsPeriodId = 1,
            GrossEarnings = Math.Round(143211.12m / 26m, 2, MidpointRounding.AwayFromZero),
            Deductions = new List<EarningPeriodDeductionDto>()
            {
                new EarningPeriodDeductionDto(){
                    Description = "1 employee dependents.",
                    Amount = dependentCost
                },
                new EarningPeriodDeductionDto(){
                    Amount = dependentOver50Cost,
                    Description = "1 employee dependents over age 50 years."
                },
                new EarningPeriodDeductionDto(){
                    Amount = employedCost,
                    Description = "Employed."
                },
                new EarningPeriodDeductionDto(){
                    Amount = over80kCost,
                    Description = "Employee salary over 80000."
                }
                
            },
            TotalDeductions = totoalDeduction,
            NetEarnings = Math.Round((143211.12m / 26m) - totoalDeduction, 2, MidpointRounding.AwayFromZero)
        };
        await response.ShouldReturn(HttpStatusCode.OK, expected);
    }

    [Fact]
    public async Task WhenAskedFor_EmployeeEarningsForAPayperiod_WithOver80kSalaryAnd3Dependents_ShouldReturnCorrectEarnings()
    {
        var response = await HttpClient.GetAsync("/api/v1/employees/2/Earnings/1");
        var dependentsCost = Math.Round(((600 * 12m) / 26m) * 3m, 2, MidpointRounding.AwayFromZero);
        var employedCost = Math.Round((1000 * 12m) / 26m, 2, MidpointRounding.AwayFromZero);
        var over80kCost = Math.Round((92365.22m * .02m) / 26m, 2, MidpointRounding.AwayFromZero);
        var expected = new GetEarningByEarningPeriodDto()
        {
            EmployeeId = 2,
            EarningsPeriodId = 1,
            GrossEarnings = Math.Round(92365.22m / 26m, 2, MidpointRounding.AwayFromZero),
            Deductions = new List<EarningPeriodDeductionDto>()
            {
                new EarningPeriodDeductionDto(){
                    Description = "3 employee dependents.",
                    Amount = dependentsCost
                },
                new EarningPeriodDeductionDto(){
                    Amount = employedCost,
                    Description = "Employed."
                },
                new EarningPeriodDeductionDto(){
                    Amount = over80kCost,
                    Description = "Employee salary over 80000."
                 },
            },
            TotalDeductions = dependentsCost + employedCost + over80kCost,
            NetEarnings = Math.Round((92365.22m / 26m) - (dependentsCost + employedCost + over80kCost), 2, MidpointRounding.AwayFromZero)
        };
        await response.ShouldReturn(HttpStatusCode.OK, expected);
    }

    [Fact]
    public async Task WhenAskedFor_EmployeeEarningsForAPayperiod_withNoDependentAndUnder80kSalary_ShouldReturnCorrectEarnings()
    {
        var response = await HttpClient.GetAsync("/api/v1/employees/1/Earnings/1");
        var employedCost = Math.Round((1000 * 12m) / 26m, 2, MidpointRounding.AwayFromZero);
        var expected = new GetEarningByEarningPeriodDto()
        {
            EmployeeId = 1,
            EarningsPeriodId = 1,
            GrossEarnings = Math.Round(75420.99m / 26m, 2, MidpointRounding.AwayFromZero),
            Deductions = new List<EarningPeriodDeductionDto>()
            {
                new EarningPeriodDeductionDto(){
                    Amount = employedCost,
                    Description = "Employed."
                }
            },
            TotalDeductions =  employedCost,
            NetEarnings = Math.Round((75420.99m / 26m) - employedCost, 2, MidpointRounding.AwayFromZero)
        };
        await response.ShouldReturn(HttpStatusCode.OK, expected);
    }
}
