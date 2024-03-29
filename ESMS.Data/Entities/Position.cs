﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class Position
    {
        public int PosID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public List<RequiredPosition> RequiredPositions { get; set; }
        public List<MinPosInProject> MinPosInProjects { get; set; }
    }
}