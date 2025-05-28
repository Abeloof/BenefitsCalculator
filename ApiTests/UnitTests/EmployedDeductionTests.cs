using Api.Domain;
using Api.Domain.DomainServices.Deductions;
using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ApiTests.UnitTests;

public class EmployedDeductionTests
{
    private readonly IOptions<EarningsOptions> _options;
    private readonly GetEmployeeDto _employee;
    
    public EmployedDeductionTests()
    {
        _options = Substitute.For<IOptions<EarningsOptions>>();
        _options.Value.Returns(new EarningsOptions()
        {
            PayPeriodsPerYear = 4,
            Deductions = new DeductionsOptions(400, 0, 0, null!)
        });
        _employee = new GetEmployeeDto()
        {
            Salary = 12000
        };
    }
    
    [Fact]
    public void EmployedDeduction_IsDeductable_ReturnsFalse()
    {
        _employee.Salary = 200;
        var classUDT = new EmployedDeduction(_options!);
        var result = classUDT.IsDeductable(_employee);
        Assert.False(result);
    }
    
    [Fact]
    public void EmployedDeduction_IsDeductable_ReturnsTrue()
    {
        var classUDT = new EmployedDeduction(_options!);
        var result = classUDT.IsDeductable(_employee);
        Assert.True(result);
    }

    [Fact]
    public void EmployedDeduction_RetrieveDeduction_Returns_CorrecResult()
    {
        var classUDT = new EmployedDeduction(_options!);
        var expected = new EarningPeriodDeductionDto()
        {
            Amount = Math.Round(((_options!.Value.Deductions.EmployeeCostPerMonth * 12m) / _options.Value.PayPeriodsPerYear), 
                2, MidpointRounding.AwayFromZero),
            Description = "Employed."
        };   
        var result = classUDT.RetrieveDeduction(_employee);
        Assert.Equivalent(expected, result);        
    }
}
