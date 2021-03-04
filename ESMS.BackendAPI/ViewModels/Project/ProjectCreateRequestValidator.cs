﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class ProjectCreateRequestValidator : AbstractValidator<ProjectCreateRequest>
    {
        public ProjectCreateRequestValidator()
        {
            RuleFor(x => x.ProjectName).NotEmpty().WithMessage("Project Name cannot be empty")
                   .MaximumLength(200).WithMessage("Project Name cannot exceed 200 characters");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be empty")
                   .MaximumLength(200).WithMessage("Description cannot exceed 200 characters");
            RuleFor(x => x.Skateholder).NotEmpty().WithMessage("Skateholder cannot be empty")
                   .MaximumLength(200).WithMessage("Skateholder cannot exceed 200 characters");
            RuleFor(x => x.DateBegin).NotEmpty().WithMessage("Date begin cannot be empty");
            RuleFor(x => x.DateEstimatedEnd).NotEmpty().WithMessage("Date estimated end cannot be empty");
        }
    }
}