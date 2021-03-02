using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.Services.Position
{
    public class RequiredPositionDetail
    {
        public int PosID { get; set; }
        public int NumberOfCandidates { get; set; }
        //list language
        public List<int> SoftSkillIDs { get; set; }
        public List<HardSkillDetail> HardSkills { get; set; }
    }
}