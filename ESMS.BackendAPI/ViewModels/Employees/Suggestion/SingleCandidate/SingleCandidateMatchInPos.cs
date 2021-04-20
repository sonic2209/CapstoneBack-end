using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees.Suggestion.SingleCandidate
{
    public class SingleCandidateMatchInPos
    {
        public string Position { get; set; }
        public int PosId { get; set; }
        public string EmpID { get; set; }
        public string EmpName { get; set; }
        public double LanguageMatch { get; set; }
        public double SoftSkillMatch { get; set; }
        public double HardSkillMatch { get; set; }
        public double ProjectTypeMatch { get; set; }
        public double ProjectFieldMatch { get; set; }
        public double OverallMatch { get; set; }
    }
}
