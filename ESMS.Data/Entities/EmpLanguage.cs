using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class EmpLanguage
    {
        public string EmpID { get; set; }
        public int LangID { get; set; }
        public int LangLevel { get; set; }
        public Language Language { get; set; }
        public Employee Employee { get; set; }
    }
}