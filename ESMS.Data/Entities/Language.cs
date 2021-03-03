using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class Language
    {
        public int LangID { get; set; }
        public string LangName { get; set; }
        public List<EmpLanguage> EmpLanguages { get; set; }
        public List<RequiredLanguage> RequiredLanguages { get; set; }
    }
}