using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees
{
    public class EmpInfoViewModel
    {
        public int PosID { get; set; }
        public string PosName { get; set; }
        public int PosLevel { get; set; }
        public List<LanguageInfo> Languages { get; set; }
        public List<SoftSkillInfo> SoftSkills { get; set; }
        public List<HardSkillInfo> HardSkills { get; set; }
    }
}