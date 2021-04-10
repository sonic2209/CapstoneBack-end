using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class ProjectField
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Project> Projects { get; set; }
    }
}