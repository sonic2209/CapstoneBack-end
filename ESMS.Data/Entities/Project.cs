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
        public ProjectStatus Status { get; set; }
        public string ProjectManagerID { get; set; }
        public int ProjectTypeID { get; set; }
        public int ProjectFieldID { get; set; }
        public Employee Employee { get; set; }
        public ProjectType ProjectType { get; set; }
        public ProjectField ProjectField { get; set; }
        public List<RequiredPosition> RequiredPositions { get; set; }
        public bool EmailStatus { get; set; }
    }
}