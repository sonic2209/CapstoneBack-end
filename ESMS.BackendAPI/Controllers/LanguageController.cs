using ESMS.BackendAPI.Services.Languages;
using ESMS.BackendAPI.ViewModels.Language;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LanguageController : ControllerBase
    {
        private readonly ILanguageService _languageService;

        public LanguageController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpGet("getLanguages")]
        public async Task<IActionResult> GetLanguages()
        {
            var languages = await _languageService.GetLanguages();
            return Ok(languages);
        }
    }
}