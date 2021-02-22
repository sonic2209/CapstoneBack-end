using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class EmpCertification
    {
        public int ID { get; set; }
        public string EmpID { get; set; }
        public int CertificationID { get; set; }
        public DateTime? DateTaken { get; set; }
        public DateTime? DateEnd { get; set; }
        public Employee Employee { get; set; }
        public Certification Certification { get; set; }
    }
}