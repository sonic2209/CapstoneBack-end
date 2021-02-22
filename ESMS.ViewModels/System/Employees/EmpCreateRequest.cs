using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESMS.ViewModels.System.Employees
{
    public class EmpCreateRequest
    {
        public string Name { get; set; }
        public DateTime DoB { get; set; }
        public string IdentityNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}