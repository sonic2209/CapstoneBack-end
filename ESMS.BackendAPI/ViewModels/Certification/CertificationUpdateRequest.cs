using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Certification
{
    public class CertificationUpdateRequest
    {
        public string CertificationName { get; set; }
        public string Description { get; set; }
        public int SkillID { get; set; }
    }
}