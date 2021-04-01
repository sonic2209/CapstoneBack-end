using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Employees;
using ESMS.ViewModels.System.Employees;
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

        Task<List<CandidateViewModel>> SuggestCandidate(int projectID, SuggestCadidateRequest request);

        Task<ApiResult<PagedResult<EmpVm>>> GetEmpsPaging(GetEmpPagingRequest request);

        Task<ApiResult<EmpVm>> GetById(string id);

        Task<ApiResult<bool>> Delete(string id);

        Task<ApiResult<bool>> AddEmpPosition(string empID, AddEmpPositionRequest request);

        Task<ApiResult<EmpInfoViewModel>> GetEmpInfo(string empID);

        Task<ApiResult<bool>> UpdateEmpInfo(string empID, AddEmpPositionRequest request);
    }
}