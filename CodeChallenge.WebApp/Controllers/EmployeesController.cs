using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CodeChallenge.WebApp.Models;
using CodeChallenge.WebApp.Services;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodeChallenge.WebApp.Controllers
{
    public class EmployeesController : Controller
    {
        private IEmployeesService _employeesService;
        public EmployeesController(IEmployeesService employeesService)
        {
            _employeesService = employeesService;
        }
        public IActionResult Employees()
        {
            ViewBag.Title = "Employees";
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var employees = await this._employeesService.GetEmployees();
            return this.Ok(DataSourceLoader.Load(employees, loadOptions));
        }
        [HttpGet]
        public async Task<object> GetSkillsForEmployee(long id)
        {
            var employee = await this._employeesService.GetEmployee(id);
            return employee;
        }
        [HttpGet]
        public async Task<IEnumerable<AuditCommentDTO>> GetChangesForEmployee(long id)
        {
            var changes = await this._employeesService.GetChangesEmployee(id);
            return changes;
        }
        [HttpGet]
        public async Task<IEnumerable<Employee>> ImportEmployees()
        {
            var employees = await this._employeesService.ImportEmployees();
            return employees;
        }
        [HttpPost]
        public async Task CreateEmployee([FromBody] string employee)
        {
            await this._employeesService.CreateEmployee(employee);
        }
        [HttpPost]
        public async Task CreateEmployees([FromBody] string employee)
        {
            await this._employeesService.CreateEmployees(employee);
        }
        [HttpPut]
        public async Task EditEmployee([FromBody] string employee)
        {
            var employee1 = JsonConvert.DeserializeObject<Employee>(employee);
            await this._employeesService.UpdateEmployee(employee1);
        }
        [HttpDelete]
        public async Task DeleteEmployees(string idsString)
        {
            await this._employeesService.DeleteEmployees(idsString);
        }
        [HttpDelete]
        public async Task DeleteEmployee(long id)
        {
            await this._employeesService.DeleteEmployee(id);
        }
    }
}