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
        Task<ApiResult<bool>> Create(PositionCreateRequest request);

        Task<ApiResult<bool>> Update(int positionID, PositionUpdateRequest request);

        Task<ApiResult<PagedResult<PositionViewModel>>> GetPositionPaging(GetPositionPagingRequest request);
    }
}