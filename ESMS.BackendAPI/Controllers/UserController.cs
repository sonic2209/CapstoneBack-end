﻿using System.Threading.Tasks;
using ESMS.BackendAPI.Services.Employees;
using ESMS.BackendAPI.ViewModels.Employees;
using ESMS.ViewModels.System.Employees;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
                return BadRequest(result);
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
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("candidate/{projectID}")]
        public async Task<IActionResult> SuggestCandidate(int projectID, [FromBody] SuggestCadidateRequest request)
        {
            var candidates = await _userService.SuggestCandidate(projectID, request);
            return Ok(candidates);
        }

        [HttpGet("candidate/{empID}")]
        public async Task<IActionResult> SuggestCandidate(string id)
        {
            var candidates = await _userService.SingleCandidateSuggest(id);
            return Ok(candidates);
        }

        //http://localhost/api/user/paging?pageIndex=1&pageSize=10&keyword=
        [HttpGet("paging")]
        public async Task<IActionResult> GetAllPaging([FromQuery] GetEmpPagingRequest request)
        {
            var products = await _userService.GetEmpsPaging(request);
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
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("getEmpInfo/{empID}")]
        public async Task<IActionResult> GetEmpInfo(string empID)
        {
            var result = await _userService.GetEmpInfo(empID);
            return Ok(result);
        }

        [HttpGet("loadEmpInfo/{empID}")]
        public async Task<IActionResult> LoadEmpInfo(string empID)
        {
            var result = await _userService.LoadEmpInfo(empID);
            return Ok(result);
        }

        [HttpPost("updateEmpInfo/{empID}")]
        public async Task<IActionResult> UpdateEmpInfo(string empID, [FromBody] AddEmpPositionRequest request)
        {
            var result = await _userService.UpdateEmpInfo(empID, request);
            return Ok(result);
        }
    }
}