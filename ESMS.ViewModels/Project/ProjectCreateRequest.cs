﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.Project
{
    public class ProjectCreateRequest
    {
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Skateholder { get; set; }
    }
}