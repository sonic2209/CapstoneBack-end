using ESMS.Application.Services.Positions;
using ESMS.ViewModels.Services.Position;
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
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        //http://localhost/api/position/paging?pageIndex=1&pageSize=10&keyword=
        [HttpGet("paging")]
        public async Task<IActionResult> GetPositionPaging([FromQuery] GetPositionPagingRequest request)
        {
            var positions = await _positionService.GetPositionPaging(request);
            return Ok(positions);
        }

        //http://localhost/api/position/getPositions
        [HttpGet("getPositions")]
        public async Task<IActionResult> GetPositions()
        {
            var positions = await _positionService.GetPositions();
            return Ok(positions);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PositionCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _positionService.Create(request);

            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //http://localhost/api/position/id
        [HttpPut("{positionID}")]
        public async Task<IActionResult> Update(int positionID, [FromBody] PositionUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _positionService.Update(positionID, request);

            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}