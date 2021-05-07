using CodeChallenge.WebApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CodeChallenge.WebApp.Services
{
    public class EmployeesService:BaseService, IEmployeesService
    {
        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            var httpClient = CreateHttpClient();
            var res = await httpClient.GetAsync(Constants.Constants.GetEmployees);
            if (res.IsSuccessStatusCode)
            {
                var jsonResult = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<IEnumerable<Employee>>(jsonResult);
            }
            return null;
        }
        public async Task<Employee> GetEmployee(long id)
        {
            var httpClient = CreateHttpClient();
            var res = await httpClient.GetAsync(Constants.Constants.GetEmployee+id);
            if (res.IsSuccessStatusCode)
            {
                var jsonResult = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<Employee>(jsonResult);
            }
            return null;
        }
        public async Task CreateEmployee(string employee)
        {
            var httpClient = CreateHttpClient();           
            var content = new StringContent(employee, Encoding.UTF8, "application/json");
            await httpClient.PostAsync(Constants.Constants.CreateEmployee, content);
        }
        public async Task CreateEmployees(string employees)
        {
            var httpClient = CreateHttpClient();           
            var content = new StringContent(employees, Encoding.UTF8, "application/json");
            await httpClient.PostAsync(Constants.Constants.CreateEmployees, content);
        }
        public async Task UpdateEmployee(Employee employee)
        {
            var httpClient = CreateHttpClient();          
            var content = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");
            var res = await httpClient.PutAsync(Constants.Constants.UpdateEmployee, content);
        }
        public async Task DeleteEmployees(string ids)
        {
            var httpClient = CreateHttpClient();           
            var res = await httpClient.DeleteAsync(Constants.Constants.DeleteEmployees+ids);
        }
        public async Task DeleteEmployee(long id)
        {
            var httpClient = CreateHttpClient();           
            await httpClient.DeleteAsync(Constants.Constants.DeleteEmployee + id);
        }
        public async Task<IEnumerable<Employee>> ImportEmployees()
        {
            var httpClient = CreateHttpClient();           
            var res = await httpClient.GetAsync(Constants.Constants.ExternalUri);
            if (res.IsSuccessStatusCode)
            {
                var jsonResult = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<IEnumerable<Employee>>(jsonResult);
            }
            return null;
        }
        public async Task<IEnumerable<AuditCommentDTO>> GetChangesEmployee(long id)
        {
            var httpClient = CreateHttpClient();           
            var res = await httpClient.GetAsync(Constants.Constants.GetChangesEmployee + id);
            if (res.IsSuccessStatusCode)
            {
                var jsonResult = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                var changes = JsonConvert.DeserializeObject<IEnumerable<AuditCommentDTO>>(jsonResult);
                return changes;
            }
            return null;
        }
    }
    public interface IEmployeesService
    {
        Task<IEnumerable<Employee>> GetEmployees();
        Task<Employee> GetEmployee(long id);
        Task UpdateEmployee(Employee employee);
        Task CreateEmployee(string employee);
        Task CreateEmployees(string employee);
        Task DeleteEmployees(string ids);
        Task DeleteEmployee(long id);
        Task<IEnumerable<Employee>> ImportEmployees();
        Task<IEnumerable<AuditCommentDTO>> GetChangesEmployee(long id);
    }
}
