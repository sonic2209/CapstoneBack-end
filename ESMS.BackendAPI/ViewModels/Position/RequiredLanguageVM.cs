using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Position
{
    public class RequiredLanguageVM
    {
        public int LangID { get; set; }
        public string LangName { get; set; }
        public int Priority { get; set; }
    }
}