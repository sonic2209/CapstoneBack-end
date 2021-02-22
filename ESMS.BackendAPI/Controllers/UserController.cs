using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESMS.Application.System.Employees;
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
            if (string.IsNullOrEmpty(result.ResultObj))
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
    }
}