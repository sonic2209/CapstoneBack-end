using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Skill
{
    public class SkillDetailVM
    {
        public int SkillID { get; set; }
        public string SkillName { get; set; }
        public int SkillType { get; set; }
        public List<HardSkillOption> HardSkillOption { get; set; }
        public List<int> SoftSkillOption { get; set; }
    }
}