using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees
{
    public class CertiInfo
    {
        public int CertiID { get; set; }
        public string CertiName { get; set; }
        public DateTime DateTaken { get; set; }
        public DateTime? DateEnd { get; set; }
        public int CertiLevel { get; set; }
    }
}