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
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name can not be empty");
                //Matches(@"/^[a-zA-Z\s]*$/");
            RuleFor(x => x.Name).MaximumLength(50).WithMessage("Name can not exceed 50 characters")
                .Matches(@"^(?:[^\W\d_]| )+$").WithMessage("Name can not contain digits or special characters")
                .When(x => !String.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Email).NotEmpty().WithMessage("Email can not be empty").MaximumLength(50).WithMessage("Email can not exceed 50 characters")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Email is not in the correct format");
            RuleFor(x => x.Email).MaximumLength(50).WithMessage("Email can not exceed 50 characters")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Email is not in the correct format").When(x => !String.IsNullOrWhiteSpace(x.Email));

            //int i = 0;
            //RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number cannot be empty").Length(10).Must(x=> int.TryParse(x,out i)).WithMessage("Phone number must be digits");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number can not be empty").Matches(@"(0[1-9])+([0-9]{8})\b").WithMessage("Phone number must contain 10 digits starting off with 0");
            RuleFor(x => x.PhoneNumber).Matches(@"(0[1-9])+([0-9]{8})\b").WithMessage("Phone number must contain 10 digits starting off with 0")
            .When(x => !String.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.UserName).NotEmpty().WithMessage("Username can not be empty").MaximumLength(20).WithMessage("Username can not exceeds 20 characters");
            RuleFor(x => x.UserName)
                .MaximumLength(20).WithMessage("Username can not exceeds 20 characters")
                .When(x => !String.IsNullOrWhiteSpace(x.UserName));
            //RuleFor(x => x.Password).NotEmpty().WithMessage("Password can not be empty")
            //    .MinimumLength(6).WithMessage("Password needs to have at least 6 characters");
            //RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Confirm Password cannot be empty");
            RuleFor(x => x.IdentityNumber).NotEmpty().WithMessage("Identity Number can not be empty");
            RuleFor(x => x.IdentityNumber).Matches("^[0-9]{10,12}$").WithMessage("Identity number format must be from 10 to 12 digits")
                .When(x => !String.IsNullOrWhiteSpace(x.IdentityNumber));

            RuleFor(x => x.Address).NotEmpty().WithMessage("Address can not be empty");
            RuleFor(x => x.Address).MaximumLength(100).WithMessage("Address can not exceed 100 characters")//.Matches(@"^(?=[^A-Za-z]*[A-Za-z])[\x00-\x7F]*$")
           .When(x => !String.IsNullOrWhiteSpace(x.Address));
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