using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;

namespace Api.Domain.Interfaces;

public interface IEarningPeriodDeduction
{
    /// <summary>
    /// Given an employee dto, it returns ture/false to indicate earning deduction applies for a period
    /// </summary>
    /// <param name="employee"></param>
    /// <returns>boolean</returns>
    bool IsDeductable(GetEmployeeDto employee);

    /// <summary>
    /// Given an earning period id and employee data, it return earning period deductions.
    /// 
    /// NOTE: earning period deductions are applied evenly across a given range. As such, method ignores
    ///       earning period id but accepts the id in anticipation requirements might change. 
    /// </summary>
    /// <param name="employee">employee related data</param>
    /// <param name="earningPeriodId">optional earning period id</param>
    /// <returns></returns>
    EarningPeriodDeductionDto RetrieveDeduction(GetEmployeeDto employee, int earningPeriodId)
        => RetrieveDeduction(employee);

    /// <summary>
    /// Given an employee data, method calculates applicable deductions.
    /// </summary>
    /// <param name="employee">employee related data</param>
    /// <returns></returns>
    EarningPeriodDeductionDto RetrieveDeduction(GetEmployeeDto employee);
}