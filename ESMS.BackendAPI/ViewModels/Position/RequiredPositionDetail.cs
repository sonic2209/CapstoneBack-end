using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Position
{
    public class RequiredPositionDetail
    {
        public int RequiredPosID { get; set; }
        public int PosID { get; set; }
        public int CandidateNeeded { get; set; }
        public List<LanguageDetail> Language { get; set; }
        public List<int> SoftSkillIDs { get; set; }
        public List<HardSkillDetail> HardSkills { get; set; }
    }
}