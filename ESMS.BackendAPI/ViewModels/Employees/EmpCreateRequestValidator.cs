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
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name can not be empty")
                .MaximumLength(50).WithMessage("Name can not exceed 50 characters").Matches(@"^(?:[^\W\d_]| )+$").WithMessage("Name can not contain digits or special characters");//Matches(@"/^[a-zA-Z\s]*$/");

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email can not be empty").MaximumLength(50).WithMessage("Email can not exceed 50 characters")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Email is not in the correct format");
            int i = 0;
            //RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number cannot be empty").Length(10).Must(x=> int.TryParse(x,out i)).WithMessage("Phone number must be digits");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number can not be empty").Matches(@"(0[1-9])+([0-9]{8})\b").WithMessage("Phone number must contain 10 digits starting off with 0");
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username can not be empty").MaximumLength(20).WithMessage("Username can not exceeds 20 characters");
            //RuleFor(x => x.Password).NotEmpty().WithMessage("Password can not be empty")
            //    .MinimumLength(6).WithMessage("Password needs to have at least 6 characters");
            //RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm Password cannot be empty");
            RuleFor(x => x.IdentityNumber).NotEmpty().WithMessage("Identity Number can not be empty").Matches("^[0-9]{10,12}$").WithMessage("Identity number format must be from 10 to 12 digits");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address can not be empty").MaximumLength(100).WithMessage("Address can not exceed 100 characters");

            //RuleFor(x => x).Custom((request, context) =>
            //{
            //    if (request.password != request.confirmpassword)
            //    {
            //        context.addfailure(nameof(request.confirmpassword), "confirm password must match with password");
            //    }
            //});

        }
    }
}