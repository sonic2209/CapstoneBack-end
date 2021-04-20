using ESMS.BackendAPI.ViewModels.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees.Suggestion.SingleCandidate
{
    public class SingleCandidateViewModel
    {
        public ProjectVM ProjectInfo { get; set; }
        public List<SingleCandidateMatchInPos> MatchInEachPos { get; set; }
    }
}
