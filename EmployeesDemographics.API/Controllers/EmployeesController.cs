using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeesDemographics.API.Models;
using EmployeesDemographics.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EmployeesDemographics.API.Controllers
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
    }
}