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
            RuleFor(x => x.SkillName).NotEmpty().WithMessage("Name cannot be empty");
            RuleFor(x => x.SkillName).MaximumLength(100).WithMessage("Name can not exceed 100 characters")
                .Matches(@"^(?:[^\W\d_]| )+$").WithMessage("Name can not contain digits or special characters")
                .When(x => !String.IsNullOrWhiteSpace(x.SkillName));
        }
    }
}