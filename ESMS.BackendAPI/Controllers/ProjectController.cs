using ESMS.Application.Projects;
using ESMS.ViewModels.Project;
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
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByID(int projectId)
        {
            var project = await _projectService.GetByID(projectId);
            if (project == null)
                return BadRequest("Cannot find project");
            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProjectCreateRequest request, string empID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var projectId = await _projectService.Create(request, empID);

            if (projectId == 0)
                return BadRequest();

            var project = await _projectService.GetByID(projectId);

            return CreatedAtAction(nameof(GetByID), new { id = projectId }, project);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] ProjectUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var affectedResult = await _projectService.Update(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("{projectId}")]
        public async Task<IActionResult> Delete(int projectId)
        {
            var affectedResult = await _projectService.Delete(projectId);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }
    }
}