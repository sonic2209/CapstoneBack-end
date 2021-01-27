using ESMS.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.System.Employees
{
    public class GetEmpPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}
