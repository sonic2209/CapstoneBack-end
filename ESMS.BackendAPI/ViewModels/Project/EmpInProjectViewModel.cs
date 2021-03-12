using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class EmpInProjectViewModel
    {
        public string EmpID { get; set; }
        public string Name { get; set; }
        public int PosID { get; set; }
        public string PosName { get; set; }
        public EmployeeStatus Status { get; set; }
        public int ProjectID { get; set; }
    }
}