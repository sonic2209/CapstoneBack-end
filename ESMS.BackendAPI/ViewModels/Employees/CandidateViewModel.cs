using ESMS.BackendAPI.ViewModels.Employees;
using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.System.Employees
{
    public class CandidateViewModel
    {
        public string EmpID { get; set; }
        public double Match { get; set; }
    }
}