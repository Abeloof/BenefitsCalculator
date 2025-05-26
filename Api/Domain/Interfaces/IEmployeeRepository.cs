using System;
using Api.Data.Entities;
using Api.Domain.Dtos.Employee;

namespace Api.Domain.Interfaces;

public interface IEmployeeRepository
{
    Task<IList<Employee>> GetEmployees();
    Task<Employee> GetEmployeeById(int Id);
}
