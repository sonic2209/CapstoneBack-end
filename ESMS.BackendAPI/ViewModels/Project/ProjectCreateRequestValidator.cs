using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class ProjectCreateRequestValidator : AbstractValidator<ProjectCreateRequest>
    {
        public ProjectCreateRequestValidator()
        {
            RuleFor(x => x.ProjectName).NotEmpty().WithMessage("Name can not be empty");
            RuleFor(x => x.ProjectName).MaximumLength(100).WithMessage("Name can not exceed 100 characters")
                .Matches(@"^(?:[^\W_]| )+$").WithMessage("Name can not contain special characters")
                .When(x => !String.IsNullOrWhiteSpace(x.ProjectName));

            RuleFor(x => x.Description).NotEmpty().WithMessage("Description can not be empty");
            RuleFor(x => x.Description).MaximumLength(1000).WithMessage("Description can not exceed 1000 characters");

            RuleFor(x => x.ProjectTypeID).NotEmpty().WithMessage("Please select type");

            RuleFor(x => x.ProjectFieldID).NotEmpty().WithMessage("Please select field");

            RuleFor(x => x.DateBegin).NotEmpty().WithMessage("Start date can not be empty");
            RuleFor(x => x.DateBegin).Must(CheckDate).WithMessage("Start date must be a valid date")
                .When(x => !String.IsNullOrWhiteSpace(x.DateBegin));

            RuleFor(x => x.DateEstimatedEnd).NotEmpty().WithMessage("Estimated end date can not be empty");
            RuleFor(x => x.DateEstimatedEnd).Must(CheckDate).WithMessage("Estimated end date must be a valid date")
                .When(x => !String.IsNullOrWhiteSpace(x.DateEstimatedEnd));
        }

        private bool CheckDate(string date)
        {
            return DateTime.TryParse(date, out DateTime d);
        }
    }
}