using ESMS.BackendAPI.Services.Projects;
using ESMS.BackendAPI.ViewModels.Position;
using ESMS.BackendAPI.ViewModels.Project;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("getEmployeeProjects/{empID}")]
        public async Task<IActionResult> GetEmployeeProjects(string empID, [FromQuery] GetProjectPagingRequest request)
        {
            var project = await _projectService.GetEmployeeProjects(empID, request);
            return Ok(project);
        }

        //Get:http://localhost/api/project/getProjects/id
        [HttpGet("getEmpsInProject/{projectID}")]
        public async Task<IActionResult> GetEmpInProjectPaging(int projectID)
        {
            var employees = await _projectService.GetEmpInProjectPaging(projectID);
            return Ok(employees);
        }

        [HttpGet("getCandidates/{projectID}")]
        public async Task<IActionResult> GetCandidates(int projectID)
        {
            var candidates = await _projectService.GetCandidates(projectID);
            return Ok(candidates);
        }

        //http://localhost/api/project/paging?pageIndex=1&pageSize=10&keyword=
        [HttpGet("paging")]
        public async Task<IActionResult> GetProjectPaging([FromQuery] GetProjectPagingRequest request)
        {
            var project = await _projectService.GetProjectPaging(request);
            return Ok(project);
        }

        [HttpGet("getProjectTypes")]
        public async Task<IActionResult> GetProjectTypes()
        {
            var projectTypes = await _projectService.GetProjectTypes();
            return Ok(projectTypes);
        }

        [HttpGet("getProjectFields")]
        public async Task<IActionResult> GetProjectFields()
        {
            var result = await _projectService.GetProjectFields();
            return Ok(result);
        }

        [HttpGet("getRequiredPositions/{projectID}")]
        public async Task<IActionResult> GetRequiredPositions(int projectID)
        {
            var result = await _projectService.GetRequiredPositions(projectID);
            return Ok(result);
        }

        [HttpGet("getRequiredPosByID/{projectID}&&{posID}")]
        public async Task<IActionResult> GetRequiredPositions(int projectID, int posID)
        {
            var result = await _projectService.GetRequiredPosByID(projectID, posID);
            return Ok(result);
        }

        //http://localhost/api/project/empID
        [HttpPost("{empID}")]
        public async Task<IActionResult> Create(string empID, [FromBody] ProjectCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _projectService.Create(empID, request);
            if (!result.IsSuccessed)
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            return Ok(result);
        }

        //Put:http://localhost/api/project/id
        [HttpPut("{projectID}")]
        public async Task<IActionResult> Update(int projectID, [FromBody] ProjectUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _projectService.Update(projectID, request);
            if (!result.IsSuccessed)
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            return Ok(result);
        }

        [HttpPut("changeStatus/{projectID}")]
        public async Task<IActionResult> ChangeStatus(int projectID)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _projectService.ChangeStatus(projectID);
            if (!result.IsSuccessed)
            {
                return StatusCode(StatusCodes.Status400BadRequest, result);
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

        [HttpPost("addCandidate/{projectID}")]
        public async Task<IActionResult> AddCandidate(int projectID, [FromBody] AddCandidateRequest request)
        {
            var result = await _projectService.AddCandidate(projectID, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("confirmCandidate/{projectID}")]
        public async Task<IActionResult> ConfirmCandidate(int projectID, [FromBody] ConfirmCandidateRequest request)
        {
            var result = await _projectService.ConfirmCandidate(projectID, request);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("getMissEmpPos")]
        public async Task<IActionResult> GetMissEmpPos()
        {
            var result = await _projectService.GetMissEmpPos();
            return Ok(result);
        }

        [HttpGet("getSkillInPos/{posID}")]
        public async Task<IActionResult> GetSkillInPos(int posID)
        {
            var result = await _projectService.GetSkillInPos(posID);
            return Ok(result);
        }

        [HttpGet("getSkillInAllPos")]
        public async Task<IActionResult> GetSkillInAllPos()
        {
            var result = await _projectService.GetSkillInAllPos();
            return Ok(result);
        }

        [HttpGet("getEmpByRequiredID/{requiredID}")]
        public async Task<IActionResult> GetEmpByRequiredID(int requiredID)
        {
            var result = await _projectService.GetEmpByRequiredID(requiredID);
            return Ok(result);
        }

        [HttpPost("checkCandidate/{projectID}")]
        public async Task<IActionResult> CheckCandidate(int projectID, AddCandidateRequest request)
        {
            var result = await _projectService.CheckCandidate(projectID, request);
            return Ok(result);
        }
    }
}