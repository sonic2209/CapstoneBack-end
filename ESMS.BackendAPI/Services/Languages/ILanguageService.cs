using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Languages
{
    public interface ILanguageService
    {
        Task<ApiResult<LanguageViewModel>> GetByID(int langID);

        Task<ApiResult<bool>> Create(LanguageCreateRequest request);

        Task<ApiResult<bool>> Update(int langID, LanguageUpdateRequest request);

        Task<ApiResult<List<LanguageViewModel>>> GetLanguages();
    }
}