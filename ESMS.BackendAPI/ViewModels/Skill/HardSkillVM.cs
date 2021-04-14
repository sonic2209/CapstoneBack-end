using ESMS.BackendAPI.ViewModels.Certification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Skill
{
    public class HardSkillVM
    {
        public int SkillID { get; set; }
        public string SkillName { get; set; }
        public List<ListCertificationViewModel> Certifications { get; set; }
    }
}