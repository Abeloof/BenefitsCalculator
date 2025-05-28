using Api.Domain.Dtos.Earnings;
using Api.Domain.Dtos.Employee;

namespace Api.Domain.Interfaces;

public interface IEarningsCalculator
{
    /// <summary>
    /// Given an earning period id and an empoyee's data, it returns calcluated earnings for given period
    /// </summary>
    /// <param name="earningPeriodId"></param>
    /// <param name="employee"></param>
    /// <returns></returns>
    GetEarningByEarningPeriodDto CalculateEarnings(int earningPeriodId, GetEmployeeDto employee);
}
