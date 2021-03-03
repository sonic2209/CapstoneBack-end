using ESMS.BackendAPI.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class GetEmpInProjectPaging : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}