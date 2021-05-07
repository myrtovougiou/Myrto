using EmployeesDemographics.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeesDemographics.API
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().Property(x => x.Name).HasMaxLength(64).IsRequired();
            modelBuilder.Entity<Employee>().Property(x => x.Surname).HasMaxLength(128).IsRequired();
            modelBuilder.Entity<Employee>().Property(x => x.Gender).HasMaxLength(64).IsRequired();
            modelBuilder.Entity<Employee>().Property(x => x.Ethnicity).HasMaxLength(64).IsRequired();
            modelBuilder.Entity<Employee>().Property(x => x.Education).HasMaxLength(64).IsRequired();
            modelBuilder.Entity<Employee>().Property(x => x.MaritalStatus).HasMaxLength(64).IsRequired();
            modelBuilder.Entity<Employee>().Property(x => x.HiringDate).HasColumnType("date");
            modelBuilder.Entity<Employee>().HasIndex(p => new { p.Name, p.Surname, p.HiringDate }).IsUnique();
        }
    }
}
