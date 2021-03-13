﻿using ESMS.Data.EF;
using ESMS.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ESMS.BackendAPI.ViewModels.Certification;
using ESMS.BackendAPI.ViewModels.Common;

namespace ESMS.BackendAPI.Services.Certifications
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
                SkillID = request.SkillID
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

        public async Task<ApiResult<CertificationViewModel>> GetByID(int certificationID)
        {
            var certification = await _context.Certifications.FindAsync(certificationID);
            if (certification == null) return new ApiErrorResult<CertificationViewModel>("Certification does not exist");
            var certificationVm = new CertificationViewModel()
            {
                CertificationID = certificationID,
                CertificationName = certification.CertificationName,
                Description = certification.Description,
                SkillID = certification.SkillID
            };

            return new ApiSuccessResult<CertificationViewModel>(certificationVm);
        }

        public async Task<ApiResult<PagedResult<ListCertificationViewModel>>> GetCertificationPaging(GetCertificationPagingRequest request)
        {
            var query = from c in _context.Certifications
                        select new { c };
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.c.CertificationName.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ListCertificationViewModel()
                {
                    CertificationID = x.c.CertificationID,
                    CertificationName = x.c.CertificationName
                }).ToListAsync();

            var pagedResult = new PagedResult<ListCertificationViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return new ApiSuccessResult<PagedResult<ListCertificationViewModel>>(pagedResult);
        }

        public async Task<ApiResult<List<ListCertificationViewModel>>> GetCertifications(int skillID)
        {
            var query = from c in _context.Certifications
                        select new { c };
            query = query.Where(x => x.c.SkillID.Equals(skillID));
            var data = await query.Select(x => new ListCertificationViewModel()
            {
                CertificationID = x.c.CertificationID,
                CertificationName = x.c.CertificationName
            }).ToListAsync();

            return new ApiSuccessResult<List<ListCertificationViewModel>>(data);
        }

        public async Task<ApiResult<bool>> Update(int certificationID, CertificationUpdateRequest request)
        {
            var certification = _context.Certifications.Find(certificationID);
            if (certification == null) return new ApiErrorResult<bool>("Certification does not exist");
            certification.CertificationName = request.CertificationName;
            certification.Description = request.Description;
            certification.SkillID = request.SkillID;
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update certification failed");
            }
            return new ApiSuccessResult<bool>();
        }
    }
}