using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Common
{
    public class ErrorModel
    {
        public string Label { get; set; }
        public List<string> Messages { get; set; }
    }
}
