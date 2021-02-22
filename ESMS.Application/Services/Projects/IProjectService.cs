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
        Task<ApiResult<int>> Create(string EmpID, ProjectCreateRequest request);

        Task<ApiResult<bool>> Update(int projectID, ProjectUpdateRequest request);

        Task<ApiResult<bool>> UpdateStatus(int projectID, int status);

        Task<ApiResult<bool>> Delete(int projectID);

        Task<ApiResult<ProjectViewModel>> GetByID(int projectID);

        Task<ApiResult<PagedResult<ProjectViewModel>>> GetProjectPaging(GetProjectPagingRequest request);

        Task<ApiResult<PagedResult<EmpInProjectViewModel>>> GetEmpInProjectPaging(int projectID, GetEmpInProjectPaging request);
    }
}