using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class EmpPositionInProject
    {
        public int ProjectID { get; set; }
        public string EmpID { get; set; }
        public int PosID { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public Project Project { get; set; }
        public Employee Employee { get; set; }
        public Position Position { get; set; }
    }
}