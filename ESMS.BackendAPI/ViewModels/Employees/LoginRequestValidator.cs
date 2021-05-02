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
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email cannot be empty")
                .Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            RuleFor(x => x.Password).NotEmpty().WithMessage("Password cannot be empty")
                .MinimumLength(6).WithMessage("Password needs to have at least 6 characters");
        }
    }
}