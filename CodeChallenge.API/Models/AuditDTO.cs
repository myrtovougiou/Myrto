using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.API.Models
{
    public class AuditDTO
    {
        public  DateTime DateTime { get; set; }
        public Employee Employee { get; set; }
        public string SkillName { get; set; }
        public string Action { get; set; }
    }
    public class AuditCommentDTO
    {
        public DateTime DateTime { get; set; }
        public string Comment { get; set; }
    }
}
