using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Position
{
    public class PositionCreateRequestValidator : AbstractValidator<PositionCreateRequest>
    {
        public PositionCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be empty")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");
        }
    }
}