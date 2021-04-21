﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees
{
    public class LoadEmpInfoVM
    {
        public int PosID { get; set; }
        public int PosLevel { get; set; }
        public List<EmpLanguageDetail> Languages { get; set; }
        public List<int> SoftSkills { get; set; }
        public List<EmpHardSkillVM> HardSkills { get; set; }
    }
}