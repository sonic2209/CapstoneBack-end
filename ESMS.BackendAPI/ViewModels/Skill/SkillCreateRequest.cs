using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Skill
{
    public class SkillCreateRequest
    {
        public string SkillName { get; set; }
        public int SkillType { get; set; }
    }
}