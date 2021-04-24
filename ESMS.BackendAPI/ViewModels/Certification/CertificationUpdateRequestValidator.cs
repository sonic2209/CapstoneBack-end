﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Certification
{
    public class CertificationUpdateRequestValidator : AbstractValidator<CertificationUpdateRequest>
    {
        public CertificationUpdateRequestValidator()
        {
            RuleFor(x => x.CertificationName).NotEmpty().WithMessage("Name cannot be empty");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description cannot be empty");
        }
    }
}