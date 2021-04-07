using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Certification
{
    public class ListCertificationViewModel
    {
        public int CertificationID { get; set; }
        public string CertificationName { get; set; }
        public int CertiLevel { get; set; }
    }
}