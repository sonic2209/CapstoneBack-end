using ESMS.ViewModels.Common;
using ESMS.ViewModels.System.Employees;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESMS.Application.System.Employees
{
    public interface IEmployeeService
    {
        Task<ApiResult<string>> Authenticate(LoginRequest request);

        Task<ApiResult<bool>> Create(EmpCreateRequest request);

        Task<ApiResult<bool>> Update(string id, EmpUpdateRequest request);

        Task<ApiResult<PagedResult<EmpVm>>> GetEmpsPaging(GetEmpPagingRequest request);

        Task<ApiResult<EmpVm>> GetById(string id);

        Task<ApiResult<bool>> Delete(string id);
    }
}