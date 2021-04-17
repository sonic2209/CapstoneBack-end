using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class ConfirmCandidateDetail
    {
        public int RequiredPosID { get; set; }
        public int PosID { get; set; }
        public List<EmpDetail> EmpIDs { get; set; }
    }
}