using CodeChallenge.API.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.API.Repositories
{
    public interface IEmployeesRepository:IGenericRepository<Employee>
    {
        Task RemoveAsync(long id);
        Task AddRangeAsync(IEnumerable<Employee> employees);
        Task<IEnumerable<AuditCommentDTO>> GetChangesByIdAsync(long id);
    }

    public class EmployeesRepository:GenericRepository<Employee>, IEmployeesRepository
    {
        public EmployeesRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
        public override async Task<Employee> GetByIdAsync(long id)
        {
            var res = await this._applicationDbContext.Set<Employee>().Where(x => x.Id == id).Include("Skillset").FirstOrDefaultAsync();
            for (int i = 0; i < res.Skillset.Count; i++)
            {
                res.Skillset[i].Employees = null;
            }
            return res;
        }
        public  async Task<IEnumerable<AuditCommentDTO>> GetChangesByIdAsync(long id)
        {
            var historyList = new List<AuditDTO>();
            var idJson = "{\"Id\":" + id + "}";
            var idJsonForSkills = "{\"EmployeesId\":" + id;
            var audits = await this._applicationDbContext.Set<Audit>().Where(x => ((x.KeyValues == idJson && x.TableName == "Employees") || (x.KeyValues.StartsWith(idJsonForSkills+",") && x.TableName == "EmployeeSkill")))
                    .Select(x => x).ToListAsync();
            foreach (var a in audits)
            {
                if (a.TableName=="Employees")
                {
                    AuditDTO dto;
                    if (a.Action == "Added")
                    {
                        dto = new AuditDTO() { DateTime = a.DateTime, Employee = JsonConvert.DeserializeObject<Employee>(a.NewValues),Action="Creation" };
                    }
                    else
                    {
                        dto = new AuditDTO() { DateTime = a.DateTime, Employee = JsonConvert.DeserializeObject<Employee>(a.NewValues), Action = a.Action };
                    }
                    historyList.Add(dto);
                }
                else
                {
                    var count = 29 + id.ToString().Length;
                    var totalCount = a.KeyValues.Length;
                    var skillIdLength = totalCount - count;
                    var skillId = a.KeyValues.Substring(count, skillIdLength-1);
                    var skillName= await this._applicationDbContext.Set<Skill>().Where(x => (x.Id.ToString())==skillId).Select(x => x.Name).FirstOrDefaultAsync();
                    if (skillName == null)
                    {
                        var skillIdJson = "{\"Id\":" + skillId + "}";
                        var skillDeletedJson = await this._applicationDbContext.Set<Audit>().Where(x => x.KeyValues == skillIdJson && x.TableName == "Skills" && x.Action=="Deleted").Select(x => x.OldValues).FirstOrDefaultAsync();
                        var skillDeleted = JsonConvert.DeserializeObject<Skill>(skillDeletedJson);
                        skillName = skillDeleted.Name;
                    }
                    var dto = new AuditDTO() { DateTime = a.DateTime, SkillName=skillName,Action=a.Action};
                    historyList.Add(dto);
                }
            }
            var auditComments = new List<AuditCommentDTO>();
            Employee previousEmployee = null;
            for (int i = 0; i < historyList.Count; i++)
            {
                if (historyList[i].Action == "Creation")
                {
                    previousEmployee = historyList[i].Employee;                   
                }
                if (historyList[i].Action == "Modified")
                {
                    if (historyList[i].Employee.Name == previousEmployee.Name && historyList[i].Employee.Surname == previousEmployee.Surname && historyList[i].Employee.HiringDate == previousEmployee.HiringDate)
                        historyList[i].Action = "Ignore";
                    else
                    {
                        previousEmployee = historyList[i].Employee;
                    }
                }
            }
            var historyListNew = new List<AuditDTO>();
            foreach( var h in historyList)
            {
                if (h.Action != "Ignore")
                    historyListNew.Add(h);
            }
            foreach (var x in historyListNew)
            {
                if (x.Action=="Creation")
                    auditComments.Add(new AuditCommentDTO() { DateTime = x.DateTime, Comment = x.Employee.Name + " " + x.Employee.Surname + " " + x.Employee.HiringDate.ToString("dd/MM/yyyy") + " Created" });
            }
            for (int i = 0; i < historyListNew.Count; i++)
            {
                if (historyListNew[i].Action == "Creation")
                {
                    previousEmployee = historyListNew[i].Employee;
                   
                }
                if (historyListNew[i].Action == "Modified")
                {
                    if (historyListNew[i].Employee.Name == previousEmployee.Name && historyListNew[i].Employee.Surname == previousEmployee.Surname && historyListNew[i].Employee.HiringDate == previousEmployee.HiringDate)
                        historyListNew[i].Action = "Ignore";
                    else
                    {
                        var newAuditComment = "";
                        if (historyListNew[i].Employee.Name != previousEmployee.Name)
                        {
                            newAuditComment = newAuditComment + "Name changed to: " + historyListNew[i].Employee.Name+" ";
                        }
                        if (historyListNew[i].Employee.Surname != previousEmployee.Surname)
                        {
                            newAuditComment = newAuditComment + "Surname changed to: " + historyListNew[i].Employee.Surname + " ";
                        }
                        if (historyListNew[i].Employee.HiringDate != previousEmployee.HiringDate)
                        {
                            newAuditComment = newAuditComment + "Hiring Date changed to: " + historyListNew[i].Employee.HiringDate.ToString("dd/MM/yyyy") + " ";
                        }
                        previousEmployee = historyListNew[i].Employee;
                        auditComments.Add(new AuditCommentDTO() {DateTime=  historyListNew[i].DateTime, Comment=newAuditComment});
                    }
                }
                if (historyListNew[i].Action == "Added")
                {
                    if (historyListNew[i].SkillName==null)
                        auditComments.Add(new AuditCommentDTO() { DateTime = historyListNew[i].DateTime, Comment = "Skill that no longer exists added" });
                    else
                        auditComments.Add(new AuditCommentDTO() { DateTime = historyListNew[i].DateTime, Comment = "Skill "+ historyListNew[i].SkillName+" added" });
                }
                if (historyListNew[i].Action == "Deleted")
                {
                    auditComments.Add(new AuditCommentDTO() { DateTime = historyListNew[i].DateTime, Comment = "Skill " + historyListNew[i].SkillName + " removed" });
                }
            }
            return auditComments;
        }
        public override async Task<IEnumerable<Employee>> GetAsync()
        {
            var res= await this._applicationDbContext.Set<Employee>().Include("Skillset").Select(x => x).ToListAsync();
            for (int i = 0; i < res.Count; i++)
            {
                for (int j = 0; j < res[i].Skillset.Count; j++)
                {
                    res[i].Skillset[j].Employees = null;
                }

            }
            return res;
        }
        public override async Task AddAsync(Employee employee)
        {           
            var skills = employee.Skillset;
            var _employee = employee;
            _employee.Skillset = new List<Skill>();
            foreach (var skill in skills)
            {
                if (skill.Id != 0)
                {
                    var _skill = await this._applicationDbContext.Set<Skill>().Where(x => x.Id == skill.Id).Select(x => x).FirstOrDefaultAsync();
                    _employee.Skillset.Add(_skill);
                }
                else
                {
                    _employee.Skillset.Add(skill);
                }

            }
            await this._applicationDbContext.Set<Employee>().AddAsync(_employee);
            await this._applicationDbContext.SaveChangesAsync();

        }
        public async Task AddRangeAsync(IEnumerable<Employee> employees)
        {
            foreach (var e in employees)
            {
                e.Id = 0;
                await this._applicationDbContext.Set<Employee>().AddAsync(e);
            }
            await this._applicationDbContext.SaveChangesAsync();

        }
        public override async Task UpdateAsync(Employee employee)
        {
            var _employee = await this._applicationDbContext.Set<Employee>().Where(x => x.Id == employee.Id).Include("Skillset").FirstOrDefaultAsync();
            var skills = employee.Skillset;
            foreach (var skill in skills)
            {
               if (skill.Id == 0)
               {
                   _employee.Skillset.Add(skill);
               }
               else if (skill.Name != "" && skill.Description != "")
               {
                   var _skill = await this._applicationDbContext.Set<Skill>().Where(x => x.Id == skill.Id).FirstOrDefaultAsync();
                   _employee.Skillset.Add(_skill);
               }
               else
               {
                   var skillDelete = _employee.Skillset.Where(x => x.Id == skill.Id).Select(x => x).FirstOrDefault();
                   var _skillDelete = await this._applicationDbContext.Set<Skill>().Where(x => x.Id == skillDelete.Id).Include("Employees").FirstOrDefaultAsync();
                   _employee.Skillset.Remove(skillDelete);
                   if (_skillDelete.Employees.Count == 1)
                   {
                       skillDelete.Employees = null;
                       this._applicationDbContext.Set<Skill>().Remove(skillDelete);
                   }
               }
            }
            _employee.Name = employee.Name;
            _employee.Surname = employee.Surname;
            _employee.HiringDate = employee.HiringDate;
            _employee.DateSkillChanged = employee.DateSkillChanged;
            _employee.TypeOfSkillChange = employee.TypeOfSkillChange;
            await this._applicationDbContext.SaveChangesAsync();
        }
        public override async Task RemoveRangeAsync(List<long> ids)
        {
            var employees = await _applicationDbContext.Set<Employee>().Where(x => ids.Contains(x.Id)).Select(x => x).Include("Skillset").ToListAsync();     
            for (int i = 0; i < employees.Count; i++)
            {
                for (int y = 0; y < employees[i].Skillset.Count; y++)
                {
                    var _skill = await _applicationDbContext.Set<Skill>().Where(x => x.Id == employees[i].Skillset[y].Id).Select(x => x).Include("Employees").FirstOrDefaultAsync();
                    if (_skill.Employees.Count==1 && !employees[i].Skillset[y].IsRequired)
                    {
                        var skillRemove = employees[i].Skillset[y];
                        skillRemove.Employees = null;
                        this._applicationDbContext.Set<Skill>().Remove(skillRemove);
                    }
                }
            }
            this._applicationDbContext.Set<Employee>().RemoveRange(employees);
            this._applicationDbContext.SaveChanges();
        }
        public async Task RemoveAsync(long id)
        {
            var _employee = await _applicationDbContext.Set<Employee>().Where(x => x.Id ==id).Select(x => x).Include("Skillset").FirstOrDefaultAsync();
            for (int y = 0; y < _employee.Skillset.Count; y++)
            {
                var _skill= await _applicationDbContext.Set<Skill>().Where(x => x.Id == _employee.Skillset[y].Id).Select(x => x).Include("Employees").FirstOrDefaultAsync();
                if (_skill.Employees.Count == 1 && !_employee.Skillset[y].IsRequired)
                {
                    var skillRemove = _employee.Skillset[y];
                    skillRemove.Employees = null;
                    this._applicationDbContext.Set<Skill>().Remove(skillRemove);
                }
            }
            this._applicationDbContext.Set<Employee>().Remove(_employee);
            this._applicationDbContext.SaveChanges();
        }

    }
}
