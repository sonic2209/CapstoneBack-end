﻿using ESMS.BackendAPI.Services.Projects;
using ESMS.BackendAPI.ViewModels.Position;
using ESMS.BackendAPI.ViewModels.Project;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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

        //Get:http://localhost/api/project/getProjects/id
        [HttpGet("getProjects/{empID}")]
        public async Task<IActionResult> GetProjectByEmpID(string empID, [FromQuery] GetProjectPagingRequest request)
        {
            var project = await _projectService.GetProjectByEmpID(empID, request);
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
        [HttpDelete("{projectID}")]
        public async Task<IActionResult> Delete(int projectID)
        {
            var result = await _projectService.Delete(projectID);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("addRequirements/{projectID}")]
        public async Task<IActionResult> AddRequiredPosition(int projectID, [FromBody] AddRequiredPositionRequest request)
        {
            var result = await _projectService.AddRequiredPosition(projectID, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}