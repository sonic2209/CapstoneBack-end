using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Position;
using ESMS.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Positions
{
    public interface IPositionService
    {
        Task<ApiResult<bool>> Create(PositionCreateRequest request);

        Task<ApiResult<bool>> Update(int positionID, PositionUpdateRequest request);

        Task<ApiResult<bool>> ChangeStatus(int positionID);

        Task<ApiResult<List<ListPositionViewModel>>> GetPositions();

        Task<ApiResult<PagedResult<PositionViewModel>>> GetPositionPaging(GetPositionPagingRequest request);

        Task<ApiResult<PositionViewModel>> GetByID(int positionID);
    }
}