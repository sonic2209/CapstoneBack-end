using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project.Statistics
{
    public class PosLevelInProject
    {
        public string Name { get; set; }
        public List<EmployeeByPosLevel> EmployeeByPositionLevels { get; set; }
    }
}