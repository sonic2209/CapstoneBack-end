using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.Services.Project
{
    public class ProjectCreateRequest
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Skateholder { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEstimatedEnd { get; set; }
    }
}