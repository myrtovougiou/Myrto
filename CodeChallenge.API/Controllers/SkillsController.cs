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
    public class SkillsController : ControllerBase
    {
        private ISkillsRepository _skillsRepository;
        public SkillsController(ISkillsRepository skillsRepository)
        {
            this._skillsRepository = skillsRepository;
        }

        [HttpGet]
        public async Task<IEnumerable<Skill>> GetRequiredSkills()
        {
            return await this._skillsRepository.GetAsync();
        }
        [HttpGet]
        public async Task<IEnumerable<Skill>> GetAllSkills()
        {
            return await this._skillsRepository.GetAllAsync();
        }

        [HttpPost]
        public async Task CreateSkill([FromBody] Skill skill)
        {
            await this._skillsRepository.AddAsync(skill);
        }

        [HttpPut]
        public async Task UpdateSkill([FromBody] Skill skill)
        {
            await this._skillsRepository.UpdateAsync(skill);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task DeleteSkill(long id)
        {
            var skill=await this._skillsRepository.GetByIdAsync(id);
            await this._skillsRepository.RemoveAsync(skill);
        }
    }
}