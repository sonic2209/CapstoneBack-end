using System.Collections.Generic;
using System.Threading.Tasks;
using ESMS.BackendAPI.Services.Employees;
using ESMS.BackendAPI.ViewModels.Employees;
using ESMS.BackendAPI.ViewModels.Employees.Suggestion;
using ESMS.ViewModels.System.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESMS.BackendAPI.Controllers
{
    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IEmployeeService _userService;

        public UserController(IEmployeeService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _userService.Authenticate(request);
            if (result.ResultObj == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] EmpCreateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _userService.Create(request);
            if (!result.IsSuccessed)
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            return Ok(result);
        }

        //PUT: http://localhost/api/users/id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] EmpUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _userService.Update(id, request);
            if (!result.IsSuccessed)
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            return Ok(result);
        }

        [HttpPut("ChangePassword/{id}")]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var result = await _userService.ChangePassword(id, request);
            if (!result.IsSuccessed)
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            return Ok(result);
        }

        [HttpPost("candidate/{projectID}")]
        public async Task<IActionResult> SuggestCandidate(int projectID, [FromBody] SuggestCadidateRequest request)
        {
            var candidates = await _userService.SuggestCandidate(projectID, request);
            return Ok(candidates);
        }

        [HttpPost("candidate/nomin/{projectID}")]
        public async Task<IActionResult> SuggestCandidateWithoutMinumumPoint(int projectID, [FromBody] SuggestCadidateRequest request)
        {
            var candidates = await _userService.SuggestCandidateWithoutMinimumPoint(projectID, request);
            return Ok(candidates);
        }

        [HttpGet("candidate/{empID}")]
        public async Task<IActionResult> SuggestSingleCandidate(string empID)
        {
            var candidates = await _userService.SingleCandidateSuggest(empID);
            return Ok(candidates);
        }

        //http://localhost/api/user/paging?pageIndex=1&pageSize=10&keyword=
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetEmpPagingRequest request)
        {
            var products = await _userService.GetEmpsPaging(request);
            return Ok(products);
        }

        [HttpPut("candidate/paging")]
        public IActionResult GetCandidatePaging([FromBody] List<MatchViewModel> listMatch, [FromQuery] GetSuggestEmpPagingRequest request)
        {
            var products = _userService.SuggestCandidatePaging(listMatch, request);
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetById(id);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _userService.Delete(id);
            return Ok(result);
        }

        [HttpPost("{empID}")]
        public async Task<IActionResult> AddEmpPosition(string empID, [FromBody] AddEmpPositionRequest request)
        {
            var result = await _userService.AddEmpPosition(empID, request);
            if (!result.IsSuccessed)
            {
                return StatusCode(StatusCodes.Status400BadRequest, result);
            }
            return Ok(result);
        }

        [HttpGet("getEmpInfo/{empID}")]
        public async Task<IActionResult> GetEmpInfo(string empID)
        {
            var result = await _userService.GetEmpInfo(empID);
            return Ok(result);
        }

        //[HttpGet("loadEmpInfo/{empID}")]
        //public async Task<IActionResult> LoadEmpInfo(string empID)
        //{
        //    var result = await _userService.LoadEmpInfo(empID);
        //    return Ok(result);
        //}

        //[HttpPost("updateEmpInfo/{empID}")]
        //public async Task<IActionResult> UpdateEmpInfo(string empID, [FromBody] AddEmpPositionRequest request)
        //{
        //    var result = await _userService.UpdateEmpInfo(empID, request);
        //    if (!result.IsSuccessed)
        //    {
        //        return StatusCode(StatusCodes.Status400BadRequest, result);
        //    }
        //    return Ok(result);
        //}

        [HttpGet("template")]
        public IActionResult GetEmpTemplate()
        {
            var rs = _userService.GetEmpTemplate();
            return File(rs.Data, rs.FileType, rs.FileName + ".xlsx");
        }
        [HttpPut("Import")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> ImportEmployeeInfo([FromForm] IFormFile file)
        {
            var result = await _userService.ImportEmployeeInfo(file);
            if (!result.IsSuccessed)
            {
                return StatusCode(StatusCodes.Status403Forbidden, result);
            }
            return Ok(result);
        }
        [HttpGet("Export/{userId}")]
        public async Task<IActionResult> ExportEmployeeInfo (string userId)
        {
            var rs = await _userService.ExportEmployeeInfo(userId);
            return File(rs.Data, rs.FileType, rs.FileName + ".xlsx");
        }
    }
}