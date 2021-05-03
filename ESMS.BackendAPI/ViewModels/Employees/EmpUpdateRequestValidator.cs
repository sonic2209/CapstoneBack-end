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
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters").Matches(@"^(?:[^\W\d_]| )+$");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be empty")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Email format is not correct");           
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number cannot be empty").Matches(@"(0[1-9])+([0-9]{8})\b").WithMessage("Phone number format is not correct");
            RuleFor(x => x.IdentityNumber).NotEmpty().WithMessage("Identity Number cannot be empty").Matches("^[0-9]+$");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address cannot be empty");
        }
    }
}
