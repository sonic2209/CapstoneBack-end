using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class AdminProjectViewModel
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Skateholder { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEstimatedEnd { get; set; }
        public ProjectStatus Status { get; set; }
        public string EmpID { get; set; }
        public string Name { get; set; }
        public int? ProjectTypeID { get; set; }
        public bool IsAddNewCandidate { get; set; }
    }
}