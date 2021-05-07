using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.WebApp.Models
{
    public class Skill:BaseEntity
    {
        public string Description { get; set; }
        public bool IsRequired { get; set; }
        public DateTime DateCreated { get; set; }
        public List<Employee> Employees { get; set; }
    }
}
