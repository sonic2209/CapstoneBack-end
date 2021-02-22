using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class EmpSkill
    {
        public int EmpSkillID { get; set; }
        public string EmpID { get; set; }
        public int SkillID { get; set; }
        public SkillLevel SkillLevel { get; set; }
        public int Exp { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public Employee Employee { get; set; }
        public Skill Skill { get; set; }
    }
}