using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Skill;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Skills
{
    public interface ISkillService
    {
        public Task<ApiResult<bool>> Create(SkillCreateRequest request);

        public Task<ApiResult<bool>> Update(int skillID, SkillUpdateRequest request);

        public Task<ApiResult<bool>> Delete(int skillID);

        public Task<ApiResult<List<ListSkillViewModel>>> GetSkill(string skillType);

        public Task<ApiResult<PagedResult<ListSkillViewModel>>> GetSkillPaging(GetSkillPagingRequest request);

        public Task<ApiResult<SkillViewModel>> GetByID(int skillID);
    }
}