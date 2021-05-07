using CodeChallenge.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.API.Repositories
{
    public interface ISkillsRepository:IGenericRepository<Skill>
    {
        Task<IEnumerable<Skill>> GetAllAsync();
    }
    public class SkillsRepository:GenericRepository<Skill>, ISkillsRepository
    {
        public SkillsRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext) { }
        public override async Task<IEnumerable<Skill>> GetAsync()
        {
            return await this._applicationDbContext.Set<Skill>().Where(x=>x.IsRequired==true).Select(x => x).ToListAsync();
        }
        public async Task<IEnumerable<Skill>> GetAllAsync()
        {
            return await this._applicationDbContext.Set<Skill>().Select(x => x).ToListAsync();
        }
        public override async Task UpdateAsync(Skill skill)
        {
            var _skill =await this._applicationDbContext.Set<Skill>().Where(x => x.Id == skill.Id).FirstOrDefaultAsync();
            _skill.Name = skill.Name;
            _skill.Description = skill.Description;
            _skill.IsRequired = skill.IsRequired;
            _skill.DateCreated = skill.DateCreated;
            await this._applicationDbContext.SaveChangesAsync();
        }
    }
}
