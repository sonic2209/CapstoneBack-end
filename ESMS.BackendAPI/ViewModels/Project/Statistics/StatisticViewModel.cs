using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project.Statistics
{
    public class StatisticViewModel
    {
        public List<ProjectByType> ProjectByTypes { get; set; }
        public List<EmployeeByProject> EmployeeByProjects { get; set; }
        public List<EmployeeByPosition> EmployeeByPositions { get; set; }
        public List<EmployeeByHardSkill> EmployeeByHardSkills { get; set; }
    }
}