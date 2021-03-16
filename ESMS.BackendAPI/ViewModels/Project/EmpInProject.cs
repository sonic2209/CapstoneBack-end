using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class EmpInProject
    {
        public string EmpID { get; set; }
        public string Name { get; set; }
        public DateTime? DateIn { get; set; }
        public EmployeeStatus Status { get; set; }
    }
}