using ESMS.Data.EF;
using ESMS.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ESMS.ViewModels.System.Certification;
using ESMS.ViewModels.Common;

namespace ESMS.Application.System.Certifications
{
    public class CertificationService : ICertificationService
    {
        private readonly ESMSDbContext _context;

        public CertificationService(ESMSDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<bool>> Create(CertificationCreateRequest request)
        {
            var certification = new Certification()
            {
                CertificationName = request.CertificationName,
                Description = request.Description,
                Image = request.Image
            };
            _context.Certifications.Add(certification);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Create certification failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<bool>> Delete(int certificationID)
        {
            var certification = _context.Certifications.Find(certificationID);
            if (certification == null) return new ApiErrorResult<bool>("Certification does not exist");
            _context.Certifications.Remove(certification);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Delete certification failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<List<CertificationViewModel>>> GetCertifications()
        {
            var query = from c in _context.Certifications
                        select new { c };
            var data = await query.Select(x => new CertificationViewModel()
            {
                CertificationID = x.c.CertificationID,
                CertificationName = x.c.CertificationName
            }).ToListAsync();

            return new ApiSuccessResult<List<CertificationViewModel>>(data);
        }

        public async Task<ApiResult<bool>> Update(int certificationID, CertificationUpdateRequest request)
        {
            var certification = _context.Certifications.Find(certificationID);
            if (certification == null) return new ApiErrorResult<bool>("Certification does not exist");
            certification.CertificationName = request.CertificationName;
            certification.Description = request.Image;
            certification.Image = request.Image;
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update certification failed");
            }
            return new ApiSuccessResult<bool>();
        }
    }
}