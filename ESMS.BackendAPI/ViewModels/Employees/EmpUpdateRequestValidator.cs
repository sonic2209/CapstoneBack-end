using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees
{
    public class EmpUpdateRequestValidator : AbstractValidator<EmpUpdateRequest>
    {
        public EmpUpdateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name can not be empty")
                .MaximumLength(50).WithMessage("Name can not exceed 50 characters").Matches(@"^(?:[^\W\d_]| )+$").WithMessage("Name can not contain digits or special characters");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email can not be empty").MaximumLength(50).WithMessage("Email can not exceed 50 characters")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Email is not in the correct format");

           
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number can not be empty").Matches(@"(0[1-9])+([0-9]{8})\b").WithMessage("Phone number must contain 10 digits starting off with 0");

            RuleFor(x => x.IdentityNumber).NotEmpty().WithMessage("Identity Number can not be empty").Matches("^[0-9]{10,12}$").WithMessage("Identity number format must be from 10 to 12 digits");
           
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address can not be empty").MaximumLength(100).WithMessage("Address can not exceed 100 characters");
        }
    }
}
