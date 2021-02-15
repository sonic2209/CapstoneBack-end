using ESMS.Application.Services.Projects;
using ESMS.ViewModels.Services.Project;
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
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        //Get:http://localhost/api/project/id
        [HttpGet("{projectID}")]
        public async Task<IActionResult> GetByID(int projectID)
        {
            var project = await _projectService.GetByID(projectID);
            return Ok(project);
        }

        //http://localhost/api/project/paging?pageIndex=1&pageSize=10&keyword=
        [HttpGet("paging")]
        public async Task<IActionResult> GetProjectPaging([FromQuery] GetProjectPagingRequest request)
        {
            var project = await _projectService.GetProjectPaging(request);
            return Ok(project);
        }

        //http://localhost/api/project/empID
        [HttpPost("{empID}")]
        public async Task<IActionResult> Create(string empID, [FromBody] ProjectCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var projectID = await _projectService.Create(empID, request);

            return Ok(projectID);
        }

        //Put:http://localhost/api/project/id
        [HttpPut("projectID")]
        public async Task<IActionResult> Update(int projectID, [FromBody] ProjectUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _projectService.Update(projectID, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //Delete:http://localhost/api/project/id
        [HttpDelete("{projectId}")]
        public async Task<IActionResult> Delete(int projectId)
        {
            var result = await _projectService.Delete(projectId);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}