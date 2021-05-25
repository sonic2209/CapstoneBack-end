using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees.Suggestion
{
    public class EmpInHardSkill
    {
        public string EmpID { get; set; }
        public int SkillID { get; set; }
        public EnumSkillLevel? SkillLevel { get; set; }
    }
}