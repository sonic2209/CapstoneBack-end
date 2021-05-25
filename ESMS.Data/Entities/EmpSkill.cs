using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class EmpSkill
    {
        public string EmpID { get; set; }
        public int SkillID { get; set; }
        public EnumSkillLevel? SkillLevel { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public Employee Employee { get; set; }
        public Skill Skill { get; set; }
    }
}