using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.Skill
{
    public class SkillUpdateRequest
    {
        public int SkillID { get; set; }
        public string SkillName { get; set; }
        public int SkillType { get; set; }
    }
}