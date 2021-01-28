using ESMS.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.Services.Project
{
    public class GetEmpInProjectPaging : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}