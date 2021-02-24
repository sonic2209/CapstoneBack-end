using ESMS.Application.System.Certifications;
using ESMS.ViewModels.System.Certification;
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
    public class CertificationController : ControllerBase
    {
        private readonly ICertificationService _certificationService;

        public CertificationController(ICertificationService certificationService)
        {
            _certificationService = certificationService;
        }

        [HttpGet("{certificationID}")]
        public async Task<IActionResult> GetByID(int certificationID)
        {
            var certification = await _certificationService.GetByID(certificationID);
            return Ok(certification);
        }

        //http://localhost/api/certification/paging?pageIndex=1&pageSize=10&keyword=
        [HttpGet("paging")]
        public async Task<IActionResult> GetPositionPaging([FromQuery] GetCertificationPagingRequest request)
        {
            var certifications = await _certificationService.GetCertificationPaging(request);
            return Ok(certifications);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CertificationCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _certificationService.Create(request);

            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //PUT: http://localhost/api/certification/id
        [HttpPut("{certificationID}")]
        public async Task<IActionResult> Update(int certificationID, [FromBody] CertificationUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var result = await _certificationService.Update(certificationID, request);

            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //Delete: http://localhost/api/certification/id
        [HttpDelete("{certificationID}")]
        public async Task<IActionResult> Delete(int certificationID)
        {
            var result = await _certificationService.Delete(certificationID);
            if (!result.IsSuccessed)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        //Get: http://localhost/api/certification/getCertifications
        [HttpGet("getCertifications")]
        public async Task<IActionResult> GetCertifications()
        {
            var certifications = await _certificationService.GetCertifications();
            return Ok(certifications);
        }
    }
}