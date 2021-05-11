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
    }
}