using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Employees
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email can not be empty");
            RuleFor(x => x.Email)
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").WithMessage("Email is not in the correct format")
                .When(x => !String.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password can not be empty");
            RuleFor(x => x.Password)
                .MinimumLength(6).WithMessage("Password needs to have at least 6 characters")
                .When(x => !String.IsNullOrWhiteSpace(x.Password)); 
        }
    }
}