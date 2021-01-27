using ESMS.ViewModels.Common;
using ESMS.ViewModels.Services.Position;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESMS.Application.Services.Positions
{
    public interface IPositionService
    {
        Task<int> Create(PositionCreateRequest request);

        Task<int> Update(PositionUpdateRequest request);

        Task<int> Delete(int positionID);

        Task<PagedResult<PositionViewModel>> GetPositionPaging(GetPositionPagingRequest request);

        Task<PagedResult<PositionViewModel>> GetEmpPositionPaging(string EmpID, GetPositionPagingRequest request);
    }
}