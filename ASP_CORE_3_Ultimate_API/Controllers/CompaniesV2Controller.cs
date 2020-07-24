using Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployee.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/companies")]
    [ApiController]
    public class CompaniesV2Controller : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;

        public CompaniesV2Controller(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompaniesAsync()
        {
            var companies = await _repositoryManager.Company.GetAllCompaniesAsync(trackChanges: false);

            return Ok(companies);
        }
    }
}
