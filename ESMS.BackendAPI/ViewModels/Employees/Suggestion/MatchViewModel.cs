﻿using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees.Suggestion
{
    public class MatchViewModel
    {
        public string EmpID { get; set; }
        public string EmpName { get; set; }
        public bool IsHighest { get; set; } = false;
        public double LanguageMatch { get; set; }
        public double SoftSkillMatch { get; set; }
        public double HardSkillMatch { get; set; }
        public double ProjectTypeMatch { get; set; }
        public double ProjectFieldMatch { get; set; }
        public double OverallMatch { get; set; }
    }
}
