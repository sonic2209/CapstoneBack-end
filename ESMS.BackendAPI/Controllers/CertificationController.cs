using ESMS.Application.Certifications;
using ESMS.ViewModels.Certification;
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
    public class CertificationController : ControllerBase
    {
        private readonly ICertificationService _certificationService;

        public CertificationController(ICertificationService certificationService)
        {
            _certificationService = certificationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CertificationCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var affectedResult = await _certificationService.Create(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromForm] CertificationUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var affectedResult = await _certificationService.Update(request);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpDelete("{certificationId}")]
        public async Task<IActionResult> Delete(int certificationId)
        {
            var affectedResult = await _certificationService.Delete(certificationId);
            if (affectedResult == 0)
                return BadRequest();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetCertifications()
        {
            var certifications = await _certificationService.GetCertifications();
            if (certifications == null)
            {
                return BadRequest();
            }
            return Ok(certifications);
        }
    }
}