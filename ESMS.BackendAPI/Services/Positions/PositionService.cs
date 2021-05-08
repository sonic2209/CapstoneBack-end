using ESMS.BackendAPI.Ultilities;
using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Position;
using ESMS.Data.EF;
using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Positions
{
    public class PositionService : IPositionService
    {
        private readonly ESMSDbContext _context;

        public PositionService(ESMSDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<bool>> ChangeStatus(int positionID)
        {
            var position = await _context.Positions.FindAsync(positionID);
            if (position == null) new ApiErrorResult<bool>("Position does not exist");
            if (position.Status)
            {
                var requiredPosition = await _context.RequiredPositions.Where(x => x.PositionID.Equals(position.PosID))
                    .Select(x => x.ID).ToListAsync();
                if (requiredPosition.Count() != 0)
                {
                    return new ApiErrorResult<bool>("This position is in project's requirement");
                }
                position.Status = false;
            }
            else
            {
                position.Status = true;
            }
            _context.Positions.Update(position);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update position failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<bool>> Create(PositionCreateRequest request)
        {
            UltilitiesService ultilities = new UltilitiesService();
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            var checkName = await _context.Positions.Where(x => x.Name.Equals(request.Name))
                .Select(x => new Position()).FirstOrDefaultAsync();
            if (checkName != null)
            {
                ultilities.AddOrUpdateError(errors, "Name", "This position name already exist");
                //return new ApiErrorResult<bool>("This position name already exist");
            }
            if (errors.Count() > 0)
            {
                return new ApiErrorResult<bool>(errors);
            }
            var position = new Position()
            {
                Name = request.Name,
                Description = request.Description,
            };
            _context.Positions.Add(position);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Create position failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<PositionViewModel>> GetByID(int positionID)
        {
            var position = await _context.Positions.FindAsync(positionID);
            if (position == null) return new ApiErrorResult<PositionViewModel>("Position does not exist");

            var positionViewModel = new PositionViewModel()
            {
                PosID = positionID,
                Name = position.Name,
                Description = position.Description,
                Status = position.Status
            };

            return new ApiSuccessResult<PositionViewModel>(positionViewModel);
        }

        public async Task<ApiResult<PagedResult<PositionViewModel>>> GetPositionPaging(GetPositionPagingRequest request)
        {
            var query = from p in _context.Positions
                        select new { p };
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.p.Name.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new PositionViewModel()
                {
                    PosID = x.p.PosID,
                    Name = x.p.Name,
                    Description = x.p.Description,
                    Status = x.p.Status
                }).ToListAsync();

            var pagedResult = new PagedResult<PositionViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return new ApiSuccessResult<PagedResult<PositionViewModel>>(pagedResult);
        }

        public async Task<ApiResult<List<ListPositionViewModel>>> GetPositions()
        {
            var data = await _context.Positions.Where(x => x.Status.Equals(true))
                .Select(x => new ListPositionViewModel()
                {
                    PosID = x.PosID,
                    Name = x.Name,
                }).ToListAsync();

            return new ApiSuccessResult<List<ListPositionViewModel>>(data);
        }

        public async Task<ApiResult<bool>> Update(int positionID, PositionUpdateRequest request)
        {
            UltilitiesService ultilities = new UltilitiesService();
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            var position = await _context.Positions.FindAsync(positionID);
            if (position == null) new ApiErrorResult<bool>("Position does not exist");
            if (!position.Name.Equals(request.Name))
            {
                var checkName = await _context.Positions.Where(x => x.Name.Equals(request.Name))
                 .Select(x => new Position()).FirstOrDefaultAsync();
                if (checkName != null)
                {
                    ultilities.AddOrUpdateError(errors, "Name", "This position name already exist");
                    //return new ApiErrorResult<bool>("This position name already exist");
                }
                if (errors.Count() > 0)
                {
                    return new ApiErrorResult<bool>(errors);
                }
                position.Name = request.Name;
            }
            position.Description = request.Description;

            _context.Positions.Update(position);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update position failed");
            }
            return new ApiSuccessResult<bool>();
        }
    }
}