using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.WebApp.Constants
{
    public static class Constants
    {
        public static string BaseUri =  "https://localhost:44355/";

        public static string GetRequiredSkills = BaseUri + "skills/GetRequiredSkills";
        public static string GetAllSkills = BaseUri + "skills/GetAllSkills";
        public static string UpdateSkill = BaseUri + "skills/UpdateSkill";
        public static string CreateSkill = BaseUri + "skills/CreateSkill";
        public static string DeleteSkill = BaseUri + "skills/DeleteSkill/";

        public static string GetEmployees = BaseUri + "employees/GetEmployees";
        public static string GetEmployee = BaseUri + "employees/GetEmployee/";
        public static string CreateEmployee = BaseUri + "employees/CreateEmployee";
        public static string CreateEmployees = BaseUri + "employees/CreateEmployees";
        public static string UpdateEmployee = BaseUri + "employees/UpdateEmployee";
        public static string DeleteEmployees = BaseUri + "employees/DeleteEmployees/";
        public static string DeleteEmployee = BaseUri + "employees/DeleteEmployee/";
        public static string GetChangesEmployee = BaseUri + "employees/GetChangesEmployee/";

        public static string ExternalUri = "https://localhost:44365/employees/GetEmployees";
    }
}
