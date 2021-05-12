using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class EmployeeProjectViewModel
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEstimatedEnd { get; set; }
        public DateTime? DateEnd { get; set; }
        public string PosName { get; set; }
        public DateTime? DateIn { get; set; }
    }
}