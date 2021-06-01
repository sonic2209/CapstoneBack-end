using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Position
{
    public class PositionUpdateRequestValidator : AbstractValidator<PositionUpdateRequest>
    {
        public PositionUpdateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name can not be empty");
            RuleFor(x => x.Name)
                .MinimumLength(3).WithMessage("Name must contain at least 3 characters")
                .MaximumLength(100).WithMessage("Name can not exceed 100 characters")
                .Matches(@"^(?:[^\W\d_]| )+$").WithMessage("Name can not contain digits or special characters")
                .When(x => !String.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Description).NotEmpty().WithMessage("Description can not be empty");
            RuleFor(x => x.Description)
                .MinimumLength(3).WithMessage("Description must contain at least 3 characters")
                .MaximumLength(1000).WithMessage("Description can not exceed 1000 characters")
                .When(x => !String.IsNullOrWhiteSpace(x.Description));
        }
    }
}