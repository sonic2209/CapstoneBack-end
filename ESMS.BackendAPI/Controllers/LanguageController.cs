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

        [HttpGet("{langID}")]
        public async Task<IActionResult> GetByID(int langID)
        {
            var language = await _languageService.GetByID(langID);
            return Ok(language);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LanguageCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _languageService.Create(request);

            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{langID}")]
        public async Task<IActionResult> Update(int langID, [FromBody] LanguageUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _languageService.Update(langID, request);

            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}