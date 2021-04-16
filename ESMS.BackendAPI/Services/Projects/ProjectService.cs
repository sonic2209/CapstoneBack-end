using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Data.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Project;
using ESMS.BackendAPI.ViewModels.Position;
using ESMS.BackendAPI.ViewModels.Project.Statistics;

namespace ESMS.BackendAPI.Services.Projects
{
    public class ProjectService : IProjectService
    {
        private readonly ESMSDbContext _context;

        public ProjectService(ESMSDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<int>> Create(string empID, ProjectCreateRequest request)
        {
            var project = _context.Projects.Find(request.ProjectID);
            if (project != null)
            {
                if (!project.ProjectName.Equals(request.ProjectName))
                {
                    var checkName = _context.Projects.Where(x => x.ProjectName.Equals(request.ProjectName)).Select(x => new Project()).FirstOrDefault();
                    if (checkName != null)
                    {
                        return new ApiErrorResult<int>("projectName : This project name is existed");
                    }
                    project.ProjectName = request.ProjectName;
                }
                project.Description = request.Description;
                if (DateTime.Compare(project.DateBegin.Date, request.DateBegin.Date) != 0)
                {
                    var checkDate = _context.Projects.Where(x => x.ProjectManagerID.Equals(empID) && x.Status != ProjectStatus.Finished
                    && x.ProjectID != project.ProjectID).OrderByDescending(x => x.DateEstimatedEnd)
                    .Select(x => x.DateEstimatedEnd).FirstOrDefault();
                    if (DateTime.Compare(request.DateBegin, checkDate.AddDays(3)) < 0)
                    {
                        return new ApiErrorResult<int>("dateBegin : Date begin must be after your current project's estimated end date(" + checkDate.ToString("dd/MM/yyyy") + ") at least 3 days");
                    }
                    project.DateBegin = request.DateBegin;
                }
                if (DateTime.Compare(request.DateBegin, request.DateEstimatedEnd) > 0)
                {
                    return new ApiErrorResult<int>("dateEstimatedEnd : Date estimated end is earlier than date begin");
                }
                project.DateEstimatedEnd = request.DateEstimatedEnd;
                project.ProjectTypeID = request.ProjectTypeID;
                project.ProjectFieldID = request.ProjectFieldID;
                _context.Projects.Update(project);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ApiErrorResult<int>("Update project failed");
                }
            }
            else
            {
                var checkName = _context.Projects.Where(x => x.ProjectName.Equals(request.ProjectName))
                    .Select(x => new Project()).FirstOrDefault();
                if (checkName != null)
                {
                    return new ApiErrorResult<int>("projectName : This project name is existed");
                }
                var checkDate = _context.Projects.Where(x => x.ProjectManagerID.Equals(empID) && x.Status != ProjectStatus.Finished)
                    .OrderByDescending(x => x.DateEstimatedEnd)
                    .Select(x => x.DateEstimatedEnd).FirstOrDefault();
                if (DateTime.Compare(request.DateBegin, checkDate.AddDays(3)) < 0)
                {
                    return new ApiErrorResult<int>("dateBegin : Date begin must be after your current project's estimated end date(" + checkDate.ToString("dd/MM/yyyy") + ") at least 3 days");
                }
                if (DateTime.Compare(request.DateBegin, request.DateEstimatedEnd) > 0)
                {
                    return new ApiErrorResult<int>("dateEstimatedEnd : Date estimated end is earlier than date begin");
                }
                project = new Project()
                {
                    ProjectName = request.ProjectName,
                    Description = request.Description,
                    DateCreated = DateTime.Now,
                    DateBegin = request.DateBegin,
                    DateEstimatedEnd = request.DateEstimatedEnd,
                    Status = ProjectStatus.Pending,
                    ProjectManagerID = empID,
                    ProjectTypeID = request.ProjectTypeID,
                    ProjectFieldID = request.ProjectFieldID
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
            var requirePos = await _context.RequiredPositions.Where(x => x.ProjectID.Equals(projectID))
                .Select(x => new RequiredPosition()
                {
                    ID = x.ID,
                    ProjectID = x.ProjectID,
                    PositionID = x.PositionID,
                    CandidateNeeded = x.CandidateNeeded,
                    DateCreated = x.DateCreated,
                    MissingEmployee = x.MissingEmployee
                }).ToListAsync();
            if (requirePos.Count() != 0)
            {
                foreach (var pos in requirePos)
                {
                    var requireSkill = await _context.RequiredSkills.Where(x => x.RequiredPositionID.Equals(pos.ID))
                        .Select(x => new RequiredSkill()
                        {
                            SkillID = x.SkillID,
                            RequiredPositionID = x.RequiredPositionID,
                            CertificationLevel = x.CertificationLevel,
                            SkillLevel = x.SkillLevel,
                            Priority = x.Priority
                        }).ToListAsync();
                    if (requireSkill.Count() != 0)
                    {
                        foreach (var skill in requireSkill)
                        {
                            _context.RequiredSkills.Remove(skill);
                        }
                    }
                    var requireLanguage = await _context.RequiredLanguages.Where(x => x.RequiredPositionID.Equals(pos.ID))
                        .Select(x => new RequiredLanguage()
                        {
                            LangID = x.LangID,
                            RequiredPositionID = x.RequiredPositionID,
                            Priority = x.Priority
                        }).ToListAsync();
                    if (requireLanguage.Count() != 0)
                    {
                        foreach (var lang in requireLanguage)
                        {
                            _context.RequiredLanguages.Remove(lang);
                        }
                    }
                    _context.RequiredPositions.Remove(pos);
                }
            }
            var empInProject = await _context.EmpPositionInProjects.Where(x => x.ProjectID.Equals(projectID))
                .Select(x => new EmpPositionInProject()
                {
                    EmpID = x.EmpID,
                    PosID = x.PosID,
                    ProjectID = x.ProjectID,
                    DateIn = x.DateIn
                }).ToListAsync();
            if (empInProject.Count() != 0)
            {
                foreach (var emp in empInProject)
                {
                    _context.EmpPositionInProjects.Remove(emp);
                }
                var check = await _context.SaveChangesAsync();
                if (check == 0)
                {
                    return new ApiErrorResult<bool>("Delete employee in project failed");
                }
            }
            _context.Projects.Remove(project);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Delete project failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<ProjectViewModel>> GetByID(int projectID)
        {
            var query = from p in _context.Projects
                        join pt in _context.ProjectTypes on p.ProjectTypeID equals pt.ID
                        select new { p, pt };
            var projectVM = await query.Where(x => x.p.ProjectID.Equals(projectID)).Select(x => new ProjectViewModel()
            {
                ProjectID = x.p.ProjectID,
                ProjectName = x.p.ProjectName,
                Description = x.p.Description,
                DateBegin = x.p.DateBegin,
                DateEstimatedEnd = x.p.DateEstimatedEnd,
                Status = x.p.Status,
                TypeID = x.p.ProjectTypeID,
                TypeName = x.pt.Name,
                FieldID = x.p.ProjectFieldID,
                DateEnd = x.p.DateEnd,
                PmID = x.p.ProjectManagerID
            }).FirstOrDefaultAsync();
            if (projectVM == null) return new ApiErrorResult<ProjectViewModel>("Project does not exist");
            projectVM.FieldName = _context.ProjectFields.Find(projectVM.FieldID).Name;
            return new ApiSuccessResult<ProjectViewModel>(projectVM);
        }

        public async Task<ApiResult<List<PositionInProject>>> GetEmpInProjectPaging(int projectID)
        {
            var query = from rp in _context.RequiredPositions
                        join po in _context.Positions on rp.PositionID equals po.PosID
                        select new { rp, po };
            var positions = await query.Where(x => x.rp.ProjectID.Equals(projectID))
                .Select(x => new ListPositionViewModel()
                {
                    PosID = x.po.PosID,
                    Name = x.po.Name
                }).Distinct().ToListAsync();
            var list = new List<PositionInProject>();
            var empQuery = from ep in _context.EmpPositionInProjects
                           join e in _context.Employees on ep.EmpID equals e.Id
                           select new { ep, e };
            foreach (var pos in positions)
            {
                int count = 0;
                var employees = await empQuery.Where(x => x.ep.PosID == pos.PosID && x.ep.ProjectID.Equals(projectID))
                    .Select(x => new EmpInProject()
                    {
                        EmpID = x.e.Id,
                        Name = x.e.Name,
                        Email = x.e.Email,
                        PhoneNumber = x.e.PhoneNumber,
                        Status = x.e.Status,
                        DateIn = x.ep.DateIn
                    }).ToListAsync();
                if (employees.Count() != 0)
                {
                    foreach (var emp in employees)
                    {
                        var projects = _context.EmpPositionInProjects.Where(x => x.EmpID.Equals(emp.EmpID) && x.ProjectID != projectID)
                            .Select(x => new EmpPositionInProject()).ToList();
                        emp.NumberOfProject = projects.Count();
                        if (emp.DateIn != null)
                        {
                            count += 1;
                        }
                    }
                }
                var positionInProject = new PositionInProject()
                {
                    PosID = pos.PosID,
                    PosName = pos.Name,
                    Employees = employees,
                    Noe = count
                };
                var listCandidateNeeds = await _context.RequiredPositions.Where(x => x.PositionID.Equals(pos.PosID)
                && x.ProjectID.Equals(projectID)).Select(x => x.CandidateNeeded).ToListAsync();
                foreach (var candidateNeed in listCandidateNeeds)
                {
                    positionInProject.CandidateNeeded += candidateNeed;
                }
                list.Add(positionInProject);
            }
            return new ApiSuccessResult<List<PositionInProject>>(list);
        }

        public async Task<ApiResult<ListProjectViewModel>> GetProjectByEmpID(string empID, GetProjectPagingRequest request)
        {
            bool check = true;
            var projects = await _context.Projects.Where(x => x.ProjectManagerID.Equals(empID) && x.Status != ProjectStatus.Finished)
                .Select(x => new Project()
                {
                    ProjectID = x.ProjectID,
                    Status = x.Status
                }).ToListAsync();
            if (projects.Count() != 0)
            {
                foreach (var p in projects)
                {
                    if (p.Status == ProjectStatus.Pending)
                    {
                        var empInProject = _context.EmpPositionInProjects.Where(x => x.ProjectID.Equals(p.ProjectID))
                            .Select(x => new EmpPositionInProject()).ToList();
                        if (empInProject.Count() == 0)
                        {
                            var project = await _context.Projects.FindAsync(p.ProjectID);
                            project.Status = ProjectStatus.NoEmployee;
                            _context.Projects.Update(project);
                            var result = await _context.SaveChangesAsync();
                            if (result == 0)
                            {
                                return new ApiErrorResult<ListProjectViewModel>("Update project " + project.ProjectName + " failed");
                            }
                        }
                    }
                }
            }
            var query = from p in _context.Projects
                        join pt in _context.ProjectTypes on p.ProjectTypeID equals pt.ID
                        select new { p, pt };
            query = query.Where(x => x.p.ProjectManagerID.Equals(empID));
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.p.ProjectName.Contains(request.Keyword));
            }

            //Paging
            int totalRow = await query.CountAsync();

            var data = await query.OrderBy(x => x.p.Status)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProjectViewModel()
                {
                    ProjectID = x.p.ProjectID,
                    ProjectName = x.p.ProjectName,
                    Description = x.p.Description,
                    DateBegin = x.p.DateBegin,
                    DateEstimatedEnd = x.p.DateEstimatedEnd,
                    Status = x.p.Status,
                    TypeID = x.p.ProjectTypeID,
                    TypeName = x.pt.Name,
                    FieldID = x.p.ProjectFieldID,
                    DateEnd = x.p.DateEnd,
                    PmID = x.p.ProjectManagerID
                }).ToListAsync();
            foreach (var p in data)
            {
                p.FieldName = _context.ProjectFields.Find(p.FieldID).Name;
            }
            //Select and projection
            var pagedResult = new PagedResult<ProjectViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };
            var listProjectViewModel = new ListProjectViewModel()
            {
                IsCreateNew = check,
                data = pagedResult
            };
            return new ApiSuccessResult<ListProjectViewModel>(listProjectViewModel);
        }

        public async Task<ApiResult<PagedResult<AdminProjectViewModel>>> GetProjectPaging(GetProjectPagingRequest request)
        {
            var list = await _context.Projects.Select(x => new Project()
            {
                ProjectID = x.ProjectID,
                Status = x.Status
            }).ToListAsync();
            foreach (var p in list)
            {
                if (p.Status == ProjectStatus.Pending)
                {
                    var empInProject = _context.EmpPositionInProjects.Where(x => x.ProjectID.Equals(p.ProjectID))
                        .Select(x => new EmpPositionInProject()).ToList();
                    if (empInProject.Count() == 0)
                    {
                        var project = await _context.Projects.FindAsync(p.ProjectID);
                        project.Status = ProjectStatus.NoEmployee;
                        _context.Projects.Update(project);
                        var result = await _context.SaveChangesAsync();
                        if (result == 0)
                        {
                            return new ApiErrorResult<PagedResult<AdminProjectViewModel>>("Update project " + project.ProjectName + " failed");
                        }
                    }
                }
            }
            var query = from pt in _context.ProjectTypes
                        join p in _context.Projects on pt.ID equals p.ProjectTypeID
                        join e in _context.Employees on p.ProjectManagerID equals e.Id
                        select new { pt, p, e };
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.p.ProjectName.Contains(request.Keyword));
            }

            //Paging
            int totalRow = await query.CountAsync();

            var data = await query.OrderBy(x => x.p.Status)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AdminProjectViewModel()
                {
                    ProjectID = x.p.ProjectID,
                    ProjectName = x.p.ProjectName,
                    Description = x.p.Description,
                    DateBegin = x.p.DateBegin,
                    DateEstimatedEnd = x.p.DateEstimatedEnd,
                    DateCreated = x.p.DateCreated,
                    DateEnd = x.p.DateEnd,
                    Status = x.p.Status,
                    EmpID = x.p.ProjectManagerID,
                    Name = x.e.Name,
                    TypeID = x.p.ProjectTypeID,
                    TypeName = x.pt.Name,
                    FieldID = x.p.ProjectFieldID,
                    IsAddNewCandidate = false
                }).ToListAsync();
            foreach (var p in data)
            {
                p.FieldName = _context.ProjectFields.Find(p.FieldID).Name;
                if (p.Status == ProjectStatus.OnGoing || p.Status == ProjectStatus.Confirmed)
                {
                    var listEmp = await _context.EmpPositionInProjects.Where(x => x.ProjectID.Equals(p.ProjectID) && x.DateIn == null)
                        .Select(x => new EmpPositionInProject()).ToListAsync();
                    if (listEmp.Count() != 0)
                    {
                        p.IsAddNewCandidate = true;
                    }
                }
            }

            //Select and projection
            var pagedResult = new PagedResult<AdminProjectViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return new ApiSuccessResult<PagedResult<AdminProjectViewModel>>(pagedResult);
        }

        public async Task<ApiResult<bool>> Update(int projectID, ProjectUpdateRequest request)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) return new ApiErrorResult<bool>("Project does not exist");

            project.Description = request.Description;
            if (DateTime.Compare(project.DateEstimatedEnd.Date, request.DateEstimatedEnd.Date) != 0)
            {
                var projects = await _context.Projects.Where(x => x.ProjectManagerID.Equals(project.ProjectManagerID) && x.Status != ProjectStatus.Finished)
                    .OrderBy(x => x.DateEstimatedEnd).Select(x => new Project()
                    {
                        DateBegin = x.DateBegin,
                        DateEstimatedEnd = x.DateEstimatedEnd
                    }).ToListAsync();
                for (int i = 0; i < projects.Count(); i++)
                {
                    if (DateTime.Compare(projects[i].DateEstimatedEnd.Date, project.DateEstimatedEnd.Date) == 0)
                    {
                        if (i != (projects.Count() - 1))
                        {
                            if (DateTime.Compare(request.DateEstimatedEnd.Date, projects[i + 1].DateBegin.AddDays(-5)) > 0)
                            {
                                return new ApiErrorResult<bool>("Date Estimated End must be before your next project's begin date(" + projects[i + 1].DateBegin.ToString("dd/MM/yyyy") + ") at least 5 days");
                            }
                        }
                    }
                }
                if (DateTime.Compare(project.DateBegin.Date, request.DateEstimatedEnd.Date) > 0)
                {
                    return new ApiErrorResult<bool>("Date estimated end is earlier than project's begin date");
                }
                project.DateEstimatedEnd = request.DateEstimatedEnd;
            }
            project.ProjectTypeID = request.TypeID;
            project.ProjectFieldID = request.FieldID;
            _context.Projects.Update(project);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update project failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<List<RequiredPositionDetail>>> AddRequiredPosition(int projectID, AddRequiredPositionRequest request)
        {
            foreach (var position in request.RequiredPositions)
            {
                var requiredPosition = new RequiredPosition()
                {
                    PositionID = position.PosID,
                    ProjectID = projectID,
                    CandidateNeeded = position.CandidateNeeded,
                    MissingEmployee = position.CandidateNeeded,
                    DateCreated = DateTime.Now
                };
                _context.RequiredPositions.Add(requiredPosition);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ApiErrorResult<List<RequiredPositionDetail>>("Add RequiredPosition failed");
                }
                position.RequiredPosID = requiredPosition.ID;
                if (position.Language.Count() != 0)
                {
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
                    result = await _context.SaveChangesAsync();
                    if (result == 0)
                    {
                        return new ApiErrorResult<List<RequiredPositionDetail>>("Add RequiredLanguage failed");
                    }
                }
                RequiredSkill requiredSkill;
                if (position.SoftSkillIDs.Count() != 0)
                {
                    foreach (var softSkill in position.SoftSkillIDs)
                    {
                        var skill = await _context.Skills.FindAsync(softSkill);
                        if (skill == null) return new ApiErrorResult<List<RequiredPositionDetail>>("SoftSkill not found");
                        if (skill.Status == false) return new ApiErrorResult<List<RequiredPositionDetail>>("SoftSkill:" + skill.SkillName + " is disable");
                        requiredSkill = new RequiredSkill()
                        {
                            RequiredPositionID = requiredPosition.ID,
                            SkillID = softSkill
                        };
                        _context.RequiredSkills.Add(requiredSkill);
                    }
                    result = await _context.SaveChangesAsync();
                    if (result == 0)
                    {
                        return new ApiErrorResult<List<RequiredPositionDetail>>("Add RequiredSoftSkill failed");
                    }
                }
                if (position.HardSkills.Count() != 0)
                {
                    foreach (var hardSkill in position.HardSkills)
                    {
                        var skill = await _context.Skills.FindAsync(hardSkill.HardSkillID);
                        if (skill == null) return new ApiErrorResult<List<RequiredPositionDetail>>("HardSkill not found");
                        if (skill.Status == false) return new ApiErrorResult<List<RequiredPositionDetail>>("HardSkill:" + skill.SkillName + " is disable");
                        requiredSkill = new RequiredSkill()
                        {
                            RequiredPositionID = requiredPosition.ID,
                            SkillID = hardSkill.HardSkillID,
                            Priority = hardSkill.Priority,
                            SkillLevel = (SkillLevel)hardSkill.SkillLevel,
                            CertificationLevel = hardSkill.CertificationLevel
                        };
                        _context.RequiredSkills.Add(requiredSkill);
                    }
                    result = await _context.SaveChangesAsync();
                    if (result == 0)
                    {
                        return new ApiErrorResult<List<RequiredPositionDetail>>("Add RequiredHardSkill failed");
                    }
                }
            }
            return new ApiSuccessResult<List<RequiredPositionDetail>>(request.RequiredPositions);
        }

        public async Task<ApiResult<int>> ChangeStatus(int projectID)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) return new ApiErrorResult<int>("Project does not exist");

            project.Status = ProjectStatus.Finished;
            project.DateEnd = DateTime.Now;
            _context.Projects.Update(project);
            var empInProject = await _context.EmpPositionInProjects.Where(x => x.ProjectID.Equals(projectID)).Select(x => new EmpPositionInProject()
            {
                EmpID = x.EmpID,
                PosID = x.PosID,
                ProjectID = x.ProjectID,
                DateIn = x.DateIn
            }).ToListAsync();
            foreach (var emp in empInProject)
            {
                if (emp.DateIn != null)
                {
                    emp.DateOut = DateTime.Now;
                    _context.EmpPositionInProjects.Update(emp);
                }
                else
                {
                    _context.EmpPositionInProjects.Remove(emp);
                }
            }
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<int>("Update project failed");
            }
            return new ApiSuccessResult<int>((int)project.Status);
        }

        public async Task<ApiResult<bool>> AddCandidate(int projectID, AddCandidateRequest request)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) return new ApiErrorResult<bool>("Project does not exist");
            if (project.Status == ProjectStatus.NoEmployee)
            {
                project.Status = ProjectStatus.Pending;
            }
            _context.Projects.Update(project);
            foreach (var candidate in request.Candidates)
            {
                var pos = await _context.Positions.FindAsync(candidate.PosID);
                if (pos == null) return new ApiErrorResult<bool>("Position not found");
                if (pos.Status == false) return new ApiErrorResult<bool>("Position:" + pos.Name + " is disable");
                var requiredPos = await _context.RequiredPositions.FindAsync(candidate.RequiredPosID);
                if (candidate.EmpIDs.Count() != 0)
                {
                    var count = 0;
                    var checkRequired = await _context.RequiredPositions.Where(x => x.ProjectID.Equals(projectID)
                    && x.PositionID.Equals(candidate.PosID)).OrderByDescending(x => x.DateCreated).Select(x => x.CandidateNeeded).ToListAsync();
                    foreach (var cn in checkRequired)
                    {
                        count += cn;
                    }
                    var employees = await _context.EmpPositionInProjects.Where(x => x.ProjectID.Equals(projectID)
                    && x.PosID.Equals(candidate.PosID)).Select(x => x.EmpID).ToListAsync();
                    count -= employees.Count();
                    if (candidate.EmpIDs.Count() > count)
                    {
                        var message = "Can only add equal or less than " + count + " employee for position " + pos.Name;
                        return new ApiErrorResult<bool>(message);
                    }
                    foreach (var emp in candidate.EmpIDs)
                    {
                        var checkEmp = _context.EmpPositionInProjects.Where(x => x.EmpID.Equals(emp) && x.DateIn != null && x.DateOut == null && x.ProjectID != projectID)
                            .Select(x => x.EmpID).FirstOrDefault();
                        if (checkEmp != null)
                        {
                            var empName = _context.Employees.Find(emp).Name;
                            return new ApiErrorResult<bool>("Employee:" + empName + " already in other project");
                        }
                        var employee = _context.EmpPositionInProjects.Where(x => x.EmpID.Equals(emp) && x.ProjectID.Equals(projectID))
                            .Select(x => new EmpPositionInProject()
                            {
                                EmpID = emp,
                                ProjectID = projectID,
                                PosID = x.PosID,
                                DateIn = x.DateIn,
                                DateOut = x.DateOut
                            }).FirstOrDefault();
                        if (employee != null)
                        {
                            if (employee.DateOut == null)
                            {
                                var empName = _context.Employees.Find(employee.EmpID).Name;
                                return new ApiErrorResult<bool>("Employee:" + empName + " already added in this project");
                            }
                        }
                        else
                        {
                            employee = new EmpPositionInProject()
                            {
                                EmpID = emp,
                                PosID = candidate.PosID,
                                ProjectID = projectID,
                                RequiredPositionID = candidate.RequiredPosID
                            };
                            _context.EmpPositionInProjects.Add(employee);
                        }
                    }
                    requiredPos.Status = RequirementStatus.Waiting;
                }
                else
                {
                    requiredPos.Status = RequirementStatus.Finished;
                }
                _context.RequiredPositions.Update(requiredPos);
            }
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("add candidate failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<List<string>>> ConfirmCandidate(int projectID, ConfirmCandidateRequest request)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) return new ApiErrorResult<List<string>>("Project does not exist");
            List<string> listAddedEmp = new List<string>();
            List<string> listRemovedEmp = new List<string>();
            List<string> listEmp = new List<string>();
            string removedMessage = "These employee already removed in this project: ";
            string addedMessage = "These employee already added in this project: ";
            string message = "These employee already in other project: ";
            foreach (var position in request.Candidates)
            {
                var pos = await _context.Positions.FindAsync(position.PosID);
                if (pos == null) return new ApiErrorResult<List<string>>("Position not found");
                if (pos.Status == false) return new ApiErrorResult<List<string>>("Position:" + pos.Name + " is disable");
                var requiredPos = await _context.RequiredPositions.FindAsync(position.RequiredPosID);
                foreach (var id in position.EmpIDs)
                {
                    var employee = await _context.Employees.FindAsync(id.EmpID);
                    var empInProject = await _context.EmpPositionInProjects.Where(x => x.EmpID.Equals(id.EmpID)
                    && x.PosID.Equals(position.PosID) && x.ProjectID.Equals(projectID)).Select(x => new EmpPositionInProject()
                    {
                        EmpID = x.EmpID,
                        PosID = x.PosID,
                        ProjectID = x.ProjectID,
                        DateIn = x.DateIn,
                        DateOut = x.DateOut,
                        RequiredPositionID = x.RequiredPositionID
                    }).FirstOrDefaultAsync();
                    if (id.IsAccept)
                    {
                        var checkEmp = await _context.EmpPositionInProjects.Where(x => x.EmpID.Equals(id.EmpID)
                        && x.DateIn != null && x.DateOut == null && x.ProjectID != projectID)
                            .Select(x => x.EmpID).FirstOrDefaultAsync();
                        if (checkEmp != null)
                        {
                            listEmp.Add(id.EmpID);
                            message += employee.Name + ",";
                        }
                        else
                        {
                            if (empInProject == null)
                            {
                                listRemovedEmp.Add(id.EmpID);
                                removedMessage += employee.Name + ",";
                            }
                            else
                            {
                                if (empInProject.DateIn == null)
                                {
                                    empInProject.DateIn = DateTime.Now;
                                    requiredPos.MissingEmployee -= 1;
                                    _context.EmpPositionInProjects.Update(empInProject);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (empInProject.DateIn != null)
                        {
                            listAddedEmp.Add(id.EmpID);
                            addedMessage += employee.Name + ",";
                        }
                        else
                        {
                            _context.EmpPositionInProjects.Remove(empInProject);
                            var rejectedEmp = new RejectedEmployee()
                            {
                                EmpID = id.EmpID,
                                RequiredPositionID = position.RequiredPosID,
                                Note = id.Note
                            };
                            _context.RejectedEmployees.Add(rejectedEmp);
                        }
                    }
                }
                if (listEmp.Count() != 0)
                {
                    return new ApiResult<List<string>>()
                    {
                        IsSuccessed = false,
                        Message = message,
                        ResultObj = listEmp
                    };
                }
                if (listRemovedEmp.Count() != 0)
                {
                    return new ApiResult<List<string>>()
                    {
                        IsSuccessed = false,
                        Message = removedMessage,
                        ResultObj = listRemovedEmp
                    };
                }
                if (listAddedEmp.Count() != 0)
                {
                    return new ApiResult<List<string>>()
                    {
                        IsSuccessed = false,
                        Message = addedMessage,
                        ResultObj = listAddedEmp
                    };
                }
                requiredPos.Status = RequirementStatus.Finished;
                _context.RequiredPositions.Update(requiredPos);
            }
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<List<string>>("confirm candidate failed");
            }
            return new ApiSuccessResult<List<string>>();
        }

        public async Task<ApiResult<PagedResult<EmployeeProjectViewModel>>> GetEmployeeProjects(string empID, GetProjectPagingRequest request)
        {
            var query = from p in _context.Projects
                        join ep in _context.EmpPositionInProjects on p.ProjectID equals ep.ProjectID
                        join po in _context.Positions on ep.PosID equals po.PosID
                        select new { p, ep, po };
            query = query.Where(x => x.ep.EmpID.Equals(empID));
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.p.ProjectName.Contains(request.Keyword));
            }

            //Paging
            int totalRow = await query.CountAsync();

            var data = await query.OrderByDescending(x => x.p.DateCreated)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new EmployeeProjectViewModel()
                {
                    ProjectID = x.p.ProjectID,
                    ProjectName = x.p.ProjectName,
                    PosName = x.po.Name,
                    DateIn = x.ep.DateIn
                }).ToListAsync();

            //Select and projection
            var pagedResult = new PagedResult<EmployeeProjectViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<EmployeeProjectViewModel>>(pagedResult);
        }

        public async Task<ApiResult<List<ProjectTypeViewModel>>> GetProjectTypes()
        {
            var data = await _context.ProjectTypes.Select(x => new ProjectTypeViewModel()
            {
                ID = x.ID,
                Name = x.Name
            }).ToListAsync();
            if (data.Count == 0)
            {
                return new ApiErrorResult<List<ProjectTypeViewModel>>("Project's type not found");
            }
            return new ApiSuccessResult<List<ProjectTypeViewModel>>(data);
        }

        public async Task<ApiResult<string>> CheckStatus(AddRequiredPositionRequest request)
        {
            string message;
            foreach (var pos in request.RequiredPositions)
            {
                var position = await _context.Positions.FindAsync(pos.PosID);
                if (position == null) return new ApiErrorResult<string>("position not found");
                if (!position.Status)
                {
                    message = "Position " + position.Name + " is not enable";
                    return new ApiErrorResult<string>(message);
                }
                if (pos.SoftSkillIDs != null)
                {
                    foreach (var id in pos.SoftSkillIDs)
                    {
                        var skill = await _context.Skills.FindAsync(id);
                        if (!skill.Status)
                        {
                            message = "Skill " + skill.SkillName + " is not enable";
                            return new ApiErrorResult<string>(message);
                        }
                    }
                }
                foreach (var hardSkill in pos.HardSkills)
                {
                    var skill = await _context.Skills.FindAsync(hardSkill.HardSkillID);
                    if (!skill.Status)
                    {
                        message = "Skill " + skill.SkillName + " is not enable";
                        return new ApiErrorResult<string>(message);
                    }
                }
            }
            message = "Success!";
            return new ApiSuccessResult<string>(message);
        }

        public async Task<ApiResult<StatisticViewModel>> GetStatistics()
        {
            var projectByTypes = new List<ProjectByType>();
            var employeeByProjects = new List<EmployeeByProject>();
            var employeeByPositions = new List<EmployeeByPosition>();
            var employeeByHardSkills = new List<EmployeeByHardSkill>();
            var projectByStatuses = new List<ProjectByStatus>();
            var listStatus = Enum.GetValues(typeof(ProjectStatus));
            var projectTypes = await _context.ProjectTypes.Select(x => new ProjectType()
            {
                ID = x.ID,
                Name = x.Name
            }).ToListAsync();
            foreach (var type in projectTypes)
            {
                var listProject = await _context.Projects.Where(x => x.ProjectTypeID.Equals(type.ID))
                    .Select(x => new Project()).ToListAsync();
                var projectByType = new ProjectByType()
                {
                    Name = type.Name,
                    Nop = listProject.Count()
                };
                projectByTypes.Add(projectByType);
            }
            var projects = await _context.Projects.Select(x => new Project()
            {
                ProjectID = x.ProjectID,
                ProjectName = x.ProjectName
            }).ToListAsync();
            foreach (var p in projects)
            {
                var listEmp = await _context.EmpPositionInProjects.Where(x => x.ProjectID.Equals(p.ProjectID))
                    .Select(x => new EmpPositionInProject()).ToListAsync();
                var employeeByProject = new EmployeeByProject()
                {
                    Name = p.ProjectName,
                    Noe = listEmp.Count()
                };
                employeeByProjects.Add(employeeByProject);
            }
            var positions = await _context.Positions.Select(x => new Position()
            {
                PosID = x.PosID,
                Name = x.Name,
            }).ToListAsync();
            foreach (var p in positions)
            {
                var listEmp = await _context.EmpPositions.Where(x => x.PosID.Equals(p.PosID))
                    .Select(x => new EmpPosition()).ToListAsync();
                var employeeByPosition = new EmployeeByPosition()
                {
                    Name = p.Name,
                    Noe = listEmp.Count()
                };
                employeeByPositions.Add(employeeByPosition);
            }
            var topPositions = (from po in employeeByPositions
                                orderby po.Noe descending
                                select po).Take(10).ToList();
            var hardSkills = await _context.Skills.Where(x => x.SkillType.Equals(SkillType.HardSkill))
                .Select(x => new Skill()
                {
                    SkillID = x.SkillID,
                    SkillName = x.SkillName
                }).ToListAsync();
            foreach (var s in hardSkills)
            {
                var listEmp = await _context.EmpSkills.Where(x => x.SkillID.Equals(s.SkillID))
                    .Select(x => new EmpSkill()).ToListAsync();
                var employeeByHardSkill = new EmployeeByHardSkill()
                {
                    Name = s.SkillName,
                    Noe = listEmp.Count()
                };
                employeeByHardSkills.Add(employeeByHardSkill);
            }
            var topHardSkills = (from hs in employeeByHardSkills
                                 orderby hs.Noe descending
                                 select hs).Take(10).ToList();
            foreach (var status in listStatus)
            {
                var listProject = await _context.Projects.Where(x => x.Status.Equals((ProjectStatus)status)).Select(x => new Project()).ToListAsync();
                ProjectByStatus projectByStatus = new ProjectByStatus()
                {
                    Status = (int)status,
                    Nop = listProject.Count()
                };
                projectByStatuses.Add(projectByStatus);
            }
            var statisticVM = new StatisticViewModel()
            {
                ProjectByTypes = projectByTypes,
                EmployeeByProjects = employeeByProjects,
                EmployeeByPositions = topPositions,
                EmployeeByHardSkills = topHardSkills,
                ProjectByStatuses = projectByStatuses
            };
            return new ApiSuccessResult<StatisticViewModel>(statisticVM);
        }

        public async Task<ApiResult<List<PositionInProject>>> GetCandidates(int projectID)
        {
            var query = from rp in _context.RequiredPositions
                        join po in _context.Positions on rp.PositionID equals po.PosID
                        select new { rp, po };
            var positions = await query.Where(x => x.rp.ProjectID.Equals(projectID) && x.rp.Status.Equals(RequirementStatus.Waiting))
                .Select(x => new PositionInProject()
                {
                    RequiredPosID = x.rp.ID,
                    PosID = x.po.PosID,
                    PosName = x.po.Name
                }).ToListAsync();
            var empQuery = from ep in _context.EmpPositionInProjects
                           join e in _context.Employees on ep.EmpID equals e.Id
                           select new { ep, e };
            foreach (var pos in positions)
            {
                pos.Employees = await empQuery.Where(x => x.ep.PosID == pos.PosID && x.ep.DateIn == null && x.ep.ProjectID.Equals(projectID))
                    .Select(x => new EmpInProject()
                    {
                        EmpID = x.e.Id,
                        Name = x.e.Name,
                        Email = x.e.Email,
                        PhoneNumber = x.e.PhoneNumber,
                        Status = x.e.Status,
                        DateIn = x.ep.DateIn
                    }).ToListAsync();
                if (pos.Employees.Count() != 0)
                {
                    foreach (var emp in pos.Employees)
                    {
                        var projects = _context.EmpPositionInProjects.Where(x => x.EmpID.Equals(emp.EmpID) && x.ProjectID != projectID)
                            .Select(x => new EmpPositionInProject()).ToList();
                        emp.NumberOfProject = projects.Count();
                        if (emp.DateIn != null)
                        {
                            pos.Noe += 1;
                        }
                    }
                }
                var listCandidateNeeds = await _context.RequiredPositions.Where(x => x.PositionID.Equals(pos.PosID)
                && x.ProjectID.Equals(projectID)).Select(x => x.CandidateNeeded).ToListAsync();
                foreach (var candidateNeed in listCandidateNeeds)
                {
                    pos.CandidateNeeded += candidateNeed;
                }
            }
            return new ApiSuccessResult<List<PositionInProject>>(positions);
        }

        public async Task<ApiResult<List<PosInProject>>> GetStatisticsByEmpID(string empID)
        {
            var posInProjects = new List<PosInProject>();
            var listPosLevel = Enum.GetValues(typeof(PositionLevel));
            var listProject = await _context.Projects.Where(x => x.ProjectManagerID.Equals(empID))
                .Select(x => new Project()
                {
                    ProjectID = x.ProjectID,
                    ProjectName = x.ProjectName,
                    DateCreated = x.DateCreated,
                    DateEnd = x.DateEnd
                }).ToListAsync();
            var query = from rp in _context.RequiredPositions
                        join po in _context.Positions on rp.PositionID equals po.PosID
                        select new { rp, po };
            foreach (var p in listProject)
            {
                var listEmpByPos = new List<EmployeeByPosition>();
                var listEmpByPosLevel = new List<EmployeeByPosLevel>();
                foreach (var level in listPosLevel)
                {
                    var empByPosLevel = new EmployeeByPosLevel()
                    {
                        PositionLevel = (int)level,
                        Noe = 0
                    };
                    listEmpByPosLevel.Add(empByPosLevel);
                }
                var listPosition = await query.Where(x => x.rp.ProjectID.Equals(p.ProjectID))
                    .Select(x => new Position()
                    {
                        PosID = x.rp.PositionID,
                        Name = x.po.Name
                    }).Distinct().ToListAsync();
                foreach (var pos in listPosition)
                {
                    var listEmp = await _context.EmpPositionInProjects.Where(x => x.ProjectID.Equals(p.ProjectID) && x.PosID.Equals(pos.PosID))
                        .Select(x => x.EmpID).ToListAsync();
                    var empByPos = new EmployeeByPosition()
                    {
                        Name = pos.Name,
                        Noe = listEmp.Count()
                    };
                    listEmpByPos.Add(empByPos);
                    foreach (var emp in listEmp)
                    {
                        var empPos = await _context.EmpPositions.FindAsync(emp, pos.PosID);
                        if (empPos != null)
                        {
                            foreach (var level in listEmpByPosLevel)
                            {
                                if (empPos.PositionLevel.Equals((PositionLevel)level.PositionLevel))
                                {
                                    level.Noe += 1;
                                }
                            }
                        }
                        else
                        {
                            listEmpByPosLevel[0].Noe += 1;
                        }
                    }
                }
                var posInProject = new PosInProject()
                {
                    Name = p.ProjectName,
                    DateCreated = p.DateCreated,
                    DateEnd = p.DateEnd,
                    EmployeeByPositions = listEmpByPos,
                    EmployeeByPosLevels = listEmpByPosLevel
                };
                posInProjects.Add(posInProject);
            }
            return new ApiSuccessResult<List<PosInProject>>(posInProjects);
        }

        public async Task<ApiResult<List<RequiredPositionVM>>> GetRequiredPositions(int projectID)
        {
            var query = from rp in _context.RequiredPositions
                        join po in _context.Positions on rp.PositionID equals po.PosID
                        select new { rp, po };
            var skillQuery = from rs in _context.RequiredSkills
                             join s in _context.Skills on rs.SkillID equals s.SkillID
                             select new { rs, s };
            var languageQuery = from l in _context.Languages
                                join rl in _context.RequiredLanguages on l.LangID equals rl.LangID
                                select new { l, rl };
            var positions = await query.Where(x => x.rp.ProjectID.Equals(projectID)).OrderByDescending(x => x.rp.DateCreated)
                .Select(x => new RequiredPositionVM()
                {
                    RequiredPosID = x.rp.ID,
                    PosID = x.rp.PositionID,
                    PosName = x.po.Name,
                    CandidateNeeded = x.rp.CandidateNeeded,
                    DateCreated = x.rp.DateCreated,
                    MissingEmployee = x.rp.MissingEmployee,
                    Status = x.rp.Status
                }).ToListAsync();
            if (positions.Count() == 0)
            {
                return new ApiErrorResult<List<RequiredPositionVM>>("This project doesn't have any required position");
            }
            foreach (var p in positions)
            {
                p.Language = await languageQuery.Where(x => x.rl.RequiredPositionID.Equals(p.RequiredPosID))
                    .Select(x => new RequiredLanguageVM()
                    {
                        LangID = x.rl.LangID,
                        LangName = x.l.LangName,
                        Priority = x.rl.Priority
                    }).ToListAsync();

                p.SoftSkillIDs = await skillQuery.Where(x => x.rs.RequiredPositionID.Equals(p.RequiredPosID)
                && x.s.SkillType.Equals(SkillType.SoftSkill)).Select(x => new RequiredSoftSkillVM()
                {
                    SoftSkillID = x.rs.SkillID,
                    SoftSkillName = x.s.SkillName
                }).ToListAsync();

                p.HardSkills = await skillQuery.Where(x => x.rs.RequiredPositionID.Equals(p.RequiredPosID)
                && x.s.SkillType.Equals(SkillType.HardSkill)).Select(x => new RequiredHardSkillVM()
                {
                    HardSkillID = x.rs.SkillID,
                    HardSkillName = x.s.SkillName,
                    SkillLevel = (int)x.rs.SkillLevel,
                    CertificationLevel = (int)x.rs.CertificationLevel,
                    Priority = (int)x.rs.Priority
                }).ToListAsync();
            }
            return new ApiSuccessResult<List<RequiredPositionVM>>(positions);
        }

        public async Task<ApiResult<List<ProjectFieldViewModel>>> GetProjectFields()
        {
            var projectFields = await _context.ProjectFields.Select(x => new ProjectFieldViewModel()
            {
                ID = x.ID,
                Name = x.Name
            }).ToListAsync();
            if (projectFields.Count() == 0)
            {
                return new ApiErrorResult<List<ProjectFieldViewModel>>("Project's field not found");
            }
            return new ApiSuccessResult<List<ProjectFieldViewModel>>(projectFields);
        }

        public async Task<ApiResult<RequiredPositionVM>> GetRequiredPosByID(int projectID, int posID)
        {
            var query = from rp in _context.RequiredPositions
                        join po in _context.Positions on rp.PositionID equals po.PosID
                        select new { rp, po };
            var skillQuery = from rs in _context.RequiredSkills
                             join s in _context.Skills on rs.SkillID equals s.SkillID
                             select new { rs, s };
            var languageQuery = from l in _context.Languages
                                join rl in _context.RequiredLanguages on l.LangID equals rl.LangID
                                select new { l, rl };
            var requiredPos = await query.Where(x => x.rp.ProjectID.Equals(projectID) && x.rp.PositionID.Equals(posID)).OrderByDescending(x => x.rp.DateCreated)
                .Select(x => new RequiredPositionVM()
                {
                    RequiredPosID = x.rp.ID,
                    PosID = x.rp.PositionID,
                    PosName = x.po.Name,
                    CandidateNeeded = x.rp.CandidateNeeded,
                    DateCreated = x.rp.DateCreated,
                    MissingEmployee = x.rp.MissingEmployee,
                    Status = x.rp.Status
                }).FirstOrDefaultAsync();
            if (requiredPos == null)
            {
                return new ApiErrorResult<RequiredPositionVM>("This project doesn't have any required position");
            }
            requiredPos.Language = await languageQuery.Where(x => x.rl.RequiredPositionID.Equals(requiredPos.RequiredPosID))
                    .Select(x => new RequiredLanguageVM()
                    {
                        LangID = x.rl.LangID,
                        LangName = x.l.LangName,
                        Priority = x.rl.Priority
                    }).ToListAsync();

            requiredPos.SoftSkillIDs = await skillQuery.Where(x => x.rs.RequiredPositionID.Equals(requiredPos.RequiredPosID)
            && x.s.SkillType.Equals(SkillType.SoftSkill)).Select(x => new RequiredSoftSkillVM()
            {
                SoftSkillID = x.rs.SkillID,
                SoftSkillName = x.s.SkillName
            }).ToListAsync();

            requiredPos.HardSkills = await skillQuery.Where(x => x.rs.RequiredPositionID.Equals(requiredPos.RequiredPosID)
            && x.s.SkillType.Equals(SkillType.HardSkill)).Select(x => new RequiredHardSkillVM()
            {
                HardSkillID = x.rs.SkillID,
                HardSkillName = x.s.SkillName,
                SkillLevel = (int)x.rs.SkillLevel,
                CertificationLevel = (int)x.rs.CertificationLevel,
                Priority = (int)x.rs.Priority
            }).ToListAsync();

            return new ApiSuccessResult<RequiredPositionVM>(requiredPos);
        }

        public async Task<ApiResult<bool>> CheckProject()
        {
            var projects = await _context.Projects.Where(x => x.Status.Equals(ProjectStatus.Confirmed))
                .Select(x => new Project()
                {
                    ProjectID = x.ProjectID,
                    ProjectName = x.ProjectName,
                    Description = x.Description,
                    DateBegin = x.DateBegin,
                    DateEstimatedEnd = x.DateEstimatedEnd,
                    Status = x.Status,
                    DateEnd = x.DateEnd,
                    DateCreated = x.DateCreated,
                    ProjectFieldID = x.ProjectFieldID,
                    ProjectTypeID = x.ProjectTypeID,
                    ProjectManagerID = x.ProjectManagerID,
                    EmailStatus = x.EmailStatus
                }).ToListAsync();
            if (projects.Count() != 0)
            {
                foreach (var p in projects)
                {
                    if (DateTime.Compare(p.DateBegin.Date, DateTime.Today) == 0)
                    {
                        p.Status = ProjectStatus.OnGoing;
                        _context.Projects.Update(p);
                    }
                }
                var result = await _context.SaveChangesAsync();
            }
            return new ApiSuccessResult<bool>();
        }
    }
}