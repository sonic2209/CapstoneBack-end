using ESMS.BackendAPI.ViewModels.Certification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees
{
    public class EmpHardSkillVM
    {
        public int SkillID { get; set; }
        public int? SkillLevel { get; set; }
        public List<EmpCertificationDetail> EmpCertifications { get; set; }
        public List<ListCertificationViewModel> CertiList { get; set; }
    }
}