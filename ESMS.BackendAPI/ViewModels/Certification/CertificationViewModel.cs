using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Certification
{
    public class CertificationViewModel
    {
        public int CertificationID { get; set; }
        public string CertificationName { get; set; }
        public string Description { get; set; }
        public int SkillID { get; set; }
        public string SkillName { get; set; }
        public int CertiLevel { get; set; }
        public bool Status { get; set; }
    }
}