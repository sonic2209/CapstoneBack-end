using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class EmployeeProjectViewModel
    {
        public string ProjectName { get; set; }
        public string PosName { get; set; }
        public DateTime? DateIn { get; set; }
    }
}