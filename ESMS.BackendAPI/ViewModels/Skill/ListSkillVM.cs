using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Skill
{
    public class ListSkillVM
    {
        public ListHardSkillVM HardSkill { get; set; }
        public ListSoftSkillVM SoftSkill { get; set; }
    }
}