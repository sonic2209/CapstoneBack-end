﻿using FluentValidation;
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
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty");
            RuleFor(x => x.Name).MaximumLength(100).WithMessage("Name can not exceed 100 characters")
                .Matches(@"^(?:[^\W\d_]| )+$").WithMessage("Name can not contain digits or special characters")
                .When(x => !String.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be empty");
            RuleFor(x => x.Description).MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
                .Matches(@"^(?:[^\W\d_]| )+$").WithMessage("Description can not contain digits or special characters")
                .When(x => !String.IsNullOrWhiteSpace(x.Description));
        }
    }
}