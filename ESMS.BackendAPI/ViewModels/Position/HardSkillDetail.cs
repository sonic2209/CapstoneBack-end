using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Position
{
    public class HardSkillDetail
    {
        public int HardSkillID { get; set; }
        public int Exp { get; set; }
        public int CertificationID { get; set; }
        public int CertificationLevel { get; set; }
        public int Priority { get; set; }
    }
}