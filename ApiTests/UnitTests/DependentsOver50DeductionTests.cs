using Api.Domain;
using Api.Domain.DomainServices.Deductions;
using Api.Domain.Dtos.Dependent;
using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ApiTests.UnitTests;

public class DependentsOver50DeductionTests
{
    private readonly IOptions<EarningsOptions> _options;
    private readonly TimeProvider _timeProvider;
    private readonly GetEmployeeDto _employee;

    public DependentsOver50DeductionTests()
    {
        _options = Substitute.For<IOptions<EarningsOptions>>();
        _options.Value.Returns(new EarningsOptions()
        {
            PayPeriodsPerYear = 4,
            Deductions = new DeductionsOptions(0, 0, 500, null!)
        });
        _employee = new GetEmployeeDto()
        {
            Dependents = new List<GetDependentDto>()
            {
                new GetDependentDto()
                {
                    DateOfBirth = new DateTime(1950, 01, 01),
                }
            }
        };
        _timeProvider = TimeProvider.System;
    }
    
    [Fact]
    public void DependentsOver50Deduction_IsDeductable_ReturnsFalse()
    {
        _employee.Dependents = new List<GetDependentDto>()
        {
            new GetDependentDto()
            {
                DateOfBirth = DateTime.Today.Subtract(TimeSpan.FromDays(365*8))
            }
        };
        var classUDT = new DependentsOver50Deduction(_options!, _timeProvider);
        var result = classUDT.IsDeductable(_employee);
        Assert.False(result);
    }
    
    [Fact]
    public void DependentsOver50Deduction_IsDeductable_ReturnsTrue()
    {
        var classUDT = new DependentsOver50Deduction(_options!, _timeProvider);
        var result = classUDT.IsDeductable(_employee);
        Assert.True(result);
    }

    [Fact]
    public void DependentsOver50Deduction_RetrieveDeduction_Returns_CorrecResult()
    {
        var classUDT = new DependentsOver50Deduction(_options!, _timeProvider);
        var expected = new EarningPeriodDeductionDto()
        {
            Amount = Math.Round(_options!.Value.Deductions.DependentOver50CostPerMonth * 12m / _options.Value.PayPeriodsPerYear, 
                2, MidpointRounding.AwayFromZero) * _employee.Dependents.Count,
            Description = $"{_employee.Dependents.Count} employee dependents over age 50 years."
        };
        var result = classUDT.RetrieveDeduction(_employee);
        Assert.Equivalent(expected, result);
    }
}
