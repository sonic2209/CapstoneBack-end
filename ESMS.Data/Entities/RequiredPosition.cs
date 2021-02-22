using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class RequiredPosition
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public int PositionID { get; set; }
        public int NumberOfCandidates { get; set; }
        public Project Project { get; set; }
        public Position Position { get; set; }
        public List<RequiredSkill> RequiredSkills { get; set; }
    }
}