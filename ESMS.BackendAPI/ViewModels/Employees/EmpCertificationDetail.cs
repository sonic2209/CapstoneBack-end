using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees
{
    public class EmpCertificationDetail
    {
        public int CertiID { get; set; }
        public DateTime DateTaken { get; set; }
        public DateTime? DateEnd { get; set; }
    }
}