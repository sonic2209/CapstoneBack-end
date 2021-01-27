﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESMS.ViewModels.System.Employees
{
    public class EmpCreateRequest
    {
        [Display(Name = "Tên")]
        public string Name { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime DoB { get; set; }
        [Display(Name = "CMND")]
        public string IdentityNumber { get; set; }
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; }

        [Display(Name = "Mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}