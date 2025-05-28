using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;

namespace Api.Domain.Interfaces;

public interface IEarningPeriodDeduction
{
    bool IsDeductable(GetEmployeeDto employee);
    EarningPeriodDeductionDto RetrieveDeduction(GetEmployeeDto employee, int earningPeriodId)
        => RetrieveDeduction(employee);
    EarningPeriodDeductionDto RetrieveDeduction(GetEmployeeDto employee);
}