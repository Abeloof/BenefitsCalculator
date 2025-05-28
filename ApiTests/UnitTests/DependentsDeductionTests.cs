using Api.Domain;
using Api.Domain.DomainServices.Deductions;
using Api.Domain.Dtos.Dependent;
using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ApiTests.UnitTests;

public class DependentsDeductionTests
{
    private readonly IOptions<EarningsOptions> _options;
    private readonly GetEmployeeDto _employee;
    
    public DependentsDeductionTests()
    {
        _options = Substitute.For<IOptions<EarningsOptions>>();
        _options.Value.Returns(new EarningsOptions()
        {
            PayPeriodsPerYear = 4,
            Deductions = new DeductionsOptions(0, 200, 0, null!)
        });
        _employee = new GetEmployeeDto()
        {
            Dependents = new List<GetDependentDto>()
            {
                new GetDependentDto(),
                new GetDependentDto()
            }
        };
    }
    
    [Fact]
    public void DependentsDeduction_IsDeductable_ReturnsTrue()
    {
        var classUDT = new DependentsDeduction(_options!);
        var result = classUDT.IsDeductable(_employee);
        Assert.True(result);
    }
    
    [Fact]
    public void DependentsDeduction_IsDeductable_ReturnsFalse()
    {
        _employee.Dependents = new List<GetDependentDto>();
        var classUDT = new DependentsDeduction(_options!);
        var result = classUDT.IsDeductable(_employee);
        Assert.False(result);
    }
    
    [Fact]
    public void DependentsDeduction_RetrieveDeduction_Returns_CorrecResult()
    {
        var classUDT = new DependentsDeduction(_options!);
        var expected = new EarningPeriodDeductionDto()
        {
            Amount = Math.Round(((_options!.Value.Deductions.DependentCostPerMonth * 12m) / _options.Value.PayPeriodsPerYear), 
                2, MidpointRounding.AwayFromZero) * _employee.Dependents.Count,
            Description =  $"{_employee.Dependents.Count} employee dependents."
        };
        var result = classUDT.RetrieveDeduction(_employee);
        Assert.Equivalent(expected, result);
    }
}