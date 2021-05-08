using ESMS.Data.EF;
using ESMS.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ESMS.BackendAPI.ViewModels.Certification;
using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.Ultilities;

namespace ESMS.BackendAPI.Services.Certifications
{
    public class CertificationService : ICertificationService
    {
        private readonly ESMSDbContext _context;

        public CertificationService(ESMSDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<bool>> ChangeStatus(int certificationID)
        {
            var certification = await _context.Certifications.FindAsync(certificationID);
            if (certification == null) return new ApiErrorResult<bool>("Certification does not exist");
            if (certification.Status)
            {
                var empCertification = await _context.EmpCertifications.Where(x => x.CertificationID.Equals(certificationID))
                    .Select(x => x.EmpID).ToListAsync();
                if (empCertification.Count() != 0)
                {
                    return new ApiErrorResult<bool>("This certification is assigned to some employees");
                }
                certification.Status = false;
            }
            else
            {
                certification.Status = true;
            }
            _context.Certifications.Update(certification);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update certification failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<bool>> Create(CertificationCreateRequest request)
        {
            UltilitiesService ultilities = new UltilitiesService();
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            var checkName = await _context.Certifications.Where(x => x.CertificationName.Equals(request.CertificationName))
                .Select(x => new Certification()).FirstOrDefaultAsync();
            if (checkName != null)
            {
                ultilities.AddOrUpdateError(errors, "CertificationName", "This certification name already exists");
                //return new ApiErrorResult<bool>("This certification name already exists");
            }
            if (errors.Count() > 0)
            {
                return new ApiErrorResult<bool>(errors);
            }
            var certification = new Certification()
            {
                CertificationName = request.CertificationName,
                Description = request.Description,
                SkillID = request.SkillID,
                CertiLevel = request.CertiLevel
            };
            _context.Certifications.Add(certification);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Create certification failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<CertificationViewModel>> GetByID(int certificationID)
        {
            var query = from c in _context.Certifications
                        join s in _context.Skills on c.SkillID equals s.SkillID
                        select new { c, s };
            var certificationVm = await query.Where(x => x.c.CertificationID.Equals(certificationID)).Select(x => new CertificationViewModel()
            {
                CertificationID = x.c.CertificationID,
                CertificationName = x.c.CertificationName,
                Description = x.c.Description,
                SkillID = x.c.SkillID,
                SkillName = x.s.SkillName,
                CertiLevel = x.c.CertiLevel,
                Status = x.c.Status
            }).FirstOrDefaultAsync();
            if (certificationVm == null) return new ApiErrorResult<CertificationViewModel>("Certification does not exist");
            return new ApiSuccessResult<CertificationViewModel>(certificationVm);
        }

        public async Task<ApiResult<PagedResult<CertificationViewModel>>> GetCertificationPaging(GetCertificationPagingRequest request)
        {
            var query = from c in _context.Certifications
                        join s in _context.Skills on c.SkillID equals s.SkillID
                        select new { c, s };
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.c.CertificationName.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new CertificationViewModel()
                {
                    CertificationID = x.c.CertificationID,
                    CertificationName = x.c.CertificationName,
                    Description = x.c.Description,
                    SkillID = x.c.SkillID,
                    SkillName = x.s.SkillName,
                    CertiLevel = x.c.CertiLevel,
                    Status = x.c.Status
                }).ToListAsync();

            var pagedResult = new PagedResult<CertificationViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return new ApiSuccessResult<PagedResult<CertificationViewModel>>(pagedResult);
        }

        public async Task<ApiResult<List<ListCertificationViewModel>>> GetCertifications(int skillID)
        {
            var data = await _context.Certifications.Where(x => x.SkillID.Equals(skillID) && x.Status.Equals(true))
                .OrderBy(x => x.CertiLevel).Select(x => new ListCertificationViewModel()
                {
                    CertificationID = x.CertificationID,
                    CertificationName = x.CertificationName,
                    CertiLevel = x.CertiLevel
                }).ToListAsync();

            return new ApiSuccessResult<List<ListCertificationViewModel>>(data);
        }

        public async Task<ApiResult<bool>> Update(int certificationID, CertificationUpdateRequest request)
        {
            UltilitiesService ultilities = new UltilitiesService();
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            var certification = await _context.Certifications.FindAsync(certificationID);
            if (certification == null) return new ApiErrorResult<bool>("Certification does not exist");
            if (!certification.CertificationName.Equals(request.CertificationName))
            {
                var checkName = await _context.Certifications.Where(x => x.CertificationName.Equals(request.CertificationName))
                    .Select(x => new Certification()).FirstOrDefaultAsync();
                if (checkName != null)
                {
                    ultilities.AddOrUpdateError(errors, "CertificationName", "This certification name already exists");
                    //return new ApiErrorResult<bool>("this certification name already exists");
                }
                if (errors.Count() > 0)
                {
                    return new ApiErrorResult<bool>(errors);
                }
                certification.CertificationName = request.CertificationName;
            }
            certification.Description = request.Description;
            certification.SkillID = request.SkillID;
            certification.CertiLevel = request.CertiLevel;
            _context.Certifications.Update(certification);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update certification failed");
            }
            return new ApiSuccessResult<bool>();
        }
    }
}