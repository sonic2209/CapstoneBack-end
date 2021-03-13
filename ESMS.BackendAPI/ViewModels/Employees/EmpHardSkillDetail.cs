using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees
{
    public class EmpHardSkillDetail
    {
        public int SkillID { get; set; }
        public int? SkillLevel { get; set; }
        public int? exp { get; set; }
        public List<EmpCertificationDetail> EmpCertifications { get; set; }
    }
}