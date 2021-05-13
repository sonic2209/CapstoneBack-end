using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Certification
{
    public class CertificationCreateRequestValidator : AbstractValidator<CertificationCreateRequest>
    {
        public CertificationCreateRequestValidator()
        {
            RuleFor(x => x.CertificationName).NotEmpty().WithMessage("Name can not be empty");
            RuleFor(x => x.CertificationName).MaximumLength(200).WithMessage("Name can not exceed 200 characters")
                .When(x => !String.IsNullOrWhiteSpace(x.CertificationName));

            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be empty");
            RuleFor(x => x.Description).MaximumLength(1000).WithMessage("Description can not exceed 1000 characters")
                .When(x => !String.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.SkillID).NotEmpty().WithMessage("Please select skill");

            RuleFor(x => x.CertiLevel).NotEmpty().WithMessage("Please select level");
        }
    }
}