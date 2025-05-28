using Api.Domain;
using Api.Domain.DomainServices;
using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;
using Api.Domain.Interfaces;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ApiTests.UnitTests;

public class EarningsCalculatorTests
{
    private readonly IOptions<EarningsOptions> _options;
    private readonly GetEmployeeDto _employee;
    private readonly IEnumerable<IEarningPeriodDeduction> _earningPeriodDeductions;
    private readonly EarningPeriodDeductionDto  _deductions;
    public EarningsCalculatorTests()
    {
        _options = Substitute.For<IOptions<EarningsOptions>>();
        _options.Value.Returns(new EarningsOptions()
        {
            PayPeriodsPerYear = 4
        });
        _employee = new GetEmployeeDto()
        {
            Id = 123,
            Salary = 12000
        };
        _deductions = new EarningPeriodDeductionDto()
        {
            Description = "Description",
            Amount = 5000m
        };
        var deductable = Substitute.For<IEarningPeriodDeduction>();
        deductable.IsDeductable(Arg.Any<GetEmployeeDto>()).Returns(true);
        deductable.RetrieveDeduction(Arg.Any<GetEmployeeDto>(), Arg.Any<int>()).Returns(_deductions);
        _earningPeriodDeductions = new List<IEarningPeriodDeduction>()
        {
            deductable
        };
    }

    [Fact]
    public void CalculateEarnings_ReturnCorrectResults()
    {
        var earningsPeriodId = 3;
        var expected = new GetEarningByEarningPeriodDto()
        {
            EmployeeId = _employee.Id,
            EarningsPeriodId = earningsPeriodId,
            Deductions = [_deductions],
            GrossEarnings = Math.Round(_employee.Salary / _options.Value.PayPeriodsPerYear, 2, MidpointRounding.AwayFromZero),
            TotalDeductions = 5000m,
            NetEarnings =  Math.Round(_employee.Salary / _options.Value.PayPeriodsPerYear, 2, MidpointRounding.AwayFromZero) - 5000m,
        };
        var classUDT = new EarningsCalulator(_earningPeriodDeductions, _options);
        var result = classUDT.CalculateEarnings(earningsPeriodId, _employee);
        Assert.Equivalent(expected, result);
    }
}