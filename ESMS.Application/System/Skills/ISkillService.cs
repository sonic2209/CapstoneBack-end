using ESMS.ViewModels.Common;
using ESMS.ViewModels.System.Skill;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESMS.Application.System.Skills
{
    public interface ISkillService
    {
        public Task<ApiResult<bool>> Create(SkillCreateRequest request);

        public Task<ApiResult<bool>> Update(int skillID, SkillUpdateRequest request);

        public Task<ApiResult<bool>> Delete(int skillID);

        public Task<ApiResult<List<SkillViewModel>>> GetSkill(string skillType);
    }
}