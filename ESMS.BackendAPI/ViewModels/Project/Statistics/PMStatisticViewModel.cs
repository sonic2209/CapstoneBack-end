﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project.Statistics
{
    public class PMStatisticViewModel
    {
        public List<PosInProject> PosInProjects { get; set; }
        public List<PosLevelInProject> PosLevelInProjects { get; set; }
    }
}