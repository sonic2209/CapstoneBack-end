using ESMS.Application.System.Skills;
using ESMS.ViewModels.System.Skill;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;

        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
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
        [HttpPut]
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

        //Delete:http://localhost/api/skill/id
        [HttpDelete("{skillID}")]
        public async Task<IActionResult> Delete(int skillID)
        {
            var result = await _skillService.Delete(skillID);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //Get:http://localhost/api/skill/HardSkill
        [HttpGet("{skillType}")]
        public async Task<IActionResult> GetSkill(string skillType)
        {
            var skills = await _skillService.GetSkill(skillType);
            return Ok(skills);
        }
    }
}