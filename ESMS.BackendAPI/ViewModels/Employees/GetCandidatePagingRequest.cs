using ESMS.ViewModels.Common;
using ESMS.ViewModels.Services.Position;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.System.Employees
{
    public class GetCandidatePagingRequest : PagingRequestBase
    {
        public List<CandidateViewModel> Candidates { get; set; }
    }
}