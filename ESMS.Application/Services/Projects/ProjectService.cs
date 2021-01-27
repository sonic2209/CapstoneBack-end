using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Data.Enums;
using ESMS.Utilities.Exceptions;
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

        public async Task<int> Create(ProjectCreateRequest request, string empID)
        {
            var project = new Project()
            {
                ProjectName = request.ProjectName,
                Description = request.Description,
                Skateholder = request.Skateholder,
                DateCreated = DateTime.Now,
                Status = ProjectStatus.Pending,
                ProjectManagerID = empID
            };
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project.ProjectID;
        }

        public async Task<int> Delete(int projectID)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) throw new ESMSException($"Cannot find a projectID: {projectID}");

            _context.Projects.Remove(project);
            return await _context.SaveChangesAsync();
        }

        public async Task<ProjectViewModel> GetByID(int projectID)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) throw new ESMSException($"Cannot find a projectID: {projectID}");

            var projectViewModel = new ProjectViewModel()
            {
                ProjectID = project.ProjectID,
                ProjectName = project.ProjectName,
                Description = project.Description,
                Skateholder = project.Skateholder,
                Status = project.Status
            };

            return projectViewModel;
        }

        public async Task<PagedResult<EmpInProjectViewModel>> GetEmpInProjectPaging(int projectID, GetEmpInProjectPaging request)
        {
            var query = from e in _context.Employees
                        join ep in _context.EmpPositionInProjects on e.Id equals ep.EmpID
                        join po in _context.Positions on ep.PosID equals po.PosID
                        select new { e, ep, po };
            query = query.Where(x => x.ep.ProjectID == projectID);
            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(x => x.e.Name.Contains(request.Name));
            }
            if (!string.IsNullOrEmpty(request.PosName))
            {
                query = query.Where(x => x.po.Name.Contains(request.PosName));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new EmpInProjectViewModel()
                {
                    Name = x.e.Name,
                    PosName = x.po.Name,
                    Description = x.po.Description,
                    Status = x.e.Status
                }).ToListAsync();

            var pagedResult = new PagedResult<EmpInProjectViewModel>()
            {
                TotalRecord = totalRow,
                Items = data
            };

            return pagedResult;
        }

        public async Task<PagedResult<ProjectViewModel>> GetProjectPaging(GetProjectPagingRequest request)
        {
            var query = from p in _context.Projects
                        select new { p };
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.p.ProjectName.Contains(request.Keyword));
            }
            int totalRow = await query.CountAsync();

            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProjectViewModel()
                {
                    ProjectID = x.p.ProjectID,
                    ProjectName = x.p.ProjectName,
                    Description = x.p.Description,
                    Skateholder = x.p.Skateholder,
                    Status = x.p.Status
                }).ToListAsync();

            var pagedResult = new PagedResult<ProjectViewModel>()
            {
                TotalRecord = totalRow,
                Items = data
            };

            return pagedResult;
        }

        public async Task<int> Update(ProjectUpdateRequest request)
        {
            var project = await _context.Projects.FindAsync(request.ProjectID);
            if (project == null) throw new ESMSException($"Cannot find a projectID: {request.ProjectID}");

            project.ProjectName = request.ProjectName;
            project.Description = request.Description;
            project.Skateholder = request.Skateholder;

            return await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateStatus(int projectID, int status)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) throw new ESMSException($"Cannot find a projectID: {projectID}");

            project.Status = (ProjectStatus)status;

            return await _context.SaveChangesAsync() > 0;
        }
    }
}