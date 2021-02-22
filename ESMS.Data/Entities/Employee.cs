using ESMS.Data.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class Employee : IdentityUser<string>
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string IdentityNumber { get; set; }
        public DateTime? DateCreated { get; set; }
        public EmployeeStatus Status { get; set; }
        public DateTime? DateEnd { get; set; }
        public List<Project> Projects { get; set; }
        public List<EmpPositionInProject> EmpPosInProjects { get; set; }
        public List<EmpSkill> EmpSkills { get; set; }
        public List<EmpCertification> EmpCertifications { get; set; }
        public List<EmpPosition> EmpPositions { get; set; }
    }
}