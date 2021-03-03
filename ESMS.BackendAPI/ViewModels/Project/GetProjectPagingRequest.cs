using ESMS.BackendAPI.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class GetProjectPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}