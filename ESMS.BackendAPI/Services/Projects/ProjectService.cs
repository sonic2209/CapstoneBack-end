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
                        return new ApiErrorResult<int>("This project name is existed");
                    }
                    project.ProjectName = request.ProjectName;
                }
                project.Description = request.Description;
                project.Skateholder = request.Skateholder;
                var checkDate = _context.Projects.Where(x => x.ProjectManagerID.Equals(empID) && x.Status != ProjectStatus.Finished)
                    .OrderByDescending(x => x.DateEstimatedEnd)
                    .Select(x => x.DateEstimatedEnd).FirstOrDefault();
                if (DateTime.Compare(request.DateBegin, checkDate.AddDays(3)) < 0)
                {
                    return new ApiErrorResult<int>("Date begin must be after your current project's estimated end date(" + checkDate.ToShortDateString() + ") at least 3 days");
                }
                project.DateBegin = request.DateBegin;
                if (DateTime.Compare(request.DateBegin, request.DateEstimatedEnd) > 0)
                {
                    return new ApiErrorResult<int>("Date estimated end is earlier than date begin");
                }
                project.DateEstimatedEnd = request.DateEstimatedEnd;
                project.ProjectTypeID = request.ProjectTypeID;
                _context.Projects.Update(project);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ApiErrorResult<int>("Update project failed");
                }
            }
            else
            {
                var projects = _context.Projects.Where(x => x.ProjectManagerID.Equals(empID) && x.Status != ProjectStatus.Finished)
                    .Select(x => new Project()).ToList();
                if (projects.Count() >= 5)
                {
                    return new ApiErrorResult<int>("Cannot create more project");
                }
                var checkName = _context.Projects.Where(x => x.ProjectName.Equals(request.ProjectName))
                    .Select(x => new Project()).FirstOrDefault();
                if (checkName != null)
                {
                    return new ApiErrorResult<int>("This project name is existed");
                }
                var checkDate = _context.Projects.Where(x => x.ProjectManagerID.Equals(empID) && x.Status != ProjectStatus.Finished)
                    .OrderByDescending(x => x.DateEstimatedEnd)
                    .Select(x => x.DateEstimatedEnd).FirstOrDefault();
                if (DateTime.Compare(request.DateBegin, checkDate.AddDays(3)) < 0)
                {
                    return new ApiErrorResult<int>("Date begin must be after your current project's estimated end date(" + checkDate.ToShortDateString() + ") at least 3 days");
                }
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
                    ProjectManagerID = empID,
                    ProjectTypeID = request.ProjectTypeID
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
                Skateholder = x.p.Skateholder,
                DateBegin = x.p.DateBegin,
                DateEstimatedEnd = x.p.DateEstimatedEnd,
                Status = x.p.Status,
                TypeID = x.p.ProjectTypeID,
                TypeName = x.pt.Name,
                DateEnd = x.p.DateEnd,
                PmID = x.p.ProjectManagerID
            }).FirstOrDefaultAsync();
            if (projectVM == null) return new ApiErrorResult<ProjectViewModel>("Project does not exist");
            return new ApiSuccessResult<ProjectViewModel>(projectVM);
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
                    Email = x.e.Email,
                    PhoneNumber = x.e.PhoneNumber,
                    Status = x.e.Status,
                    DateIn = x.ep.DateIn
                }).ToListAsync();
                foreach (var emp in employees)
                {
                    var projects = _context.EmpPositionInProjects.Where(x => x.EmpID.Equals(emp.EmpID) && x.ProjectID != projectID)
                        .Select(x => new EmpPositionInProject()).ToList();
                    emp.NumberOfProject = projects.Count();
                }
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
                if (projects.Count() >= 5)
                {
                    check = false;
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
                    Skateholder = x.p.Skateholder,
                    DateBegin = x.p.DateBegin,
                    DateEstimatedEnd = x.p.DateEstimatedEnd,
                    Status = x.p.Status,
                    TypeID = x.p.ProjectTypeID,
                    TypeName = x.pt.Name,
                    DateEnd = x.p.DateEnd,
                    PmID = x.p.ProjectManagerID
                }).ToListAsync();

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
                    Skateholder = x.p.Skateholder,
                    DateBegin = x.p.DateBegin,
                    DateEstimatedEnd = x.p.DateEstimatedEnd,
                    DateCreated = x.p.DateCreated,
                    DateEnd = x.p.DateEnd,
                    Status = x.p.Status,
                    EmpID = x.p.ProjectManagerID,
                    Name = x.e.Name,
                    TypeID = x.p.ProjectTypeID,
                    TypeName = x.pt.Name,
                    IsAddNewCandidate = false
                }).ToListAsync();
            foreach (var p in data)
            {
                if (p.Status == ProjectStatus.OnGoing)
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
            project.Skateholder = request.Skateholder;
            project.DateEstimatedEnd = request.DateEstimatedEnd;
            project.ProjectTypeID = request.TypeID;

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
                var list = new List<int>();
                foreach (var poslevel in position.PosLevel)
                {
                    var requiredPosition = new RequiredPosition()
                    {
                        PositionID = position.PosID,
                        ProjectID = projectID,
                        PositionLevel = (PositionLevel)poslevel
                    };
                    _context.RequiredPositions.Add(requiredPosition);
                    var result = await _context.SaveChangesAsync();
                    if (result == 0)
                    {
                        return new ApiErrorResult<bool>("Create requiredPosition failed");
                    }
                    list.Add(requiredPosition.ID);
                }
                foreach (var id in list)
                {
                    foreach (var language in position.Language)
                    {
                        var requiredLanguage = new RequiredLanguage()
                        {
                            LangID = language.LangID,
                            RequiredPositionID = id,
                            Priority = language.Priority
                        };
                        _context.RequiredLanguages.Add(requiredLanguage);
                    }
                    RequiredSkill requiredSkill;
                    if (position.SoftSkillIDs != null)
                    {
                        foreach (var softSkill in position.SoftSkillIDs)
                        {
                            requiredSkill = new RequiredSkill()
                            {
                                RequiredPositionID = id,
                                SkillID = softSkill
                            };
                            _context.RequiredSkills.Add(requiredSkill);
                        }
                    }
                    foreach (var hardSkill in position.HardSkills)
                    {
                        requiredSkill = new RequiredSkill()
                        {
                            RequiredPositionID = id,
                            SkillID = hardSkill.HardSkillID,
                            Priority = hardSkill.Priority,
                            SkillLevel = (SkillLevel)hardSkill.SkillLevel,
                            CertificationLevel = hardSkill.CertificationLevel
                        };
                        _context.RequiredSkills.Add(requiredSkill);
                    }
                    var result = await _context.SaveChangesAsync();
                    if (result == 0)
                    {
                        return new ApiErrorResult<bool>("Create requiredSkill failed");
                    }
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
                    var employee = await _context.Employees.FindAsync(emp.EmpID);
                    employee.Status = EmployeeStatus.Pending;
                    _context.Employees.Update(employee);
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
                _context.Projects.Update(project);
            }
            foreach (var candidate in request.Candidates)
            {
                var pos = await _context.Positions.FindAsync(candidate.PosID);
                if (pos == null) return new ApiErrorResult<bool>("Position not found");
                if (pos.Status == false) return new ApiErrorResult<bool>("Position:" + pos.Name + " is disable");
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
                            ProjectID = projectID
                        };
                        _context.EmpPositionInProjects.Add(employee);
                    }
                }
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
            List<string> listEmp = new List<string>();
            string message = "";
            if (request.IsAccept)
            {
                message = "These employee already removed in this project: ";
                foreach (var position in request.Candidates)
                {
                    var pos = await _context.Positions.FindAsync(position.PosID);
                    if (pos == null) return new ApiErrorResult<List<string>>("Position not found");
                    if (pos.Status == false) return new ApiErrorResult<List<string>>("Position:" + pos.Name + " is disable");
                    foreach (var id in position.EmpIDs)
                    {
                        var employee = _context.Employees.Find(id);
                        var empInProject = _context.EmpPositionInProjects.Where(x => x.EmpID.Equals(id)
                        && x.PosID.Equals(position.PosID) && x.ProjectID.Equals(projectID)).Select(x => new EmpPositionInProject()
                        {
                            EmpID = x.EmpID,
                            PosID = x.PosID,
                            ProjectID = x.ProjectID,
                            DateIn = x.DateIn
                        }).FirstOrDefault();
                        if (empInProject == null)
                        {
                            listEmp.Add(id);
                            message += employee.Name + ",";
                        }
                        else
                        {
                            empInProject.DateIn = DateTime.Now;
                            _context.EmpPositionInProjects.Update(empInProject);
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
                }
                if (DateTime.Compare(DateTime.Today, project.DateBegin) >= 0)
                {
                    project.Status = ProjectStatus.OnGoing;
                }
                else
                {
                    project.Status = ProjectStatus.Confirmed;
                }
                _context.Projects.Update(project);
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ApiErrorResult<List<string>>("confirm candidate failed");
                }
            }
            else
            {
                message = "These employee already added in this project: ";
                foreach (var position in request.Candidates)
                {
                    var pos = await _context.Positions.FindAsync(position.PosID);
                    if (pos == null) return new ApiErrorResult<List<string>>("Position not found");
                    if (pos.Status == false) return new ApiErrorResult<List<string>>("Position:" + pos.Name + " is disable");
                    foreach (var id in position.EmpIDs)
                    {
                        var employee = _context.Employees.Find(id);
                        var empInProject = _context.EmpPositionInProjects.Where(x => x.EmpID.Equals(id)
                        && x.PosID.Equals(position.PosID) && x.ProjectID.Equals(projectID)).Select(x => new EmpPositionInProject()
                        {
                            EmpID = x.EmpID,
                            PosID = x.PosID,
                            ProjectID = x.ProjectID,
                            DateIn = x.DateIn
                        }).FirstOrDefault();
                        if (empInProject.DateIn != null)
                        {
                            listEmp.Add(id);
                            message += employee.Name + ",";
                        }
                        else
                        {
                            _context.EmpPositionInProjects.Remove(empInProject);
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
                    var result = await _context.SaveChangesAsync();
                    if (result == 0)
                    {
                        return new ApiErrorResult<List<string>>("confirm candidate failed");
                    }
                }
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
                var employees = await query.Where(x => x.ep.PosID == pos.PosID && x.ep.DateIn == null).Select(x => new EmpInProject()
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
                    }
                    var positionInProject = new PositionInProject()
                    {
                        PosID = pos.PosID,
                        PosName = pos.Name,
                        Employees = employees
                    };
                    list.Add(positionInProject);
                }
            }
            return new ApiSuccessResult<List<PositionInProject>>(list);
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
                        foreach (var level in listEmpByPosLevel)
                        {
                            if (empPos.PositionLevel.Equals((PositionLevel)level.PositionLevel))
                            {
                                level.Noe += 1;
                            }
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
    }
}