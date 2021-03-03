using ESMS.BackendAPI.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Employees
{
    public class GetEmpPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}