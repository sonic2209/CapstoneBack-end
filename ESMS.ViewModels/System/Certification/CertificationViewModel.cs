﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.System.Certification
{
    public class CertificationViewModel
    {
        public int CertificationID { get; set; }
        public string CertificationName { get; set; }
        public string Description { get; set; }
        public int SkillID { get; set; }
    }
}