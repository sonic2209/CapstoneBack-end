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
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description can not be empty");
            RuleFor(x => x.Description).MaximumLength(1000).WithMessage("Description can not exceed 1000 characters")
                .When(x => !String.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.DateEstimatedEnd).NotEmpty().WithMessage("Please input estimated end date");
            RuleFor(x => x.DateEstimatedEnd).Must(CheckDate).WithMessage("Estimated end date must be a valid date")
                .When(x => !String.IsNullOrWhiteSpace(x.DateEstimatedEnd));
        }

        private bool CheckDate(string date)
        {
            return DateTime.TryParse(date, out DateTime d);
        }
    }
}