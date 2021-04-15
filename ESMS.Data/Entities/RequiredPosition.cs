using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class RequiredPosition
    {
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public int PositionID { get; set; }
        public int CandidateNeeded { get; set; }
        public int MissingEmployee { get; set; }
        public RequirementStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public Project Project { get; set; }
        public Position Position { get; set; }
        public List<RequiredSkill> RequiredSkills { get; set; }
        public List<RequiredLanguage> RequiredLanguages { get; set; }
        public List<RejectedEmployee> RejectedEmployees { get; set; }
        public List<EmpPositionInProject> EmpPositionInProjects { get; set; }
    }
}