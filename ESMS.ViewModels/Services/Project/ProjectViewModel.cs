using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.Services.Project
{
    public class ProjectViewModel
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Skateholder { get; set; }
        public string DateBegin { get; set; }
        public string DateEstimatedEnd { get; set; }
        public ProjectStatus Status { get; set; }
    }
}