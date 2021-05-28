using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.NewPassword).Matches("^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).{6,8}$").WithMessage("Password must be between 6-8 characters and contain at least one uppercase letter, one lowercase letter and one digit");
            

            RuleFor(x => x).Custom((request, context) =>
            {
                if (request.NewPassword != request.ConfirmPassword)
                {
                    context.AddFailure(nameof(request.ConfirmPassword), "Confirm password must match with your new password");
                }
            });
        }
    }
}
