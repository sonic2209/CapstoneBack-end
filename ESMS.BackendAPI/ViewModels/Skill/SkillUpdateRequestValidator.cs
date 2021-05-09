using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Skill
{
    public class SkillUpdateRequestValidator : AbstractValidator<SkillUpdateRequest>
    {
        public SkillUpdateRequestValidator()
        {
            RuleFor(x => x.SkillName).NotEmpty().WithMessage("Name cannot be empty")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");
        }
    }
}