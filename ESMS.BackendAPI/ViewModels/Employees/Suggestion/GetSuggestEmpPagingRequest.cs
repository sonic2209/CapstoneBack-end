using ESMS.BackendAPI.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Employees.Suggestion
{
    public class GetSuggestEmpPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}
