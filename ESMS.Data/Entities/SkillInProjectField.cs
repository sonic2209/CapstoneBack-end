using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class SkillInProjectField
    {
        public int FieldID { get; set; }
        public int SkillID { get; set; }
        public ProjectField ProjectField { get; set; }
        public Skill Skill { get; set; }
    }
}