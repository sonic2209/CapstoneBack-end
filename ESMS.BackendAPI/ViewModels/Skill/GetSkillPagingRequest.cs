using ESMS.BackendAPI.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.BackendAPI.ViewModels.Skill
{
    public class GetSkillPagingRequest : PagingRequestBase
    {
        public string Keyword { get; set; }
    }
}