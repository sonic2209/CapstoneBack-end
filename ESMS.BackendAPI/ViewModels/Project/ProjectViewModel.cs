﻿using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class ProjectViewModel
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Skateholder { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEstimatedEnd { get; set; }
        public ProjectStatus Status { get; set; }
        public int? TypeID { get; set; }
        public string TypeName { get; set; }
    }
}