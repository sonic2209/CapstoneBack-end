using ESMS.BackendAPI.ViewModels.Employees;
using ESMS.BackendAPI.ViewModels.Employees.Suggestion;
using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.System.Employees
{
    public class CandidateViewModel
    {
        public string Position { get; set; }
        public List<MatchViewModel> MatchDetail { get; set; }
    }
}