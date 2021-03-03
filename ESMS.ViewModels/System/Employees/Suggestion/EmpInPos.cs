using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.System.Employees.Suggestion
{
    public class EmpInPos
    {
        public string EmpId { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }
        public NameExp NameExp { get; set; }
    }
}
