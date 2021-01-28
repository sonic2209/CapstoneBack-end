using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class SkillRequirement
    {
        public int ID { get; set; }
        public int SkillID { get; set; }
        public int PositionRequirementID { get; set; }
        public Skill Skill { get; set; }
        public PositionRequirement PositionRequirement { get; set; }
    }
}