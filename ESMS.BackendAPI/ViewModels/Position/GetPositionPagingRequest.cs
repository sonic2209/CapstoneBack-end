using ESMS.BackendAPI.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Position
{
    public class GetPositionPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}