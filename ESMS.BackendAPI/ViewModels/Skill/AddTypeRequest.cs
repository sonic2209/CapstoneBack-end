using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Skill
{
    public class AddTypeRequest
    {
        public int TypeID { get; set; }
        public int PosID { get; set; }
        public int SkillID { get; set; }
    }
}