using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Employees
{
    public class EmpCreateRequestValidator : AbstractValidator<EmpCreateRequest>
    {
        public EmpCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters").Matches(@"^(?:[^\W\d_]| )+$");//Matches(@"/^[a-zA-Z\s]*$/");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be empty")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Email format is not correct");
            int i = 0;
            //RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number cannot be empty").Length(10).Must(x=> int.TryParse(x,out i)).WithMessage("Phone number must be digits");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number cannot be empty").Matches(@"(0[1-9])+([0-9]{8})\b").WithMessage("Phone number format is not correct");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username cannot be empty");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password cannot be empty")
                .MinimumLength(6).WithMessage("Password needs to have at least 6 characters");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm Password cannot be empty");
            RuleFor(x => x.IdentityNumber).NotEmpty().WithMessage("Identity Number cannot be empty").Matches("^[0-9]+$");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address cannot be empty");

            RuleFor(x => x).Custom((request, context) =>
            {
                if (request.Password != request.ConfirmPassword)
                {
                    context.AddFailure(nameof(request.ConfirmPassword),"Confirm password must match with password");
                }
            });
            
        }
    }
}