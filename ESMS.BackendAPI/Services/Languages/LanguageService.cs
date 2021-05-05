using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Language;
using ESMS.Data.EF;
using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Languages
{
    public class LanguageService : ILanguageService
    {
        private readonly ESMSDbContext _context;

        public LanguageService(ESMSDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<bool>> Create(LanguageCreateRequest request)
        {
            var checkName = _context.Languages.Where(x => x.LangName.Equals(request.LangName))
                .Select(x => new Language()).FirstOrDefault();
            if (checkName != null)
            {
                return new ApiErrorResult<bool>("This language name already exist");
            }
            var language = new Language()
            {
                LangName = request.LangName
            };
            _context.Languages.Add(language);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Create language failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<LanguageViewModel>> GetByID(int langID)
        {
            var language = await _context.Languages.FindAsync(langID);
            if (language == null) return new ApiErrorResult<LanguageViewModel>("Language does not exist");

            var languageViewModel = new LanguageViewModel()
            {
                LangID = language.LangID,
                LangName = language.LangName
            };

            return new ApiSuccessResult<LanguageViewModel>(languageViewModel);
        }

        public async Task<ApiResult<List<LanguageViewModel>>> GetLanguages()
        {
            var query = from l in _context.Languages
                        select new { l };
            var data = await query.Select(x => new LanguageViewModel()
            {
                LangID = x.l.LangID,
                LangName = x.l.LangName
            }).ToListAsync();

            return new ApiSuccessResult<List<LanguageViewModel>>(data);
        }

        public async Task<ApiResult<bool>> Update(int langID, LanguageUpdateRequest request)
        {
            var language = await _context.Languages.FindAsync(langID);
            if (language == null) new ApiErrorResult<bool>("Language does not exist");

            language.LangName = request.LangName;

            _context.Languages.Update(language);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update language failed");
            }
            return new ApiSuccessResult<bool>();
        }
    }
}