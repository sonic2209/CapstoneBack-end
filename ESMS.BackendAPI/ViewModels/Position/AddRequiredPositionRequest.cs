using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Position
{
    public class AddRequiredPositionRequest
    {
        public List<RequiredPositionDetail> RequiredPositions { get; set; }
    }
}