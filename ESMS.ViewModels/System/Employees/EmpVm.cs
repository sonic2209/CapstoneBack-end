using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESMS.ViewModels.System.Employees
{
    public class EmpVm
    {
        public string Id { get; set; }

        [Display(Name = "Tên")]
        public string Name { get; set; }

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Tài khoản")]
        public string UserName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Ngày tạo")]
        public DateTime DateCreated { get; set; }
        public IList<string> Roles { get; set; }
    }
}
