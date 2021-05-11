using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class ProjectViewModel
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public DateTime DateBegin { get; set; }
        public DateTime DateEstimatedEnd { get; set; }
        public DateTime? DateCreated { get; set; }
        public ProjectStatus Status { get; set; }
        public int? TypeID { get; set; }
        public string TypeName { get; set; }
        public int? FieldID { get; set; }
        public string FieldName { get; set; }
        public DateTime? DateEnd { get; set; }
        public string PmID { get; set; }
        public string PmName { get; set; }
        public int Noe { get; set; }
        public bool IsMissEmp { get; set; }
    }
}