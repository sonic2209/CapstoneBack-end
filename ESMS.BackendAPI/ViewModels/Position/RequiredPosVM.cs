using ESMS.BackendAPI.ViewModels.Employees.Suggestion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Position
{
    public class RequiredPosVM
    {
        public int RequiredPosID { get; set; }
        public int PosID { get; set; }
        public string PosName { get; set; }
        public int CandidateNeeded { get; set; }
        public int MissingEmployee { get; set; }
        public List<LanguageDetail> Language { get; set; }
        public List<int> SoftSkillIDs { get; set; }
        public List<HardSkillDetail> HardSkills { get; set; }
        public MatchViewModel MatchDetail { get; set; }
    }
}