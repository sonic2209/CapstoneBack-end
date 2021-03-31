using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Data.Enums;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Skill;

namespace ESMS.BackendAPI.Services.Skills
{
    public class SkillService : ISkillService
    {
        private readonly ESMSDbContext _context;

        public SkillService(ESMSDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<bool>> ChangeStatus(int skillID)
        {
            var skill = _context.Skills.Find(skillID);
            if (skill == null) return new ApiErrorResult<bool>("Skill does not exist");
            if (skill.Status)
            {
                skill.Status = false;
            }
            else
            {
                skill.Status = true;
            }
            _context.Skills.Update(skill);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update skill failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<bool>> Create(SkillCreateRequest request)
        {
            var checkName = _context.Skills.Where(x => x.SkillName.Equals(request.SkillName))
                .Select(x => new Skill()).FirstOrDefault();
            if (checkName != null)
            {
                return new ApiErrorResult<bool>("This skill name is existed");
            }
            var skill = new Skill()
            {
                SkillName = request.SkillName,
                SkillType = (SkillType)request.SkillType
            };
            _context.Skills.Add(skill);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Create skill failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<SkillViewModel>> GetByID(int skillID)
        {
            var skill = await _context.Skills.FindAsync(skillID);
            if (skill == null) return new ApiErrorResult<SkillViewModel>("Skill does not exist");

            var skillViewModel = new SkillViewModel()
            {
                SkillID = skillID,
                SkillName = skill.SkillName,
                SkillType = skill.SkillType
            };

            return new ApiSuccessResult<SkillViewModel>(skillViewModel);
        }

        public async Task<ApiResult<List<ListSkillViewModel>>> GetSkill(string skillType)
        {
            SkillType st = (SkillType)Enum.Parse(typeof(SkillType), skillType);
            var query = from s in _context.Skills
                        select new { s };
            query = query.Where(x => x.s.SkillType == st);
            var data = await query.Select(x => new ListSkillViewModel()
            {
                SkillID = x.s.SkillID,
                SkillName = x.s.SkillName
            }).ToListAsync();

            return new ApiSuccessResult<List<ListSkillViewModel>>(data);
        }

        public async Task<ApiResult<PagedResult<SkillViewModel>>> GetSkillPaging(GetSkillPagingRequest request)
        {
            var query = from s in _context.Skills
                        select new { s };
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.s.SkillName.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new SkillViewModel()
                {
                    SkillID = x.s.SkillID,
                    SkillName = x.s.SkillName,
                    SkillType = x.s.SkillType,
                    Status = x.s.Status
                }).ToListAsync();

            var pagedResult = new PagedResult<SkillViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return new ApiSuccessResult<PagedResult<SkillViewModel>>(pagedResult);
        }

        public async Task<ApiResult<bool>> Update(int skillID, SkillUpdateRequest request)
        {
            var skill = _context.Skills.Find(skillID);
            if (skill == null) return new ApiErrorResult<bool>("Skill does not exist");
            if (!skill.SkillName.Equals(request.SkillName))
            {
                var checkName = _context.Skills.Where(x => x.SkillName.Equals(request.SkillName))
                .Select(x => new Skill()).FirstOrDefault();
                if (checkName != null)
                {
                    return new ApiErrorResult<bool>("This skill name is existed");
                }
                else
                {
                    skill.SkillName = request.SkillName;
                }
            }
            skill.SkillType = (SkillType)request.SkillType;
            _context.Skills.Update(skill);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update skill failed");
            }
            return new ApiSuccessResult<bool>();
        }
    }
}