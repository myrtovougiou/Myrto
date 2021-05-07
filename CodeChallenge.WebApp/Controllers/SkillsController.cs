using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.WebApp.Models;
using CodeChallenge.WebApp.Services;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CodeChallenge.WebApp.Controllers
{
    public class SkillsController : Controller
    {
        private ISkillsService _skillsService;
        public SkillsController(ISkillsService skillsService)
        {
            this._skillsService = skillsService;
        }

        public IActionResult Skills()
        {
            ViewBag.Title = "Skills";
            return View();
        }
        [HttpGet]
        public async Task<IEnumerable<Skill>> GetRequiredSkills()
        {
            var skills = await this._skillsService.GetRequiredSkills();
            return skills;
        }
        [HttpGet]
        public async Task<IEnumerable<Skill>> GetAllSkills()
        {
            var skills = await this._skillsService.GetAllSkills();
            return skills;
        }
        public async Task<IActionResult> Get(DataSourceLoadOptions loadOptions)
        {
            var skills = await this._skillsService.GetRequiredSkills();
            return this.Ok(DataSourceLoader.Load(skills, loadOptions));
        }
        public async Task<IActionResult> GetAll(DataSourceLoadOptions loadOptions)
        {
            var skills = await this._skillsService.GetAllSkills();
            return this.Ok(DataSourceLoader.Load(skills, loadOptions));
        }
        [HttpPut]
        public async Task EditSkill([FromBody] string skill)
        {
            Skill skill1 = JsonConvert.DeserializeObject<Skill>(skill);
            await this._skillsService.UpdateSkill(skill1);            
        }
        
        [HttpPost]
        public async Task CreateSkill([FromBody] string skill)
        {
            Skill skill1 = JsonConvert.DeserializeObject<Skill>(skill);
            await this._skillsService.CreateSkill(skill1);
        }
        [HttpDelete]
        public async Task DeleteSkill(long id)
        {
            await this._skillsService.DeleteSkill(id);
        }
    }
}