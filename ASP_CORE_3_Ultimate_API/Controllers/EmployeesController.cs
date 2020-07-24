using AutoMapper;
using CompanyEmployee.DTO;
using CompanyEmployee.Filters;
using Contracts;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployee.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/companies/{companyId}/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;
        private readonly IDataShaper<EmployeeDto> _dataShaper;

        public EmployeesController(IRepositoryManager repositoryManager, 
            ILoggerManager loggerManager, 
            IMapper mapper,
            IDataShaper<EmployeeDto> dataShaper)
        {
            _repositoryManager = repositoryManager;
            _loggerManager = loggerManager;
            _mapper = mapper;
            _dataShaper = dataShaper;
        }

        // api/companies/C9D4C053-49B6-410CBC78-2D54A9991870/employees?pageNumber=2&pageSize=2
        [HttpGet]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId, EmployeeRequestParameter reqParam)
        {
            var company = await _repositoryManager.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _loggerManager.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
            }
            
            var employeesFromDb = await _repositoryManager.Employee.GetEmployeesAsync(companyId, reqParam, trackChanges: false);

            var empDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

            return Ok(_dataShaper.ShapeData(empDto, reqParam.Fields));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = _repositoryManager.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _loggerManager.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employeeDb = await _repositoryManager.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);
            if (employeeDb == null)
            {
                _loggerManager.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }

            return Ok(employeeDb);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employeeDto)
        {
            if (employeeDto == null)
            {
                _loggerManager.LogError("EmployeeForCreationDto object sent from client is null.");
                return BadRequest("EmployeeForCreationDto object is null");
            }

            var company = _repositoryManager.Company.GetCompanyAsync(companyId, false);
            if (company == null)
            {
                _loggerManager.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
                return NotFound();
            }

            var employee = _mapper.Map<Employee>(employeeDto);

            await _repositoryManager.Employee.CreateEmployeeForCompanyAsync(companyId, employee);
            await _repositoryManager.SaveAsync();

            return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employee.Id }, employee);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            var company = await _repositoryManager.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _loggerManager.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
            }
            var employeeForCompany = await _repositoryManager.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);
            if (employeeForCompany == null)
            {
                _loggerManager.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            
            await _repositoryManager.Employee.DeleteEmployeeAsync(employeeForCompany);
            await _repositoryManager.SaveAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody]EmployeeForUpdateDto employee)
        {
            if (employee == null)
            {
                _loggerManager.LogError("EmployeeForUpdateDto object sent from client is null.");
                return BadRequest("EmployeeForUpdateDto object is null");
            }
            var company = await _repositoryManager.Company.GetCompanyAsync(companyId, trackChanges: false);
            if (company == null)
            {
                _loggerManager.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
            return NotFound();
            }
            var employeeEntity = await _repositoryManager.Employee.GetEmployeeAsync(companyId, id, trackChanges: true);
            if (employeeEntity == null)
            {
                _loggerManager.LogInfo($"Employee with id: {id} doesn't exist in the database.");
                return NotFound();
            }
            _mapper.Map(employee, employeeEntity);
            await _repositoryManager.SaveAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if(patchDoc == null)
            {
                _loggerManager.LogError("PatchDoc sent from client is null");
                return BadRequest("PatchDoc object is null");
            }
            var company = _repositoryManager.Company.GetCompanyAsync(companyId, false);

            if (company == null)
                return NotFound($"Company with Id {companyId} doesn't exist in database");

            var employee = _repositoryManager.Employee.GetEmployeeAsync(companyId, id, false);

            if (employee == null)
                return NotFound($"Employee with id {id} doesn't exist in database");

            var empToPatch = _mapper.Map<EmployeeForUpdateDto>(employee);

            patchDoc.ApplyTo(empToPatch);

            TryValidateModel(empToPatch);

            if(!ModelState.IsValid)
            {
                _loggerManager.LogError("Invalid model state for patch document");
                return UnprocessableEntity();
            }

            _mapper.Map(empToPatch, employee);

            _repositoryManager.SaveAsync();

            return NoContent();
        }
    }
}
