using ESMS.BackendAPI.ViewModels.Certification;
using ESMS.BackendAPI.ViewModels.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Certifications
{
    public interface ICertificationService
    {
        public Task<ApiResult<bool>> Create(CertificationCreateRequest request);

        public Task<ApiResult<bool>> Update(int certificationID, CertificationUpdateRequest request);

        public Task<ApiResult<bool>> Delete(int certificationID);

        public Task<ApiResult<List<ListCertificationViewModel>>> GetCertifications(int skillID);

        public Task<ApiResult<PagedResult<CertificationViewModel>>> GetCertificationPaging(GetCertificationPagingRequest request);

        public Task<ApiResult<CertificationViewModel>> GetByID(int certificationID);
    }
}