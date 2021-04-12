using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Position
{
    public class RequiredPositionVM
    {
        public int RequiredPosID { get; set; }
        public int PosID { get; set; }
        public string PosName { get; set; }
        public int CandidateNeeded { get; set; }
        public int MissingEmployee { get; set; }
        public RequirementStatus Status { get; set; }
        public DateTime DateCreated { get; set; }
        public List<RequiredLanguageVM> Language { get; set; }
        public List<RequiredSoftSkillVM> SoftSkillIDs { get; set; }
        public List<RequiredHardSkillVM> HardSkills { get; set; }
    }
}