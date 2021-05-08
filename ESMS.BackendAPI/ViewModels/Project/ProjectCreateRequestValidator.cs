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
            RuleFor(x => x.ProjectName).NotEmpty().WithMessage("Name cannot be empty")
                   .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be empty")
                   .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");
            RuleFor(x => x.DateBegin).NotEmpty().WithMessage("Date begin cannot be empty");
            RuleFor(x => x.DateEstimatedEnd).NotEmpty().WithMessage("Date estimated end cannot be empty");
        }
    }
}