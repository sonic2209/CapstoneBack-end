using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ESMS.ViewModels.System.Employees
{
    public class EmpUpdateRequest
    {

        [Display(Name = "Tên")]
        public string Name { get; set; }

        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; }
    }
}
