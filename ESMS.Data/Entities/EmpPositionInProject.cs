using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class EmpPositionInProject
    {
        public string EmpID { get; set; }
        public int? RequiredPositionID { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public bool IsAccept { get; set; }
        public string Note { get; set; }
        public Employee Employee { get; set; }
        public RequiredPosition RequiredPosition { get; set; }
    }
}