using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class MinPosInProject
    {
        public int TypeID { get; set; }
        public int PosID { get; set; }
        public ProjectType ProjectType { get; set; }
        public Position Position { get; set; }
    }
}