using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.Services.Project
{
    public class EmpInProjectViewModel
    {
        public string EmployeeID { get; set; }
        public string Name { get; set; }
        public string PosName { get; set; }
        public string Description { get; set; }
        public EmployeeStatus Status { get; set; }
    }
}