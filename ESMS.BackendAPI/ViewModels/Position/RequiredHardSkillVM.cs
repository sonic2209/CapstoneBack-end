using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Position
{
    public class RequiredHardSkillVM
    {
        public int HardSkillID { get; set; }
        public string HardSkillName { get; set; }
        public int SkillLevel { get; set; }
        public int CertificationLevel { get; set; }
        public int Priority { get; set; }
    }
}