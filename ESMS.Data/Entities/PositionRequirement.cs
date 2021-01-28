using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class PositionRequirement
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public int PositionID { get; set; }
        public Project Project { get; set; }
        public Position Position { get; set; }
        public List<SkillRequirement> SkillRequirements { get; set; }
    }
}