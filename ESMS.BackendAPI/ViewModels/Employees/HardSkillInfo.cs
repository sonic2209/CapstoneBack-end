using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees
{
    public class HardSkillInfo
    {
        public int SkillID { get; set; }
        public string SkillName { get; set; }
        public int? SkillLevel { get; set; }
        public List<CertiInfo> Certifications { get; set; }
    }
}