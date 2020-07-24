using Contracts;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reposiory
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext ctx)
            : base(ctx)
        {

        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, EmployeeRequestParameter employeeRequestParameter, bool trackChanges)
        {
            return await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
                .OrderBy(e => e.Name)
                .Skip((employeeRequestParameter.PageNumber - 1) * employeeRequestParameter.PageSize)
                .Take(employeeRequestParameter.PageSize)
                .ToListAsync();
        }
        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            return await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id),
            trackChanges)
            .SingleOrDefaultAsync();
        }

        public async Task CreateEmployeeForCompanyAsync(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            await Task.Run(() => Create(employee));
        }

        public async Task DeleteEmployeeAsync(Employee employee)
        {
            await Task.Run(() => Delete(employee));
        }
    }
}
