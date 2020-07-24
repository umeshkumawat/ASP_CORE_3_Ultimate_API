using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reposiory
{
    // This is Time Of Work pattern. 
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _ctx;
        private ICompanyRepository _companyRepository;
        private IEmployeeRepository _employeeRepository;

        public RepositoryManager(RepositoryContext ctx)
        {
            _ctx = ctx;
        }
        public ICompanyRepository Company
        {
            get 
            {
                if (_companyRepository == null)
                    _companyRepository = new CompanyRepository(_ctx);
                return _companyRepository;
            }
        }

        public IEmployeeRepository Employee
        {
            get
            {
                if (_employeeRepository == null)
                    _employeeRepository = new EmployeeRepository(_ctx);
                return _employeeRepository;
            }
        }

        public async Task SaveAsync() => await _ctx.SaveChangesAsync();
    }
}
