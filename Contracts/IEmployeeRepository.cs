using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, EmployeeRequestParameter employeeRequestParameter, bool trackChanges);
        Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges);
        Task CreateEmployeeForCompanyAsync(Guid companyId, Employee employee);
        Task DeleteEmployeeAsync(Employee employee);
    }
}
