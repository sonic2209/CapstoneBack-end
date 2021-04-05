using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project.Statistics
{
    public class PosInProject
    {
        public string Name { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateEnd { get; set; }
        public List<EmployeeByPosition> EmployeeByPositions { get; set; }
        public List<EmployeeByPosLevel> EmployeeByPosLevels { get; set; }
    }
}