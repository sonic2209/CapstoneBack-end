using ESMS.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.Services.Position
{
    public class GetPositionPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}