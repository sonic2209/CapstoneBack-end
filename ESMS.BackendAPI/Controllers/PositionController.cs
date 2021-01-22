using ESMS.Application.Services.Positions;
using ESMS.ViewModels.Services.Position;
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
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _positionService;

        public PositionController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPositionPaging([FromQuery] GetPositionPagingRequest request)
        {
            var positions = await _positionService.GetPositionPaging(request);
            if (positions == null)
            {
                return BadRequest();
            }

            return Ok(positions);
        }

        [HttpGet("{empID}")]
        public async Task<IActionResult> GetEmpPositionPaging(string empID, [FromQuery] GetPositionPagingRequest request)
        {
            var positions = await _positionService.GetEmpPositionPaging(empID, request);
            if (positions == null)
            {
                return BadRequest();
            }

            return Ok(positions);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PositionCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var positionId = await _positionService.Create(request);

            if (positionId == 0)
                return BadRequest();

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] PositionUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var affectedResult = await _positionService.Update(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("{positionId}")]
        public async Task<IActionResult> Delete(int positionId)
        {
            var affectedResult = await _positionService.Delete(positionId);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }
    }
}