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
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be empty")
                   .MaximumLength(200).WithMessage("Description cannot exceed 200 characters");
            RuleFor(x => x.DateEstimatedEnd).NotEmpty().WithMessage("Date estimated end cannot be empty");
        }
    }
}