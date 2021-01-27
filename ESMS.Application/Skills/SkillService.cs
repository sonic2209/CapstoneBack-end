using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Data.Enums;
using ESMS.ViewModels.Skill;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;

namespace ESMS.Application.Skills
{
    public class SkillService : ISkillService
    {
        private readonly ESMSDbContext _context;

        public SkillService(ESMSDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(SkillCreateRequest request)
        {
            var skill = new Skill()
            {
                SkillName = request.SkillName,
                SkillType = (SkillType)request.SkillType
            };
            _context.Skills.Add(skill);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Delete(int skillID)
        {
            var skill = _context.Skills.Find(skillID);

            _context.Skills.Remove(skill);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<SkillViewModel>> GetSkill(string skillType)
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

            return data;
        }

        public async Task<int> Update(SkillUpdateRequest request)
        {
            var skill = _context.Skills.Find(request.SkillID);

            skill.SkillName = request.SkillName;
            skill.SkillType = (SkillType)request.SkillType;
            return await _context.SaveChangesAsync();
        }
    }
}