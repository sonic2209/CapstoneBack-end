using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Skill
{
    public class GetSkillRequest
    {
        public int PosID { get; set; }
        public int TypeID { get; set; }
        public int FieldID { get; set; }
        public List<int> HardSkill { get; set; }
        public List<int> SoftSkill { get; set; }
    }
}