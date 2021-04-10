using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class ProjectUpdateRequest
    {
        public string Description { get; set; }
        public string Skateholder { get; set; }
        public DateTime DateEstimatedEnd { get; set; }
        public int TypeID { get; set; }
        public int FieldID { get; set; }
    }
}