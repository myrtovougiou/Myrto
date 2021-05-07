using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.API.Models;
using CodeChallenge.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodeChallenge.API.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private IEmployeesRepository _employeesRepository;
        public EmployeesController(IEmployeesRepository employeesRepository)
        {
            this._employeesRepository = employeesRepository;
        }
        [HttpGet]
        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            return await this._employeesRepository.GetAsync();
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<Employee> GetEmployee(long id)
        {
            var employee= await this._employeesRepository.GetByIdAsync(id);
            return employee;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IEnumerable<AuditCommentDTO>> GetChangesEmployee(long id)        
        {
            var comments = await this._employeesRepository.GetChangesByIdAsync(id);
            return comments;
        }

        [HttpPost]
        public async Task CreateEmployee([FromBody] Employee employee)
        {
            await this._employeesRepository.AddAsync(employee);
        }
        [HttpPost]
        public async Task CreateEmployees([FromBody] IEnumerable<Employee> employees)
        {
            await this._employeesRepository.AddRangeAsync(employees);
        }

        [HttpPut]
        public async Task UpdateEmployee([FromBody] Employee employee)
        {
            await this._employeesRepository.UpdateAsync(employee);
        }

        [HttpDelete]
        [Route("{idsString}")]
        public async Task DeleteEmployees(string idsString)
        {
            var ids = JsonConvert.DeserializeObject<long[]>(idsString).ToList();
            await this._employeesRepository.RemoveRangeAsync(ids);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteEmployee(long id)
        {
           await this._employeesRepository.RemoveAsync(id);
        }
    }
}