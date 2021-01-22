using ESMS.Data.Entities;
using ESMS.ViewModels.Common;
using ESMS.ViewModels.Services.Project;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESMS.Application.Services.Projects
{
    public interface IProjectService
    {
        Task<int> Create(ProjectCreateRequest request, string empID);

        Task<int> Update(ProjectUpdateRequest request);

        Task<bool> UpdateStatus(int projectID, int status);

        Task<int> Delete(int projectID);

        Task<ProjectViewModel> GetByID(int projectID);

        Task<PagedResult<ProjectViewModel>> GetProjectPaging(GetProjectPagingRequest request);

        Task<PagedResult<EmpInProjectViewModel>> GetEmpInProjectPaging(int projectID, GetEmpInProjectPaging request);
    }
}