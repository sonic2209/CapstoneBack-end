using ESMS.BackendAPI.ViewModels.Position;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class ProjectVM
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public int TypeID { get; set; }
        public int FieldID { get; set; }
        public string ProjectManagerID { get; set; }
        public List<RequiredPosVM> RequiredPositions { get; set; }
    }
}