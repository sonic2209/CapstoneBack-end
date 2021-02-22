using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.ViewModels.Common;
using ESMS.ViewModels.Services.Position;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.Application.Services.Positions
{
    public class PositionService : IPositionService
    {
        private readonly ESMSDbContext _context;

        public PositionService(ESMSDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<bool>> Create(PositionCreateRequest request)
        {
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

        public async Task<ApiResult<PagedResult<GetPositionPagingViewModel>>> GetPositionPaging(GetPositionPagingRequest request)
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
                .Select(x => new GetPositionPagingViewModel()
                {
                    PosID = x.p.PosID,
                    Name = x.p.Name,
                    Description = x.p.Description
                }).ToListAsync();

            var pagedResult = new PagedResult<GetPositionPagingViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return new ApiSuccessResult<PagedResult<GetPositionPagingViewModel>>(pagedResult);
        }

        public async Task<ApiResult<List<PositionViewModel>>> GetPositions()
        {
            var query = from p in _context.Positions
                        select new { p };
            var data = await query.Select(x => new PositionViewModel()
            {
                PosID = x.p.PosID,
                Name = x.p.Name,
            }).ToListAsync();

            return new ApiSuccessResult<List<PositionViewModel>>(data);
        }

        public async Task<ApiResult<bool>> Update(int positionID, PositionUpdateRequest request)
        {
            var position = await _context.Positions.FindAsync(positionID);
            if (position == null) new ApiErrorResult<bool>("Position does not exist");

            position.Name = request.Name;
            position.Description = request.Description;

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update position failed");
            }
            return new ApiSuccessResult<bool>();
        }
    }
}