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
    public class SkillsService:BaseService, ISkillsService
    {
        public async Task<IEnumerable<Skill>> GetRequiredSkills()
        {
            var httpClient = CreateHttpClient();
            var res = await httpClient.GetAsync(Constants.Constants.GetRequiredSkills);
            if (res.IsSuccessStatusCode)
            {
                var jsonResult = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<IEnumerable<Skill>>(jsonResult);
            }
            return null;
        }
        public async Task<IEnumerable<Skill>> GetAllSkills()
        {
            var httpClient = CreateHttpClient();           
            var res = await httpClient.GetAsync(Constants.Constants.GetAllSkills);
            if (res.IsSuccessStatusCode)
            {
                var jsonResult = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                return JsonConvert.DeserializeObject<IEnumerable<Skill>>(jsonResult);
            }
            return null;
        }
        public async Task UpdateSkill(Skill skill)
        {
            var httpClient = CreateHttpClient();           
            var content = new StringContent(JsonConvert.SerializeObject(skill), Encoding.UTF8, "application/json");
            await httpClient.PutAsync(Constants.Constants.UpdateSkill, content);
        }
        public async Task CreateSkill(Skill skill)
        {
            var httpClient = CreateHttpClient();          
            var content = new StringContent(JsonConvert.SerializeObject(skill), Encoding.UTF8, "application/json");
            await httpClient.PostAsync(Constants.Constants.CreateSkill, content);
        }
        public async Task DeleteSkill(long id)
        {
            var httpClient = CreateHttpClient();         
            await httpClient.DeleteAsync(Constants.Constants.DeleteSkill+id);
        }
    }

    public interface ISkillsService
    {
        Task<IEnumerable<Skill>> GetRequiredSkills();
        Task<IEnumerable<Skill>> GetAllSkills();
        Task UpdateSkill(Skill skill);
        Task CreateSkill(Skill skill);
        Task DeleteSkill(long id);
    }
}
