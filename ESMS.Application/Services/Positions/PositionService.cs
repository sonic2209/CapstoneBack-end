using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Utilities.Exceptions;
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

        public async Task<int> Create(PositionCreateRequest request)
        {
            var position = new Position()
            {
                Name = request.Name,
                Description = request.Description,
            };
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();
            return position.PosID;
        }

        public async Task<int> Delete(int positionID)
        {
            var position = await _context.Positions.FindAsync(positionID);
            if (position == null) throw new ESMSException($"Cannot find a projectID: {positionID}");

            _context.Positions.Remove(position);
            return await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<PositionViewModel>> GetEmpPositionPaging(string EmpID, GetPositionPagingRequest request)
        {
            var query = from p in _context.Positions
                        join ep in _context.EmpPositionInProjects on p.PosID equals ep.PosID
                        select new { p, ep };
            query = query.Where(x => x.ep.EmpID.Equals(EmpID));
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
                    Description = x.p.Description
                }).ToListAsync();

            var pagedResult = new PagedResult<PositionViewModel>()
            {
                TotalRecord = totalRow,
                Items = data
            };

            return pagedResult;
        }

        public async Task<PagedResult<PositionViewModel>> GetPositionPaging(GetPositionPagingRequest request)
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
                    Description = x.p.Description
                }).ToListAsync();

            var pagedResult = new PagedResult<PositionViewModel>()
            {
                TotalRecord = totalRow,
                Items = data
            };

            return pagedResult;
        }

        public async Task<int> Update(PositionUpdateRequest request)
        {
            var position = await _context.Positions.FindAsync(request.PosID);
            if (position == null) throw new ESMSException($"Cannot find a projectID: {request.PosID}");

            position.Name = request.Name;
            position.Description = request.Description;

            return await _context.SaveChangesAsync();
        }
    }
}