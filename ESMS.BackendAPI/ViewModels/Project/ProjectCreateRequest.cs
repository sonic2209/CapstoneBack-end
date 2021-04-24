using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class ProjectCreateRequest
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string DateBegin { get; set; }
        public string DateEstimatedEnd { get; set; }
        public int ProjectTypeID { get; set; }
        public int ProjectFieldID { get; set; }
    }
}