using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees.Suggestion
{
    public class EmpInPosAndLang
    {
        public string EmpId { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }
        public PositionLevel NameExp { get; set; }
        public int LangLevel { get; set; }
    }
}
