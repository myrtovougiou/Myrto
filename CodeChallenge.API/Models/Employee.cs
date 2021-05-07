using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.API.Models
{
    public class Employee:BaseEntity
    {
        public string Surname { get; set; }
        public DateTime HiringDate { get; set; }
        public List<Skill> Skillset { get; set; }
        public DateTime DateSkillChanged { get; set; }
        public string TypeOfSkillChange { get; set; }
    }
}
