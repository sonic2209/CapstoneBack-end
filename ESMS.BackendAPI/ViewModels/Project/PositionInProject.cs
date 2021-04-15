using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class PositionInProject
    {
        public int RequiredPosID { get; set; }
        public int PosID { get; set; }
        public string PosName { get; set; }
        public int CandidateNeeded { get; set; }
        public int Noe { get; set; }
        public List<EmpInProject> Employees { get; set; }
    }
}