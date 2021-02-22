using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class SkillInPosition
    {
        public int ID { get; set; }
        public int SkillID { get; set; }
        public int PositionID { get; set; }
        public Skill Skill { get; set; }
        public Position Position { get; set; }
    }
}