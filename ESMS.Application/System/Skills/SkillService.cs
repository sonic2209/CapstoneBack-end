using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Data.Enums;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using ESMS.ViewModels.System.Skill;
using ESMS.ViewModels.Common;

namespace ESMS.Application.System.Skills
{
    public class SkillService : ISkillService
    {
        private readonly ESMSDbContext _context;

        public SkillService(ESMSDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<bool>> Create(SkillCreateRequest request)
        {
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

        public async Task<ApiResult<bool>> Delete(int skillID)
        {
            var skill = _context.Skills.Find(skillID);
            if (skill == null) return new ApiErrorResult<bool>("Skill does not exist");
            _context.Skills.Remove(skill);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Delete skill failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<List<SkillViewModel>>> GetSkill(string skillType)
        {
            SkillType st = (SkillType)Enum.Parse(typeof(SkillType), skillType);
            var query = from s in _context.Skills
                        select new { s };
            query = query.Where(x => x.s.SkillType == st);
            var data = await query.Select(x => new SkillViewModel()
            {
                SkillID = x.s.SkillID,
                SkillName = x.s.SkillName
            }).ToListAsync();

            return new ApiSuccessResult<List<SkillViewModel>>(data);
        }

        public async Task<ApiResult<bool>> Update(int skillID, SkillUpdateRequest request)
        {
            var skill = _context.Skills.Find(skillID);
            if (skill == null) return new ApiErrorResult<bool>("Skill does not exist");
            skill.SkillName = request.SkillName;
            skill.SkillType = (SkillType)request.SkillType;
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update skill failed");
            }
            return new ApiSuccessResult<bool>();
        }
    }
}