using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.Services.Project
{
    public class ProjectUpdateRequestValidator : AbstractValidator<ProjectUpdateRequest>
    {
        public ProjectUpdateRequestValidator()
        {
            RuleFor(x => x.ProjectName).NotEmpty().WithMessage("Project Name cannot be empty")
                   .MaximumLength(200).WithMessage("Project Name cannot exceed 200 characters");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be empty")
                   .MaximumLength(200).WithMessage("Description cannot exceed 200 characters");
            RuleFor(x => x.Skateholder).NotEmpty().WithMessage("Skateholder cannot be empty")
                   .MaximumLength(200).WithMessage("Skateholder cannot exceed 200 characters");
        }
    }
}