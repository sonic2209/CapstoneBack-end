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
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name can not be empty");            
            RuleFor(x => x.Name)
                .MinimumLength(3).WithMessage("Name must contain at least 3 characters")
                .MaximumLength(50).WithMessage("Name can not exceed 50 characters")
                .Matches(@"^(?:[^\W\d_]| )+$").WithMessage("Name can not contain digits or special characters")
                .When(x => !String.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email can not be empty");
            RuleFor(x => x.Email).MaximumLength(50).WithMessage("Email can not exceed 50 characters")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Email is not in the correct format").When(x => !String.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number can not be empty");
            RuleFor(x => x.PhoneNumber).Matches(@"(0[1-9])+([0-9]{8})\b").WithMessage("Phone number must contain 10 digits starting off with 0")
            .When(x => !String.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.IdentityNumber).NotEmpty().WithMessage("Identity number can not be empty");
            RuleFor(x => x.IdentityNumber).Matches("^[0-9]{10,12}$").WithMessage("Identity number format must be from 10 to 12 digits")
                .When(x => !String.IsNullOrWhiteSpace(x.IdentityNumber));

            RuleFor(x => x.Address).NotEmpty().WithMessage("Address can not be empty");
            RuleFor(x => x.Address).MaximumLength(100).WithMessage("Address can not exceed 100 characters")
            .MinimumLength(3).WithMessage("Address must contain at least 3 characters")
                .When(x => !String.IsNullOrWhiteSpace(x.Address));
        }
    }
}
