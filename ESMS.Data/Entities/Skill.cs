using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class Skill
    {
        public int SkillID { get; set; }
        public string SkillName { get; set; }
        public SkillType SkillType { get; set; }
        public List<Certification> Certifications { get; set; }
        public List<EmpSkill> EmpSkills { get; set; }
        public List<RequiredSkill> RequiredSkills { get; set; }
        public List<SkillInPosition> SkillInPositions { get; set; }
    }
}