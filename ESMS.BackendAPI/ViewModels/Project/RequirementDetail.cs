using ESMS.BackendAPI.ViewModels.Position;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class RequirementDetail
    {
        public int RequiredPosID { get; set; }
        public int CandidateNeeded { get; set; }
        public int MissingEmployee { get; set; }
        public List<RequiredLanguageVM> Language { get; set; }
        public List<RequiredSoftSkillVM> SoftSkillIDs { get; set; }
        public List<RequiredHardSkillVM> HardSkills { get; set; }
        public List<EmpInProject> Employees { get; set; }
    }
}