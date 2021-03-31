﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Employees
{
    public class EmpUpdateRequest
    {
        public string Name { get; set; }
        public string IdentityNumber { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string RoleName { get; set; }
    }
}