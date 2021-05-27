using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Skill
{
    public class ListHardSkillVM
    {
        public List<HardSkillVM> MinumumSkill { get; set; }
        public List<HardSkillVM> OptionalSkill { get; set; }
    }
}