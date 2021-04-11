using ESMS.BackendAPI.Services.Skills;
using ESMS.BackendAPI.ViewModels.Skill;
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
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        [HttpGet("{skillID}")]
        public async Task<IActionResult> GetByID(int skillID)
        {
            var skill = await _skillService.GetByID(skillID);
            return Ok(skill);
        }

        //http://localhost/api/certification/paging?pageIndex=1&pageSize=10&keyword=
        [HttpGet("paging")]
        public async Task<IActionResult> GetPositionPaging([FromQuery] GetSkillPagingRequest request)
        {
            var skills = await _skillService.GetSkillPaging(request);
            return Ok(skills);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SkillCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _skillService.Create(request);

            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //Put:http://localhost/api/skill/id
        [HttpPut("{skillID}")]
        public async Task<IActionResult> Update(int skillID, [FromBody] SkillUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _skillService.Update(skillID, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("changeStatus/{skillID}")]
        public async Task<IActionResult> ChangeStatus(int skillID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _skillService.ChangeStatus(skillID);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //Get:http://localhost/api/skill/type/typeID&&posID
        [HttpGet("type/{typeID}&&{posID}")]
        public async Task<IActionResult> GetHardSkills(int typeID, int posID)
        {
            var skills = await _skillService.GetHardSkills(typeID, posID);
            return Ok(skills);
        }

        //Get:http://localhost/api/skill/field/fieldID
        [HttpGet("field/{fieldID}")]
        public async Task<IActionResult> GetSoftSkills(int fieldID)
        {
            var skills = await _skillService.GetSoftSkills(fieldID);
            return Ok(skills);
        }
    }
}