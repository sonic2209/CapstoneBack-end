using ESMS.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.System.Certification
{
    public class GetCertificationPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}