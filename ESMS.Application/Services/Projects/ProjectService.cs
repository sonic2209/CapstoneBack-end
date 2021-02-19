using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Data.Enums;
using ESMS.ViewModels.Common;
using ESMS.ViewModels.Services.Project;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ESMS.Application.Services.Projects
{
    public class ProjectService : IProjectService
    {
        private readonly ESMSDbContext _context;

        public ProjectService(ESMSDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<int>> Create(string EmpID, ProjectCreateRequest request)
        {
            var project = _context.Projects.Find(request.ProjectID);
            if (project != null)
            {
                project.ProjectName = request.ProjectName;
                project.Description = request.Description;
                project.Skateholder = request.Skateholder;
                project.DateBegin = request.DateBegin;
                project.DateEstimatedEnd = request.DateEstimatedEnd;
                _context.Projects.Update(project);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ApiErrorResult<int>("Update project failed");
                }
            }
            else
            {
                project = new Project()
                {
                    ProjectName = request.ProjectName,
                    Description = request.Description,
                    Skateholder = request.Skateholder,
                    DateCreated = DateTime.Now,
                    DateBegin = request.DateBegin,
                    DateEstimatedEnd = request.DateEstimatedEnd,
                    Status = ProjectStatus.Pending
                };
                _context.Projects.Add(project);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ApiErrorResult<int>("Create project failed");
                }
            }
            return new ApiSuccessResult<int>(project.ProjectID);
        }

        public async Task<ApiResult<bool>> Delete(int projectID)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) return new ApiErrorResult<bool>("Project does not exist");

            project.DateEnd = DateTime.Now;
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Delete project failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<ProjectViewModel>> GetByID(int projectID)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) return new ApiErrorResult<ProjectViewModel>("Project does not exist");

            var projectViewModel = new ProjectViewModel()
            {
                ProjectID = project.ProjectID,
                ProjectName = project.ProjectName,
                Description = project.Description,
                Skateholder = project.Skateholder,
                DateBegin = project.DateBegin.ToString("dd/MM/yyyy"),
                DateEstimatedEnd = project.DateEstimatedEnd.ToString("dd/MM/yyyy"),
                Status = project.Status
            };

            return new ApiSuccessResult<ProjectViewModel>(projectViewModel);
        }

        public async Task<ApiResult<PagedResult<EmpInProjectViewModel>>> GetEmpInProjectPaging(int projectID, GetEmpInProjectPaging request)
        {
            var query = from e in _context.Employees
                        join ep in _context.EmpPositionInProjects on e.Id equals ep.EmpID
                        join po in _context.Positions on ep.PosID equals po.PosID
                        select new { e, ep, po };
            query = query.Where(x => x.ep.ProjectID == projectID);
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.e.Name.Contains(request.Keyword) || x.po.Name.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new EmpInProjectViewModel()
                {
                    EmployeeID = x.e.Id,
                    Name = x.e.Name,
                    PosName = x.po.Name,
                    Description = x.po.Description,
                    Status = x.e.Status
                }).ToListAsync();

            var pagedResult = new PagedResult<EmpInProjectViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return new ApiSuccessResult<PagedResult<EmpInProjectViewModel>>(pagedResult);
        }

        public async Task<ApiResult<PagedResult<ProjectViewModel>>> GetProjectPaging(GetProjectPagingRequest request)
        {
            var query = from p in _context.Projects
                        select new { p };
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.p.ProjectName.Contains(request.Keyword));
            }

            //Paging
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProjectViewModel()
                {
                    ProjectID = x.p.ProjectID,
                    ProjectName = x.p.ProjectName,
                    Description = x.p.Description,
                    Skateholder = x.p.Skateholder,
                    DateBegin = x.p.DateBegin.ToString("dd/MM/yyyy"),
                    DateEstimatedEnd = x.p.DateEstimatedEnd.ToString("dd/MM/yyyy"),
                    Status = x.p.Status
                }).ToListAsync();

            //Select and projection
            var pagedResult = new PagedResult<ProjectViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return new ApiSuccessResult<PagedResult<ProjectViewModel>>(pagedResult);
        }

        public async Task<ApiResult<bool>> Update(int projectID, ProjectUpdateRequest request)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) return new ApiErrorResult<bool>("Project does not exist");

            project.ProjectName = request.ProjectName;
            project.Description = request.Description;
            project.Skateholder = request.Skateholder;

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update project failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<bool>> UpdateStatus(int projectID, int status)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) return new ApiErrorResult<bool>("Project does not exist");

            project.Status = (ProjectStatus)status;

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update project failed");
            }
            return new ApiSuccessResult<bool>();
        }
    }
}