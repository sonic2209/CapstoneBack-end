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
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int NumberOfProject { get; set; }
        public DateTime? DateIn { get; set; }
        public EmployeeStatus Status { get; set; }
    }
}