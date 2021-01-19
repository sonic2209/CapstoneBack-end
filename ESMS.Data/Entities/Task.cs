using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class Task
    {
        public int TaskID { get; set; }
        public int TeamID { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateEnd { get; set; }
        public Team Team { get; set; }
    }
}