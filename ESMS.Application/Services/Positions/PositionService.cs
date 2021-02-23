using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Data.Enums;
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

        public async Task<ApiResult<bool>> AddRequiredPosition(int projectID, AddRequiredPositionRequest request)
        {
            foreach (var position in request.RequiredPositions)
            {
                var requiredPosition = new RequiredPosition()
                {
                    NumberOfCandidates = position.NumberOfCandidates,
                    PositionID = position.PosID,
                    ProjectID = projectID
                };
                _context.RequiredPositions.Add(requiredPosition);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ApiErrorResult<bool>("Create requiredPosition failed");
                }
                RequiredSkill requiredSkill;
                foreach (var softSkill in position.SoftSkillIDs)
                {
                    requiredSkill = new RequiredSkill()
                    {
                        RequiredPositionID = requiredPosition.ID,
                        SkillID = softSkill
                    };
                    _context.RequiredSkills.Add(requiredSkill);
                }
                foreach (var hardSkill in position.HardSkills)
                {
                    requiredSkill = new RequiredSkill()
                    {
                        RequiredPositionID = requiredPosition.ID,
                        SkillID = hardSkill.HardSkillID,
                        Priority = hardSkill.Priority,
                        SkillLevel = (SkillLevel)hardSkill.SkillLevel
                    };
                    _context.RequiredSkills.Add(requiredSkill);
                }
                result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ApiErrorResult<bool>("Create requiredSkill failed");
                }
            }
            return new ApiSuccessResult<bool>();
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

        public async Task<ApiResult<PositionViewModel>> GetByID(int positionID)
        {
            var position = await _context.Positions.FindAsync(positionID);
            if (position == null) return new ApiErrorResult<PositionViewModel>("Position does not exist");

            var positionViewModel = new PositionViewModel()
            {
                PosID = positionID,
                Name = position.Name,
                Description = position.Description
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
                    Description = x.p.Description
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
            var query = from p in _context.Positions
                        select new { p };
            var data = await query.Select(x => new ListPositionViewModel()
            {
                PosID = x.p.PosID,
                Name = x.p.Name,
            }).ToListAsync();

            return new ApiSuccessResult<List<ListPositionViewModel>>(data);
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