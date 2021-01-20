using ESMS.Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Entities
{
    public class Employee
    {
        public string EmpID { get; set; }
        public string Name { get; set; }
        public Role RoleID { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string IdentityNumber { get; set; }
        public DateTime? DateCreated { get; set; }
        public EmployeeStatus Status { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<Project> Projects { get; set; }
        public List<EmpPositionInProject> EmpPosInProjects { get; set; }
        public List<EmpSkill> EmpSkills { get; set; }
        public List<EmpCertification> EmpCertifications { get; set; }
    }
}