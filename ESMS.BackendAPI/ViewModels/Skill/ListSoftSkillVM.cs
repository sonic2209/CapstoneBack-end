using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Skill
{
    public class ListSoftSkillVM
    {
        public List<ListSkillViewModel> MinumumSkill { get; set; }
        public List<ListSkillViewModel> OptionalSkill { get; set; }
    }
}