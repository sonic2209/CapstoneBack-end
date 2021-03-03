using ESMS.ViewModels.Services.Position;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.System.Employees
{
    public class SuggestCadidateRequest
    {
        public List<RequiredPositionDetail> RequiredPositions { get; set; }
    }
}