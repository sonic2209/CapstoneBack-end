using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Position
{
    public class HardSkillDetail
    {
        public int HardSkillID { get; set; }
        public int SkillLevel { get; set; }
        public int CertificationLevel { get; set; }
        public int Priority { get; set; }
    }
}