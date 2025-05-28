using Api.Domain;
using Api.Domain.DomainServices.Deductions;
using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ApiTests.UnitTests;

public class SalaryBasedDeductionTests
{
    private readonly IOptions<EarningsOptions>? _options = null;
    private readonly GetEmployeeDto _employee;

    public SalaryBasedDeductionTests()
    {
        _options = Substitute.For<IOptions<EarningsOptions>>();
        _options.Value.Returns(new EarningsOptions()
        {
            PayPeriodsPerYear = 4,
            Deductions = new DeductionsOptions(0, 0, 0, new SalaryDeductionOptions(10000, 3))
        });
        _employee = new GetEmployeeDto()
        {
            Salary = 12000
        };
    }

    [Fact]
    public void SalaryBasedDeduction_IsDeductable_ReturnsFalse()
    {
        _employee.Salary = 200;
        var classUDT = new SalaryBasedDeduction(_options!);
        var result = classUDT.IsDeductable(_employee);
        Assert.False(result);
    }
    
    [Fact]
    public void SalaryBasedDeduction_IsDeductable_ReturnsTrue()
    {
        var classUDT = new SalaryBasedDeduction(_options!);
        var result = classUDT.IsDeductable(_employee);
        Assert.True(result);
    }

    [Fact]
    public void SalaryBasedDeduction_RetrieveDeduction_Returns_CorrecResult()
    {
        var expected = new EarningPeriodDeductionDto()
        {
            Amount = Math.Round(((_options!.Value.Deductions.SalaryDeduction.DeductionPercentPerYear  / 100m) 
                    * _employee.Salary / _options.Value.PayPeriodsPerYear), 
                2, MidpointRounding.AwayFromZero),
            Description = $"Employee salary over {_options!.Value.Deductions.SalaryDeduction.Over}."
        };  
        var classUDT = new SalaryBasedDeduction(_options!);
        var result = classUDT.RetrieveDeduction(_employee);
        Assert.Equivalent(expected, result);   
    }
}
