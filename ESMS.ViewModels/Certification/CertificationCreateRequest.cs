﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.Certification
{
    public class CertificationCreateRequest
    {
        public string CertificationName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}