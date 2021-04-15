using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class ConfirmCandidateRequest
    {
        public List<ConfirmCandidateDetail> Candidates { get; set; }
    }
}