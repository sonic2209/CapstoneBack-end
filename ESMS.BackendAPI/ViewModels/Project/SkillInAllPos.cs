using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class SkillInAllPos
    {
        public int PosID { get; set; }
        public List<SkillInPos> SkillInPos { get; set; }
    }
}
