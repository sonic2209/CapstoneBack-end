using ESMS.ViewModels.Common;
using ESMS.ViewModels.System.Employees;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESMS.Application.System
{
    public class EmployeeService : IEmployeeService
    {
        public Task<ApiResult<string>> Authenticate(LoginRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<bool>> Create(EmpCreateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<bool>> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<EmpVm>> GetById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<PagedResult<EmpVm>>> GetEmpsPaging(GetEmpPagingRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<bool>> Update(string id, EmpUpdateRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
