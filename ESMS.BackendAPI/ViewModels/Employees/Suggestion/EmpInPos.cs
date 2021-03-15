using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees.Suggestion
{
    public class EmpInPos
    {
        public string EmpId { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }
        public string EmpName { get; set; }
        public String Position { get; set; }
        public NameExp NameExp { get; set; }
    }
}
