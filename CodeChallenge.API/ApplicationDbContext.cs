using CodeChallenge.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeChallenge.API
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILoggerFactory loggerFactory)
            : base(options)
        {
            this._loggerFactory = loggerFactory;
        }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Audit> Audits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLoggerFactory(this._loggerFactory);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>().Property(x => x.Name).HasMaxLength(64).IsRequired();
            modelBuilder.Entity<Skill>().Property(x => x.Description).HasMaxLength(256).IsRequired();
            modelBuilder.Entity<Skill>().Property(x => x.DateCreated).HasColumnType("date");
            modelBuilder.Entity<Employee>().Property(x => x.Name).HasMaxLength(64).IsRequired();
            modelBuilder.Entity<Employee>().Property(x => x.Surname).HasMaxLength(128).IsRequired();
            modelBuilder.Entity<Employee>().Property(x => x.HiringDate).HasColumnType("date");
            modelBuilder.Entity<Employee>().Property(x => x.DateSkillChanged).HasColumnType("date");
            modelBuilder.Entity<Employee>().Property(x => x.TypeOfSkillChange).HasMaxLength(64).IsRequired();
            modelBuilder.Entity<Skill>().HasIndex(p => p.Description).IsUnique();
            modelBuilder.Entity<Skill>().HasIndex(p => p.Name).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(p => new { p.Name, p.Surname, p.HiringDate }).IsUnique();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var temoraryAuditEntities = await AuditNonTemporaryProperties();
            var result = await base.SaveChangesAsync();
            await AuditTemporaryProperties(temoraryAuditEntities);
            return result;
        }
        async Task<IEnumerable<Tuple<EntityEntry, Audit>>> AuditNonTemporaryProperties()
        {
            this.ChangeTracker.DetectChanges();
            var entitiesToTrack = this.ChangeTracker.Entries().Where(e => !(e.Entity is Audit) && e.State != EntityState.Detached && e.State != EntityState.Unchanged);

            await this.Audits.AddRangeAsync(
                entitiesToTrack.Where(e => !e.Properties.Any(p => p.IsTemporary)).Select(e => new Audit()
                {
                    TableName = e.Metadata.GetTableName(),
                    Action = Enum.GetName(typeof(EntityState), e.State),
                    DateTime = DateTime.Now.ToUniversalTime(),
                    KeyValues = JsonConvert.SerializeObject(e.Properties.Where(p => p.Metadata.IsPrimaryKey()).ToDictionary(p => p.Metadata.Name, p => p.CurrentValue).NullIfEmpty()),
                    NewValues = JsonConvert.SerializeObject(e.Properties.Where(p => e.State == EntityState.Added || e.State == EntityState.Modified).ToDictionary(p => p.Metadata.Name, p => p.CurrentValue).NullIfEmpty()),
                    OldValues = JsonConvert.SerializeObject(e.Properties.Where(p => e.State == EntityState.Deleted || e.State == EntityState.Modified).ToDictionary(p => p.Metadata.Name, p => p.OriginalValue).NullIfEmpty())
                }).ToList()
            );

            //Return list of pairs of EntityEntry and ToolAudit  
            return entitiesToTrack.Where(e => e.Properties.Any(p => p.IsTemporary))
                 .Select(e => new Tuple<EntityEntry, Audit>(e, new Audit()
                 {
                     TableName = e.Metadata.GetTableName(),
                     Action = Enum.GetName(typeof(EntityState), e.State),
                     DateTime = DateTime.Now.ToUniversalTime(),
                     NewValues = JsonConvert.SerializeObject(e.Properties.Where(p => !p.Metadata.IsPrimaryKey()).ToDictionary(p => p.Metadata.Name, p => p.CurrentValue).NullIfEmpty())
                 }
                 )).ToList();
        }

        async Task AuditTemporaryProperties(IEnumerable<Tuple<EntityEntry, Audit>> temporatyEntities)
        {
            if (temporatyEntities != null && temporatyEntities.Any())
            {
                await this.Audits.AddRangeAsync(
                temporatyEntities.ForEach(t => t.Item2.KeyValues = JsonConvert.SerializeObject(t.Item1.Properties.Where(p => p.Metadata.IsPrimaryKey()).ToDictionary(p => p.Metadata.Name, p => p.CurrentValue).NullIfEmpty()))
                    .Select(t => t.Item2)
                );
                await this.SaveChangesAsync();
            }
            await Task.CompletedTask;
        }
    }
    public static class Extensions
    {
        public static IDictionary<TKey, TValue> NullIfEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            if (dictionary == null || !dictionary.Any())
            {
                return null;
            }
            return dictionary;
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (T element in source)
            {
                action(element);
            }
            return source;
        }
    }
}
