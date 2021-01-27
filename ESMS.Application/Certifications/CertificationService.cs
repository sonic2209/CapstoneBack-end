using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.ViewModels.Certification;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ESMS.Application.Certifications
{
    public class CertificationService : ICertificationService
    {
        private readonly ESMSDbContext _context;

        public CertificationService(ESMSDbContext context)
        {
            _context = context;
        }

        public async Task<int> Create(CertificationCreateRequest request)
        {
            var certification = new Certification()
            {
                CertificationName = request.CertificationName,
                Description = request.Description,
                Image = request.Image
            };
            _context.Certifications.Add(certification);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Delete(int certificationId)
        {
            var certification = _context.Certifications.Find(certificationId);

            _context.Certifications.Remove(certification);
            return await _context.SaveChangesAsync();
        }

        public async Task<List<CertificationViewModel>> GetCertifications()
        {
            var query = from c in _context.Certifications
                        select new { c };
            var data = await query.Select(x => new CertificationViewModel()
            {
                CertificationID = x.c.CertificationID,
                CertificationName = x.c.CertificationName
            }).ToListAsync();

            return data;
        }

        public async Task<int> Update(CertificationUpdateRequest request)
        {
            var certification = _context.Certifications.Find(request.CertificationID);

            certification.CertificationName = request.CertificationName;
            certification.Description = request.Image;
            certification.Image = request.Image;
            return await _context.SaveChangesAsync();
        }
    }
}