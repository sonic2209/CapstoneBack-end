using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class RequirementDetail
    {
        public int RequiredPosID { get; set; }
        public int CandidateNeeded { get; set; }
        public int MissingEmployee { get; set; }
        public List<EmpInProject> Employees { get; set; }
    }
}