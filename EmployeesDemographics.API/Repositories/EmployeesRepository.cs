using EmployeesDemographics.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeesDemographics.API.Repositories
{
    public class EmployeesRepository:IEmployeesRepository
    {
        private ApplicationDbContext _applicationDbContext;
        public EmployeesRepository(ApplicationDbContext applicationDbContext)
        {
            this._applicationDbContext = applicationDbContext;
        }
        public async Task<IEnumerable<Employee>> GetAsync()
        {
            return await this._applicationDbContext.Set<Employee>().Select(x => x).ToListAsync();
        }
    }
    public interface IEmployeesRepository
    {
        Task<IEnumerable<Employee>> GetAsync();
    }
}
