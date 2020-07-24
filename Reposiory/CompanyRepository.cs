using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reposiory
{
    public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
    {
        public CompanyRepository(RepositoryContext ctx)
            : base(ctx)
        {

        }

        public Task CreateCompanyAsync(Company company)
        {
            Create(company);
            return Task.CompletedTask;
        }

        public async Task DeleteCompanyAsync(Company company)
        {
            await Task.Run(() => Delete(company));
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges)
        {
            return await FindAll(trackChanges)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
        {
            return await FindByCondition(c => ids.Contains(c.Id), trackChanges).ToListAsync();
        }

        public async Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges)
        {
            return await FindByCondition(c => c.Id == companyId, trackChanges)
                .SingleOrDefaultAsync();
        }
    }
}
