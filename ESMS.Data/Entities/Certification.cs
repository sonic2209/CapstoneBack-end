using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class Certification
    {
        public int CertificationID { get; set; }
        public string CertificationName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int SkillID { get; set; }
        public Skill Skill { get; set; }
        public List<EmpCertification> EmpCertifications { get; set; }
    }
}