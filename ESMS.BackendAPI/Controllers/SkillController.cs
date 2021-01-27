using ESMS.Application.Skills;
using ESMS.ViewModels.Skill;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Controllers
{
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
        public async Task<IActionResult> Create([FromForm] SkillCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var affectedResult = await _skillService.Create(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] SkillUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var affectedResult = await _skillService.Update(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("{skillId}")]
        public async Task<IActionResult> Delete(int skillId)
        {
            var affectedResult = await _skillService.Delete(skillId);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpGet("{skillType}")]
        public async Task<IActionResult> GetSkill(string skillType)
        {
            var skills = await _skillService.GetSkill(skillType);
            if (skills == null)
            {
                return BadRequest();
            }
            return Ok(skills);
        }
    }
}