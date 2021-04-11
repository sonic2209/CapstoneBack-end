using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Skill
{
    public class AddFieldRequest
    {
        public int FieldID { get; set; }
        public int SkillID { get; set; }
    }
}