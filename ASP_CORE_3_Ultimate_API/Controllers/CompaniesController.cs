using AutoMapper;
using CompanyEmployee.DTO;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployee.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;

        public CompaniesController(IRepositoryManager repositoryManager, ILoggerManager loggerManager, IMapper mapper)
        {
            _repositoryManager = repositoryManager;
            _loggerManager = loggerManager;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCompaniesAsync()
        {
            var companies = await _repositoryManager.Company.GetAllCompaniesAsync(trackChanges: false);

            var companyDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);

            return Ok(companyDto);
        }

        [HttpGet("{id}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompanyAsync(Guid id)
        {
            var company = await _repositoryManager.Company.GetCompanyAsync(id, trackChanges: false);

            if (company == null)
            {
                _loggerManager.LogError($"Company with Id {id} doesn't exist in database.");
                return NotFound();
            }
            else
            {
                var companyDto = _mapper.Map<CompanyDto>(company);
                return Ok(company);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto companyCreationDto)
        {
            if (companyCreationDto == null)
                return BadRequest("No company data specified");

            var company = _mapper.Map<Company>(companyCreationDto);

            await _repositoryManager.Company.CreateCompanyAsync(company);
            await _repositoryManager.SaveAsync();

            return CreatedAtRoute("CompanyById", new { id = company.Id }, company);
        }

        [HttpGet("collection/({ids})")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            return Ok(await _repositoryManager.Company.GetByIdsAsync(ids, false));
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody]IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                _loggerManager.LogError("Company collection sent from client is null.");
                return BadRequest("Company collection is null");
            }
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                await _repositoryManager.Company.CreateCompanyAsync(company);
            }
            await _repositoryManager.SaveAsync();
            var companyCollectionToReturn =
            _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CompanyCollection", new { ids },
            companyCollectionToReturn);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            var company = await _repositoryManager.Company.GetCompanyAsync(id, trackChanges: false);
            if (company == null)
            {
                _loggerManager.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            await _repositoryManager.Company.DeleteCompanyAsync(company);
            await _repositoryManager.SaveAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            if (company == null)
            {
                _loggerManager.LogError("CompanyForUpdateDto object sent from client is null.");
                return BadRequest("CompanyForUpdateDto object is null");
            }
            var companyEntity = await _repositoryManager.Company.GetCompanyAsync(id, trackChanges: true);
            if (companyEntity == null)
            {
                _loggerManager.LogInfo($"Company with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _mapper.Map(company, companyEntity);
            await _repositoryManager.SaveAsync();
            return NoContent();
        }
    }
}
