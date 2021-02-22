using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class Project
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEstimatedEnd { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Description { get; set; }
        public string Skateholder { get; set; }
        public ProjectStatus Status { get; set; }
        public string ProjectManagerID { get; set; }
        public Employee Employee { get; set; }
        public List<EmpPositionInProject> EmpPosInProjects { get; set; }
        public List<RequiredPosition> RequiredPositions { get; set; }
    }
}