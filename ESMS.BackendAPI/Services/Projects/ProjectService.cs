﻿using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Data.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Project;
using ESMS.BackendAPI.ViewModels.Position;

namespace ESMS.BackendAPI.Services.Projects
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
                if (DateTime.Compare(request.DateBegin, request.DateEstimatedEnd) > 0)
                {
                    return new ApiErrorResult<int>("Date estimated end is earlier than date begin");
                }
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
                if (DateTime.Compare(request.DateBegin, request.DateEstimatedEnd) > 0)
                {
                    return new ApiErrorResult<int>("Date estimated end is earlier than date begin");
                }
                project = new Project()
                {
                    ProjectName = request.ProjectName,
                    Description = request.Description,
                    Skateholder = request.Skateholder,
                    DateCreated = DateTime.Now,
                    DateBegin = request.DateBegin,
                    DateEstimatedEnd = request.DateEstimatedEnd,
                    Status = ProjectStatus.Pending,
                    ProjectManagerID = EmpID
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

        public async Task<ApiResult<List<PositionInProject>>> GetEmpInProjectPaging(int projectID)
        {
            var query = from e in _context.Employees
                        join ep in _context.EmpPositionInProjects on e.Id equals ep.EmpID
                        join po in _context.Positions on ep.PosID equals po.PosID
                        select new { e, ep, po };
            query = query.Where(x => x.ep.ProjectID == projectID);
            var positions = await query.Select(x => new ListPositionViewModel()
            {
                PosID = x.po.PosID,
                Name = x.po.Name
            }).Distinct().ToListAsync();
            var list = new List<PositionInProject>();
            foreach (var pos in positions)
            {
                var employees = await query.Where(x => x.ep.PosID == pos.PosID).Select(x => new EmpInProject()
                {
                    EmpID = x.e.Id,
                    Name = x.e.Name,
                    Status = x.e.Status,
                    DateIn = x.ep.DateIn
                }).ToListAsync();
                var positionInProject = new PositionInProject()
                {
                    PosID = pos.PosID,
                    PosName = pos.Name,
                    Employees = employees
                };
                list.Add(positionInProject);
            }
            return new ApiSuccessResult<List<PositionInProject>>(list);
        }

        public async Task<ApiResult<PagedResult<ProjectViewModel>>> GetProjectByEmpID(string EmpID, GetProjectPagingRequest request)
        {
            var query = from p in _context.Projects
                        select new { p };
            query = query.Where(x => x.p.ProjectManagerID.Equals(EmpID));
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.p.ProjectName.Contains(request.Keyword));
            }

            //Paging
            int totalRow = await query.CountAsync();

            var data = await query.OrderByDescending(x => x.p.DateCreated)
                .Skip((request.PageIndex - 1) * request.PageSize)
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

            var data = await query.OrderByDescending(x => x.p.DateCreated)
                .Skip((request.PageIndex - 1) * request.PageSize)
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

        public async Task<ApiResult<bool>> AddRequiredPosition(int projectID, AddRequiredPositionRequest request)
        {
            foreach (var position in request.RequiredPositions)
            {
                var requiredPosition = new RequiredPosition()
                {
                    NumberOfCandidates = position.NumberOfCandidates,
                    PositionID = position.PosID,
                    ProjectID = projectID
                };
                _context.RequiredPositions.Add(requiredPosition);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ApiErrorResult<bool>("Create requiredPosition failed");
                }
                foreach (var language in position.Language)
                {
                    var requiredLanguage = new RequiredLanguage()
                    {
                        LangID = language.LangID,
                        RequiredPositionID = requiredPosition.ID,
                        Priority = language.Priority
                    };
                    _context.RequiredLanguages.Add(requiredLanguage);
                }
                RequiredSkill requiredSkill;
                foreach (var softSkill in position.SoftSkillIDs)
                {
                    requiredSkill = new RequiredSkill()
                    {
                        RequiredPositionID = requiredPosition.ID,
                        SkillID = softSkill
                    };
                    _context.RequiredSkills.Add(requiredSkill);
                }
                foreach (var hardSkill in position.HardSkills)
                {
                    requiredSkill = new RequiredSkill()
                    {
                        RequiredPositionID = requiredPosition.ID,
                        SkillID = hardSkill.HardSkillID,
                        Priority = hardSkill.Priority,
                        Exp = hardSkill.Exp,
                        CertificationLevel = hardSkill.CertificationLevel
                    };
                    _context.RequiredSkills.Add(requiredSkill);
                }
                result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ApiErrorResult<bool>("Create requiredSkill failed");
                }
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<int>> ChangeStatus(int projectID)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) return new ApiErrorResult<int>("Project does not exist");

            project.Status = ProjectStatus.Finished;
            project.DateEnd = DateTime.Now;
            _context.Projects.Update(project);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<int>("Update project failed");
            }
            var empInProject = await _context.EmpPositionInProjects.Where(x => x.ProjectID.Equals(projectID)).Select(x => new EmpPositionInProject()
            {
                EmpID = x.EmpID,
                PosID = x.PosID,
                ProjectID = x.ProjectID,
                DateIn = x.DateIn
            }).ToListAsync();
            foreach (var emp in empInProject)
            {
                var employee = await _context.Employees.FindAsync(emp.EmpID);
                employee.Status = EmployeeStatus.Pending;
                _context.Employees.Update(employee);
            }
            result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<int>("Update project failed");
            }
            return new ApiSuccessResult<int>((int)project.Status);
        }

        public async Task<ApiResult<bool>> AddCandidate(int projectID, AddCandidateRequest request)
        {
            foreach (var candidate in request.Candidates)
            {
                foreach (var emp in candidate.EmpIDs)
                {
                    var employee = new EmpPositionInProject()
                    {
                        EmpID = emp,
                        PosID = candidate.PosID,
                        ProjectID = projectID
                    };
                    _context.EmpPositionInProjects.Add(employee);
                }
            }
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("add candidate failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<bool>> ConfirmCandidate(int projectID, ConfirmCandidateRequest request)
        {
            foreach (var position in request.Candidates)
            {
                foreach (var id in position.EmpIDs)
                {
                    var employee = _context.Employees.Find(id);
                    employee.Status = EmployeeStatus.OnGoing;
                    _context.Employees.Update(employee);
                    var empInProject = _context.EmpPositionInProjects.Where(x => x.EmpID.Equals(id)
                    && x.PosID.Equals(position.PosID) && x.ProjectID.Equals(projectID)).Select(x => new EmpPositionInProject()
                    {
                        EmpID = x.EmpID,
                        PosID = x.PosID,
                        ProjectID = x.ProjectID,
                        DateIn = DateTime.Now
                    }).FirstOrDefault();
                    _context.EmpPositionInProjects.Update(empInProject);
                }
            }
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("confirm candidate failed");
            }
            return new ApiSuccessResult<bool>();
        }
    }
}