﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Skill
{
    public class SkillCreateRequestValidator : AbstractValidator<SkillCreateRequest>
    {
        public SkillCreateRequestValidator()
        {
            RuleFor(x => x.SkillName).NotEmpty().WithMessage("Name cannot be empty")
                .MaximumLength(200).WithMessage("Name cannot exceed 200 characters");
        }
    }
}