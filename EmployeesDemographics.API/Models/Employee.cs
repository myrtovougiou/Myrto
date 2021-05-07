using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeesDemographics.API.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime HiringDate { get; set; }
        public string Gender { get; set; }
        [Range(18, 90)]
        public int Age { get; set; }
        public string Ethnicity { get; set; }
        public string Education { get; set; }
        public string MaritalStatus { get; set; }
    }
}
