using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Position;
using ESMS.BackendAPI.ViewModels.Project;
using ESMS.BackendAPI.ViewModels.Project.Statistics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Projects
{
    public interface IProjectService
    {
        Task<ApiResult<int>> Create(string empID, ProjectCreateRequest request);

        Task<ApiResult<bool>> Update(int projectID, ProjectUpdateRequest request);

        Task<ApiResult<int>> ChangeStatus(int projectID);

        Task<ApiResult<bool>> Delete(int projectID);

        Task<ApiResult<ProjectViewModel>> GetByID(int projectID);

        Task<ApiResult<PagedResult<AdminProjectViewModel>>> GetProjectPaging(GetProjectPagingRequest request);

        Task<ApiResult<ListProjectViewModel>> GetProjectByEmpID(string empID, GetProjectPagingRequest request);

        Task<ApiResult<PagedResult<EmployeeProjectViewModel>>> GetEmployeeProjects(string empID, GetProjectPagingRequest request);

        Task<ApiResult<List<PositionInProject>>> GetEmpInProjectPaging(int projectID);

        Task<ApiResult<List<CandidateInProject>>> GetCandidates(int projectID);

        Task<ApiResult<List<RequiredPositionDetail>>> AddRequiredPosition(int projectID, AddRequiredPositionRequest request);

        Task<ApiResult<List<RequiredPositionVM>>> GetRequiredPositions(int projectID);

        Task<ApiResult<RequiredPositionVM>> GetRequiredPosByID(int projectID, int posID);

        Task<ApiResult<bool>> AddCandidate(int projectID, AddCandidateRequest request);

        Task<ApiResult<List<string>>> ConfirmCandidate(int projectID, ConfirmCandidateRequest request);

        Task<ApiResult<List<ProjectTypeViewModel>>> GetProjectTypes();

        Task<ApiResult<List<ProjectFieldViewModel>>> GetProjectFields();

        Task<ApiResult<string>> CheckStatus(AddRequiredPositionRequest request);

        Task<ApiResult<StatisticViewModel>> GetStatistics();

        Task<ApiResult<List<PosInProject>>> GetStatisticsByEmpID(string empID);

        Task<ApiResult<bool>> CheckProject();

        List<ProjectVM> GetMissEmpProjects(string empID);

        Task<List<DeletedProject>> CheckNoEmpProject();

        Task<ApiResult<List<MissEmpPosition>>> GetMissEmpPos();

        Task<ApiResult<List<SkillInPos>>> GetSkillInPos(int posID);

        Task<ApiResult<List<SkillInAllPos>>> GetSkillInAllPos();

        Task<ApiResult<List<EmpInProject>>> GetEmpByRequiredID(int requiredID);

        Task<ApiResult<List<string>>> CheckCandidate(int projectID, AddCandidateRequest request);
    }
}