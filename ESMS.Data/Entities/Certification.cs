using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class Certification
    {
        public int CertificationID { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int EmpSkillID { get; set; }
        public DateTime DateTaken { get; set; }
        public DateTime DateEnd { get; set; }
        public EmpSkill EmpSkill { get; set; }
    }
}