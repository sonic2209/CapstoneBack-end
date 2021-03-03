using ESMS.BackendAPI.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Certification
{
    public class GetCertificationPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}