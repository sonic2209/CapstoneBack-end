using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.Services.Project
{
    public class ProjectUpdateRequest
    {
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Skateholder { get; set; }
    }
}