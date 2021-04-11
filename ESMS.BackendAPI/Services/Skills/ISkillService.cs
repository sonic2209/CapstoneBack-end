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

        public Task<ApiResult<bool>> ChangeStatus(int skillID);

        public Task<ApiResult<List<ListSkillViewModel>>> GetSkills(int skillType);

        public Task<ApiResult<List<ListSkillViewModel>>> GetHardSkills(int typeID, int posID);

        public Task<ApiResult<List<int>>> GetSoftSkills(int fieldID);

        public Task<ApiResult<PagedResult<SkillViewModel>>> GetSkillPaging(GetSkillPagingRequest request);

        public Task<ApiResult<SkillViewModel>> GetByID(int skillID);
    }
}