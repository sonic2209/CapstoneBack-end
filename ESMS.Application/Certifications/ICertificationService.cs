using ESMS.ViewModels.Certification;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESMS.Application.Certifications
{
    public interface ICertificationService
    {
        public Task<int> Create(CertificationCreateRequest request);

        public Task<int> Update(CertificationUpdateRequest request);

        public Task<int> Delete(int certificationId);

        public Task<List<CertificationViewModel>> GetCertifications();
    }
}