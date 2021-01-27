using ESMS.ViewModels.Skill;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESMS.Application.Skills
{
    public interface ISkillService
    {
        public Task<int> Create(SkillCreateRequest request);

        public Task<int> Update(SkillUpdateRequest request);

        public Task<int> Delete(int skillID);

        public Task<List<SkillViewModel>> GetSkill(string skillType);
    }
}