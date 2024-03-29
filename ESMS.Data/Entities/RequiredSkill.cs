﻿using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class RequiredSkill
    {
        public int SkillID { get; set; }
        public int RequiredPositionID { get; set; }
        public int? Priority { get; set; }
        public EnumSkillLevel? SkillLevel { get; set; }
        public int? CertificationLevel { get; set; }
        public Skill Skill { get; set; }
        public RequiredPosition RequiredPosition { get; set; }
    }
}