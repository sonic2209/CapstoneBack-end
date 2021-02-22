using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class EmpPosition
    {
        public int ID { get; set; }
        public string EmpID { get; set; }
        public int PosID { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime DateOut { get; set; }
        public NameExp NameExp { get; set; }
        public Employee Employee { get; set; }
        public Position Position { get; set; }
    }
}