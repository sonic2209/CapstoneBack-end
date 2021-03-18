using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Position
{
    public class RequiredPositionDetail
    {
        public int PosID { get; set; }
        public List<int> PosLevel { get; set; }
        public List<LanguageDetail> Language { get; set; }
        public List<int> SoftSkillIDs { get; set; }
        public List<HardSkillDetail> HardSkills { get; set; }
    }
}