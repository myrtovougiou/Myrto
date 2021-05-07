using CodeChallenge.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.API.Repositories
{
    public interface IGenericRepository<T> where T:BaseEntity
    {
        Task<IEnumerable<T>> GetAsync();
        Task<T> GetByIdAsync(long id);
        Task AddAsync(T baseEntity);
        Task UpdateAsync(T baseEntity);
        Task RemoveAsync(T baseEntity);
        Task RemoveRangeAsync(List<long> ids);

    }
    public class GenericRepository<T>:IGenericRepository<T> where T:BaseEntity
    {
        protected ApplicationDbContext _applicationDbContext;
        public GenericRepository(ApplicationDbContext applicationDbContext) 
        {
            this._applicationDbContext = applicationDbContext;
        }
        public virtual async Task<IEnumerable<T>> GetAsync()
        {
            return await this._applicationDbContext.Set<T>().Select(x => x).ToListAsync();
        }
        public virtual async Task<T> GetByIdAsync(long id)
        {
            var res= await this._applicationDbContext.Set<T>().Where(x => x.Id == id).FirstOrDefaultAsync();
            return res;
        }
        public virtual async Task AddAsync(T baseEntity)
        {
            await this._applicationDbContext.Set<T>().AddAsync(baseEntity);
            await this._applicationDbContext.SaveChangesAsync();
        }
        public virtual async Task UpdateAsync(T baseEntity)
        {
            var _baseEntity = await this._applicationDbContext.Set<T>().Where(x => x.Id == baseEntity.Id).FirstOrDefaultAsync();
            _baseEntity.Name = baseEntity.Name;
            await this._applicationDbContext.SaveChangesAsync();
        }
        public virtual async Task RemoveAsync(T baseEntity)
        {
            this._applicationDbContext.Set<T>().Remove(baseEntity);
            await this._applicationDbContext.SaveChangesAsync();
        }
        public virtual async Task RemoveRangeAsync(List<long> ids)
        {
            var entities = await _applicationDbContext.Set<T>().Where(x => ids.Contains(x.Id)).Select(x => x).ToListAsync();
            this._applicationDbContext.Set<T>().RemoveRange(entities);
            this._applicationDbContext.SaveChanges();
        }

    }
}
