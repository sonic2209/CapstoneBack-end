using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class RequiredLanguage
    {
        public int RequiredPositionID { get; set; }
        public int LangID { get; set; }
        public int Priority { get; set; }
        public Language Language { get; set; }
        public RequiredPosition RequiredPosition { get; set; }
    }
}