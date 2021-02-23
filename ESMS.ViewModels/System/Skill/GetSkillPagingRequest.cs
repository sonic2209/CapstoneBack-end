using ESMS.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.ViewModels.System.Skill
{
    public class GetSkillPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}