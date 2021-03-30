using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Skill
{
    public class SkillViewModel
    {
        public int SkillID { get; set; }
        public string SkillName { get; set; }
        public SkillType SkillType { get; set; }
        public bool Status { get; set; }
    }
}