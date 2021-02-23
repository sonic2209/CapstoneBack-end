﻿using ESMS.ViewModels.Common;
using ESMS.ViewModels.System.Certification;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ESMS.Application.System.Certifications
{
    public interface ICertificationService
    {
        public Task<ApiResult<bool>> Create(CertificationCreateRequest request);

        public Task<ApiResult<bool>> Update(int certificationID, CertificationUpdateRequest request);

        public Task<ApiResult<bool>> Delete(int certificationID);

        public Task<ApiResult<List<ListCertificationViewModel>>> GetCertifications();

        public Task<ApiResult<PagedResult<ListCertificationViewModel>>> GetCertificationPaging(GetCertificationPagingRequest request);

        public Task<ApiResult<CertificationViewModel>> GetByID(int certificationID);
    }
}