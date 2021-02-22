using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.System.Skill
{
    public class SkillUpdateRequest
    {
        public string SkillName { get; set; }
        public int SkillType { get; set; }
    }
}