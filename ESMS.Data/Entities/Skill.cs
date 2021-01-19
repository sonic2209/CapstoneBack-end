using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class Skill
    {
        public int SkillID { get; set; }
        public string SkillName { get; set; }
        public List<EmpSkill> EmpSkills { get; set; }
    }
}