using ESMS.BackendAPI.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.ViewModels.Project
{
    public class ListProjectViewModel
    {
        public bool IsCreateNew { get; set; }
        public PagedResult<ProjectViewModel> data { get; set; }
    }
}