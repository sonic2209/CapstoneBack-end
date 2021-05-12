using FluentValidation;
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
            RuleFor(x => x.SkillName).NotEmpty().WithMessage("Name can not be empty");
            RuleFor(x => x.SkillName).MaximumLength(100).WithMessage("Name can not exceed 100 characters")
                .Matches(@"^(?:[^\d]| )+$").WithMessage("Name can not contain digits")
                .When(x => !String.IsNullOrWhiteSpace(x.SkillName));
            RuleFor(x => x.SkillType).InclusiveBetween(0, 1).WithMessage("Please select type(hard skill or soft skill)");
        }
    }
}