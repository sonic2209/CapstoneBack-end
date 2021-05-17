using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Employees;
using ESMS.BackendAPI.ViewModels.Employees.Suggestion;
using ESMS.BackendAPI.ViewModels.Employees.Suggestion.SingleCandidate;
using ESMS.BackendAPI.ViewModels.Project;
using ESMS.ViewModels.System.Employees;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Employees
{
    public interface IEmployeeService
    {
        Task<ApiResult<LoginVm>> Authenticate(LoginRequest request);

        Task<ApiResult<string>> Create(EmpCreateRequest request);

        Task<ApiResult<bool>> Update(string id, EmpUpdateRequest request);

        Task<ApiResult<List<CandidateViewModel>>> SuggestCandidate(int projectID, SuggestCadidateRequest request);
        Task<ApiResult<List<CandidateViewModel>>> SuggestCandidateWithoutMinimumPoint(int projectID, SuggestCadidateRequest request);

        ApiResult<PagedResult<MatchViewModel>> SuggestCandidatePaging(List<MatchViewModel> listMatch, GetSuggestEmpPagingRequest request);
        Task<ApiResult<PagedResult<EmpVm>>> GetEmpsPaging(GetEmpPagingRequest request);

        Task<ApiResult<EmpVm>> GetById(string id);

        Task<ApiResult<bool>> Delete(string id);

        Task<ApiResult<bool>> AddEmpPosition(string empID, AddEmpPositionRequest request);

        Task<ApiResult<EmpInfoViewModel>> GetEmpInfo(string empID);

        Task<ApiResult<bool>> UpdateEmpInfo(string empID, AddEmpPositionRequest request);

        Task<ApiResult<LoadEmpInfoVM>> LoadEmpInfo(string empID);

        Task<ApiResult<List<ProjectVM>>> SingleCandidateSuggest(string empID);
        Task<ApiResult<bool>> ChangePassword(string id, ChangePasswordRequest request);
        FileModel GetFileById();
        Task<string> HandleFile(IFormFile file, string productId);

        Task<FileModel> ExportEmployeeInfo(string id);
        Task<ApiResult<bool>> ImportEmployeeInfo(IFormFile file);
    }
}