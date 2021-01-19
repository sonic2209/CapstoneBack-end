using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.Position
{
    public class PositionUpdateRequest
    {
        public int PosID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}