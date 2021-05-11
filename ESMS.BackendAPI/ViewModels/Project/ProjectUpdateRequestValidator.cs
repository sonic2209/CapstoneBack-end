using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class ProjectUpdateRequestValidator : AbstractValidator<ProjectUpdateRequest>
    {
        public ProjectUpdateRequestValidator()
        {
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be empty");
            RuleFor(x => x.Description).MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
                .Matches(@"^(?:[^\W\d_]| )+$").WithMessage("Description can not contain digits or special characters")
                .When(x => !String.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.TypeID).NotEmpty().WithMessage("Please select type");

            RuleFor(x => x.FieldID).NotEmpty().WithMessage("Please select field");

            RuleFor(x => x.DateEstimatedEnd).NotEmpty().WithMessage("Estimated end date cannot be empty");
            RuleFor(x => x.DateEstimatedEnd).Must(CheckDate).WithMessage("Estimated end date must be a valid date")
                .When(x => !String.IsNullOrWhiteSpace(x.DateEstimatedEnd));
        }

        private bool CheckDate(string date)
        {
            return DateTime.TryParse(date, out DateTime d);
        }
    }
}