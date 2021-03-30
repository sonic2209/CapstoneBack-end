using ESMS.BackendAPI.ViewModels.Position;
using System.Collections.Generic;

namespace ESMS.ViewModels.System.Employees
{
    public class SuggestCadidateRequest
    {
        public List<RequiredPositionDetail> RequiredPositions { get; set; }
        public int ProjectTypeID { get; set; }
    }
}