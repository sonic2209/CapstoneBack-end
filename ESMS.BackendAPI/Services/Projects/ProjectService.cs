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
using ESMS.BackendAPI.Ultilities;

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
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            bool checkProjectName = false;
            bool checkDateBegin = false;
            bool checkDateEnd = false;
            var project = await _context.Projects.FindAsync(request.ProjectID);
            var checkText = new string(request.ProjectName.Where(c => !Char.IsWhiteSpace(c)).ToArray());
            DateTime dateBegin = DateTime.Parse(request.DateBegin);
            DateTime dateEstimatedEnd = DateTime.Parse(request.DateEstimatedEnd);
            if (project != null)
            {
                if (checkText.All(char.IsDigit))
                {
                    UltilitiesService.AddOrUpdateError(errors, "ProjectName", "Name can not be digits only");
                    checkProjectName = true;
                }
                if (!project.ProjectName.Equals(request.ProjectName))
                {
                    var checkName = _context.Projects.Where(x => x.ProjectName.Equals(request.ProjectName)).Select(x => new Project()).FirstOrDefault();
                    if (checkName != null)
                    {
                        if (checkProjectName == false)
                        {
                            UltilitiesService.AddOrUpdateError(errors, "ProjectName", "This name already exist");
                        }
                        //return new ApiErrorResult<int>("projectName : This project name already exist");
                    }
                }
                var checkProjectDate = await _context.Projects.Where(x => x.ProjectManagerID.Equals(empID) && x.Status != ProjectStatus.Finished
                && x.ProjectID != project.ProjectID).OrderBy(x => x.DateEstimatedEnd)
                .Select(x => new Project()
                {
                    DateBegin = x.DateBegin,
                    DateEstimatedEnd = x.DateEstimatedEnd
                }).ToListAsync();
                var check = true;
                if (DateTime.Compare(project.DateBegin, dateBegin) != 0 || DateTime.Compare(project.DateEstimatedEnd, dateEstimatedEnd) != 0)
                {
                    if (DateTime.Compare(dateBegin, DateTime.Today.AddMonths(1)) < 0)
                    {
                        UltilitiesService.AddOrUpdateError(errors, "dateBegin", "Start date must be after today at least 1 month");
                        checkDateBegin = true;
                        //return new ApiErrorResult<int>("dateBegin : Start date must be after today at least 1 month");
                    }

                    if (DateTime.Compare(dateBegin, dateEstimatedEnd.AddDays(-30)) > 0)
                    {
                        UltilitiesService.AddOrUpdateError(errors, "dateEstimatedEnd", "Estimated end date must be after start date at least 30 days");
                        checkDateEnd = true;
                        //return new ApiErrorResult<int>("dateEstimatedEnd : Estimated End Date must be after date begin at least 30 days");
                    }
                    if (checkProjectDate.Count() != 0)
                    {
                        for (int i = 0; i < checkProjectDate.Count(); i++)
                        {
                            if (DateTime.Compare(checkProjectDate[i].DateBegin.Date, dateBegin.Date) == 0)
                            {
                                if (checkDateBegin == false)
                                {
                                    UltilitiesService.AddOrUpdateError(errors, "dateBegin", "This is other project's start date");
                                    checkDateBegin = true;
                                }
                                break;
                            }
                            if (DateTime.Compare(checkProjectDate[i].DateBegin, dateBegin) > 0)
                            {
                                check = false;
                            }
                            if (check == false)
                            {
                                if (i == 0)
                                {
                                    if (DateTime.Compare(dateEstimatedEnd, checkProjectDate[i].DateBegin.AddDays(-7)) > 0)
                                    {
                                        if (checkDateEnd == false)
                                        {
                                            UltilitiesService.AddOrUpdateError(errors, "dateEstimatedEnd", "Estimated end date must be before your next project's start date(" + checkProjectDate[i].DateBegin.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                            checkDateEnd = true;
                                        }
                                        //return new ApiErrorResult<int>("dateEstimatedEnd : Estimated End Date must be before your next project's begin date(" + checkProjectDate[i].DateBegin.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                    }
                                }
                                if (i > 0)
                                {
                                    if (DateTime.Compare(dateBegin, checkProjectDate[i - 1].DateEstimatedEnd.AddDays(7)) < 0)
                                    {
                                        if (checkDateBegin == false)
                                        {
                                            UltilitiesService.AddOrUpdateError(errors, "dateBegin", "Start date must be after your previous project's estimated end date(" + checkProjectDate[i - 1].DateEstimatedEnd.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                            checkDateBegin = true;
                                        }
                                        //return new ApiErrorResult<int>("dateBegin : Start date must be after your previous project's estimated end date(" + checkProjectDate[i - 1].DateEstimatedEnd.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                    }
                                    if (DateTime.Compare(dateEstimatedEnd, checkProjectDate[i].DateBegin.AddDays(-7)) > 0)
                                    {
                                        if (checkDateEnd == false)
                                        {
                                            UltilitiesService.AddOrUpdateError(errors, "dateEstimatedEnd", "Estimated end date must be before your next project's start date(" + checkProjectDate[i].DateBegin.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                            checkDateEnd = true;
                                        }
                                        //return new ApiErrorResult<int>("dateEstimatedEnd : Estimated End Date must be before your next project's begin date(" + checkProjectDate[i].DateBegin.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                    }
                                }
                                break;
                            }
                        }
                        if (check == true)
                        {
                            if (DateTime.Compare(project.DateBegin, dateBegin) != 0)
                            {
                                if (DateTime.Compare(dateBegin, checkProjectDate[checkProjectDate.Count() - 1].DateEstimatedEnd.AddDays(7)) < 0)
                                {
                                    if (checkDateBegin == false)
                                    {
                                        UltilitiesService.AddOrUpdateError(errors, "dateBegin", "Start date must be after your previous project's estimated end date(" + checkProjectDate[checkProjectDate.Count() - 1].DateEstimatedEnd.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                        checkDateBegin = true;
                                    }
                                    //return new ApiErrorResult<int>("dateBegin : Start date must be after your previous project's estimated end date(" + checkProjectDate[checkProjectDate.Count() - 1].DateEstimatedEnd.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                }
                            }
                        }
                    }
                }
                if (errors.Count() > 0)
                {
                    return new ApiErrorResult<int>(errors);
                }
                project.ProjectName = request.ProjectName;
                project.Description = request.Description;
                project.DateBegin = dateBegin;
                project.DateEstimatedEnd = dateEstimatedEnd;
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
                if (checkText.All(char.IsDigit))
                {
                    UltilitiesService.AddOrUpdateError(errors, "ProjectName", "Name can not be digits only");
                    checkProjectName = true;
                }
                var checkName = _context.Projects.Where(x => x.ProjectName.Equals(request.ProjectName))
                    .Select(x => new Project()).FirstOrDefault();
                if (checkName != null)
                {
                    if (checkProjectName == false)
                    {
                        UltilitiesService.AddOrUpdateError(errors, "ProjectName", "This name already exist");
                    }
                    //return new ApiErrorResult<int>("projectName : This project name already exist");
                }
                var checkProjectDate = await _context.Projects.Where(x => x.ProjectManagerID.Equals(empID) && x.Status != ProjectStatus.Finished)
                    .OrderBy(x => x.DateEstimatedEnd).Select(x => new Project()
                    {
                        DateBegin = x.DateBegin,
                        DateEstimatedEnd = x.DateEstimatedEnd
                    }).ToListAsync();
                var check = true;
                if (DateTime.Compare(dateBegin, DateTime.Today.AddMonths(1)) < 0)
                {
                    if (checkDateBegin == false)
                    {
                        UltilitiesService.AddOrUpdateError(errors, "dateBegin", "Start date must be after today at least 1 month");
                        checkDateBegin = true;
                    }
                    //return new ApiErrorResult<int>("dateBegin : Start date must be after today at least 1 month");
                }

                if (DateTime.Compare(dateBegin, dateEstimatedEnd.AddDays(-30)) > 0)
                {
                    if (checkDateEnd == false)
                    {
                        UltilitiesService.AddOrUpdateError(errors, "dateEstimatedEnd", "Estimated end date must be after start date at least 30 days");
                        checkDateEnd = true;
                    }
                    //return new ApiErrorResult<int>("dateEstimatedEnd : Estimated End Date must be after date begin at least 30 days");
                }
                if (checkProjectDate.Count() != 0)
                {
                    for (int i = 0; i < checkProjectDate.Count(); i++)
                    {
                        if (DateTime.Compare(checkProjectDate[i].DateBegin, dateBegin) == 0)
                        {
                            if (checkDateBegin == false)
                            {
                                UltilitiesService.AddOrUpdateError(errors, "dateBegin", "This is other project's start date");
                                checkDateBegin = true;
                            }
                            break;
                        }
                        if (DateTime.Compare(checkProjectDate[i].DateBegin, dateBegin) > 0)
                        {
                            check = false;
                        }
                        if (check == false)
                        {
                            if (i == 0)
                            {
                                if (DateTime.Compare(dateEstimatedEnd, checkProjectDate[i].DateBegin.AddDays(-7)) > 0)
                                {
                                    if (checkDateEnd == false)
                                    {
                                        UltilitiesService.AddOrUpdateError(errors, "dateEstimatedEnd", "Estimated end date must be before your next project's start date(" + checkProjectDate[i].DateBegin.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                        checkDateEnd = true;
                                    }
                                    //return new ApiErrorResult<int>("dateEstimatedEnd : Estimated End Date must be before your next project's begin date(" + checkProjectDate[i].DateBegin.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                }
                            }
                            if (i > 0)
                            {
                                if (DateTime.Compare(dateBegin, checkProjectDate[i - 1].DateEstimatedEnd.AddDays(7)) < 0)
                                {
                                    if (checkDateBegin == false)
                                    {
                                        UltilitiesService.AddOrUpdateError(errors, "dateBegin", "Start date must be after your previous project's estimated end date(" + checkProjectDate[i - 1].DateEstimatedEnd.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                        checkDateBegin = true;
                                    }
                                    //return new ApiErrorResult<int>("dateBegin : Start date must be after your previous project's estimated end date(" + checkProjectDate[i - 1].DateEstimatedEnd.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                }
                                if (DateTime.Compare(dateEstimatedEnd, checkProjectDate[i].DateBegin.AddDays(-7)) > 0)
                                {
                                    if (checkDateEnd == false)
                                    {
                                        UltilitiesService.AddOrUpdateError(errors, "dateEstimatedEnd", "Estimated end date must be before your next project's start date(" + checkProjectDate[i].DateBegin.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                        checkDateEnd = true;
                                    }
                                    //return new ApiErrorResult<int>("dateEstimatedEnd : Estimated End Date must be before your next project's begin date(" + checkProjectDate[i].DateBegin.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                }
                            }
                            break;
                        }
                    }
                    if (check == true)
                    {
                        if (DateTime.Compare(dateBegin, checkProjectDate[checkProjectDate.Count() - 1].DateEstimatedEnd.AddDays(7)) < 0)
                        {
                            if (checkDateBegin == false)
                            {
                                UltilitiesService.AddOrUpdateError(errors, "dateBegin", "Start date must be after your previous project's estimated end date(" + checkProjectDate[checkProjectDate.Count() - 1].DateEstimatedEnd.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                                checkDateBegin = true;
                            }
                            //return new ApiErrorResult<int>("dateBegin : Start date must be after your previous project's estimated end date(" + checkProjectDate[checkProjectDate.Count() - 1].DateEstimatedEnd.ToString("dd/MM/yyyy") + ") at least 1 weeks");
                        }
                    }
                }
                if (errors.Count() > 0)
                {
                    return new ApiErrorResult<int>(errors);
                }
                project = new Project()
                {
                    ProjectName = request.ProjectName,
                    Description = request.Description,
                    DateCreated = DateTime.Now,
                    DateBegin = dateBegin,
                    DateEstimatedEnd = dateEstimatedEnd,
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
                    var empInProject = await _context.EmpPositionInProjects.Where(x => x.RequiredPositionID.Equals(pos.ID))
                        .Select(x => new EmpPositionInProject()
                        {
                            EmpID = x.EmpID,
                            RequiredPositionID = x.RequiredPositionID,
                            DateIn = x.DateIn,
                            DateOut = x.DateOut,
                            Status = x.Status,
                            Note = x.Note
                        }).ToListAsync();
                    if (empInProject.Count() != 0)
                    {
                        foreach (var emp in empInProject)
                        {
                            _context.EmpPositionInProjects.Remove(emp);
                        }
                    }
                    _context.RequiredPositions.Remove(pos);
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
                DateCreated = x.p.DateCreated,
                Status = x.p.Status,
                TypeID = x.p.ProjectTypeID,
                TypeName = x.pt.Name,
                FieldID = x.p.ProjectFieldID,
                DateEnd = x.p.DateEnd,
                PmID = x.p.ProjectManagerID
            }).FirstOrDefaultAsync();
            if (projectVM == null) return new ApiErrorResult<ProjectViewModel>("Project does not exist");
            projectVM.PmName = _context.Employees.Find(projectVM.PmID).Name;
            projectVM.FieldName = _context.ProjectFields.Find(projectVM.FieldID).Name;
            var empQuery = from ep in _context.EmpPositionInProjects
                           join rp in _context.RequiredPositions on ep.RequiredPositionID equals rp.ID
                           select new { ep, rp };
            var employee = await empQuery.Where(x => x.rp.ProjectID.Equals(projectID)
            && x.ep.Status.Equals(ConfirmStatus.Accept)).Select(x => x.ep.EmpID).ToListAsync();
            projectVM.Noe = employee.Count();
            var missEmpInPos = await _context.RequiredPositions.Where(x => x.ProjectID.Equals(projectVM.ProjectID) && x.MissingEmployee > 0).FirstOrDefaultAsync();
            if (missEmpInPos != null)
            {
                projectVM.IsMissEmp = true;
            }
            return new ApiSuccessResult<ProjectViewModel>(projectVM);
        }

        public async Task<ApiResult<List<PositionInProject>>> GetEmpInProjectPaging(int projectID)
        {
            var query = from rp in _context.RequiredPositions
                        join po in _context.Positions on rp.PositionID equals po.PosID
                        select new { rp, po };
            var positions = await query.Where(x => x.rp.ProjectID.Equals(projectID))
                .Select(x => new PositionInProject()
                {
                    PosID = x.po.PosID,
                    PosName = x.po.Name,
                }).ToListAsync();
            var positionInProject = positions.GroupBy(x => new { x.PosID, x.PosName }).Select(x => x.FirstOrDefault()).ToList();
            var empQuery = from ep in _context.EmpPositionInProjects
                           join e in _context.Employees on ep.EmpID equals e.Id
                           select new { ep, e };
            foreach (var pos in positionInProject)
            {
                pos.Employees = new List<EmpInProject>();
                var listRequirePos = await _context.RequiredPositions.Where(x => x.ProjectID.Equals(projectID)
                && x.PositionID.Equals(pos.PosID)).Select(x => new RequiredPosition()
                {
                    ID = x.ID,
                    CandidateNeeded = x.CandidateNeeded
                }).ToListAsync();
                foreach (var requirePos in listRequirePos)
                {
                    var employees = await empQuery.Where(x => x.ep.RequiredPositionID.Equals(requirePos.ID)
                    && x.ep.Status != ConfirmStatus.Reject)
                        .Select(x => new EmpInProject()
                        {
                            EmpID = x.e.Id,
                            Name = x.e.Name,
                            Email = x.e.Email,
                            PhoneNumber = x.e.PhoneNumber,
                            Status = x.ep.Status,
                            DateIn = x.ep.DateIn
                        }).ToListAsync();
                    if (employees.Count() != 0)
                    {
                        foreach (var emp in employees)
                        {
                            pos.Employees.Add(emp);
                        }
                    }
                    var listEmp = await _context.EmpPositionInProjects.Where(x => x.RequiredPositionID.Equals(requirePos.ID)
                    && x.DateIn != null).Select(x => x.EmpID).ToListAsync();
                    pos.Noe += listEmp.Count();
                    pos.CandidateNeeded += requirePos.CandidateNeeded;
                }
            }
            return new ApiSuccessResult<List<PositionInProject>>(positionInProject);
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
            var empQuery = from ep in _context.EmpPositionInProjects
                           join rp in _context.RequiredPositions on ep.RequiredPositionID equals rp.ID
                           select new { ep, rp };
            if (projects.Count() != 0)
            {
                foreach (var p in projects)
                {
                    if (p.Status == ProjectStatus.Pending)
                    {
                        var empInProject = empQuery.Where(x => x.rp.ProjectID.Equals(p.ProjectID))
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

            var data = await query.OrderBy(x => x.p.Status).ThenByDescending(x => x.p.DateCreated)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProjectViewModel()
                {
                    ProjectID = x.p.ProjectID,
                    ProjectName = x.p.ProjectName,
                    Description = x.p.Description,
                    DateBegin = x.p.DateBegin,
                    DateEstimatedEnd = x.p.DateEstimatedEnd,
                    DateCreated = x.p.DateCreated,
                    Status = x.p.Status,
                    TypeID = x.p.ProjectTypeID,
                    TypeName = x.pt.Name,
                    FieldID = x.p.ProjectFieldID,
                    DateEnd = x.p.DateEnd,
                    PmID = x.p.ProjectManagerID
                }).ToListAsync();
            foreach (var p in data)
            {
                p.PmName = _context.Employees.Find(p.PmID).Name;
                p.FieldName = _context.ProjectFields.Find(p.FieldID).Name;
                var missEmpInPos = await _context.RequiredPositions.Where(x => x.ProjectID.Equals(p.ProjectID) && x.MissingEmployee > 0).FirstOrDefaultAsync();
                if (missEmpInPos != null)
                {
                    p.IsMissEmp = true;
                }
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
            var list = await _context.Projects.Where(x => x.Status != ProjectStatus.Finished).Select(x => new Project()
            {
                ProjectID = x.ProjectID,
                Status = x.Status
            }).ToListAsync();
            var empQuery = from ep in _context.EmpPositionInProjects
                           join rp in _context.RequiredPositions on ep.RequiredPositionID equals rp.ID
                           select new { ep, rp };
            foreach (var p in list)
            {
                if (p.Status == ProjectStatus.Pending)
                {
                    var empInProject = empQuery.Where(x => x.rp.ProjectID.Equals(p.ProjectID))
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

            var data = await query.OrderBy(x => x.p.Status).ThenByDescending(x => x.p.DateCreated)
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
                    IsAddNewCandidate = false,
                    IsMissEmp = false
                }).ToListAsync();
            foreach (var p in data)
            {
                p.FieldName = _context.ProjectFields.Find(p.FieldID).Name;
                if (p.Status == ProjectStatus.OnGoing || p.Status == ProjectStatus.Confirmed)
                {
                    var missEmpInPos = await _context.RequiredPositions.Where(x => x.ProjectID.Equals(p.ProjectID) && x.MissingEmployee > 0).FirstOrDefaultAsync();
                    if (missEmpInPos != null)
                    {
                        p.IsMissEmp = true;
                    }
                    var listEmp = await empQuery.Where(x => x.rp.ProjectID.Equals(p.ProjectID) && x.ep.Status.Equals(ConfirmStatus.New))
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
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) return new ApiErrorResult<bool>("Project does not exist");

            DateTime dateEstimatedEnd = DateTime.Parse(request.DateEstimatedEnd);
            if (request.Description.All(char.IsDigit))
            {
                UltilitiesService.AddOrUpdateError(errors, "Description", "Description can not be digits only");
            }

            if (DateTime.Compare(project.DateEstimatedEnd.Date, dateEstimatedEnd.Date) != 0)
            {
                if (DateTime.Compare(project.DateEstimatedEnd.Date, dateEstimatedEnd.Date) < 0)
                {
                    UltilitiesService.AddOrUpdateError(errors, "dateEstimatedEnd", "Estimated end date can not be delayed");
                    //return new ApiErrorResult<bool>("Estimated End Date cannot be delayed");
                }
                else if (DateTime.Compare(project.DateBegin.AddDays(30).Date, dateEstimatedEnd.Date) > 0)
                {
                    UltilitiesService.AddOrUpdateError(errors, "dateEstimatedEnd", "Estimated end date must be after start date at least 30 days");
                    //return new ApiErrorResult<bool>("Estimated End Date is earlier than project's begin date");
                }
                project.DateEstimatedEnd = dateEstimatedEnd;
                //var projects = await _context.Projects.Where(x => x.ProjectManagerID.Equals(project.ProjectManagerID) && x.Status != ProjectStatus.Finished)
                //    .OrderBy(x => x.DateEstimatedEnd).Select(x => new Project()
                //    {
                //        DateBegin = x.DateBegin,
                //        DateEstimatedEnd = x.DateEstimatedEnd
                //    }).ToListAsync();
                //for (int i = 0; i < projects.Count(); i++)
                //{
                //    if (DateTime.Compare(projects[i].DateEstimatedEnd.Date, project.DateEstimatedEnd.Date) == 0)
                //    {
                //        if (i != (projects.Count() - 1))
                //        {
                //            if (DateTime.Compare(dateEstimatedEnd.Date, projects[i + 1].DateBegin.AddDays(-5)) > 0)
                //            {
                //                return new ApiErrorResult<bool>("Date Estimated End must be before your next project's begin date(" + projects[i + 1].DateBegin.ToString("dd/MM/yyyy") + ") at least 5 days");
                //            }
                //        }
                //    }
                //}
            }
            if (errors.Count() > 0)
            {
                return new ApiErrorResult<bool>(errors);
            }
            project.Description = request.Description;
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

        private string CheckStatus(AddRequiredPositionRequest request)
        {
            string message = "";
            foreach (var pos in request.RequiredPositions)
            {
                if (pos.PosID == 0) return "Please select position";
                var position = _context.Positions.Find(pos.PosID);
                if (position == null) return "Position not found";
                if (!position.Status)
                {
                    message = "Position " + position.Name + " is not enable";
                    return message;
                }
                message = position.Name + " - ";
                string hardSkillMessage = "";
                string softSkillMessage = "";
                string languageMessage = "";
                if (pos.HardSkills.Count() == 0)
                {
                    hardSkillMessage = "Must have at least 1 hard skill";
                }
                else
                {
                    foreach (var hardSkill in pos.HardSkills)
                    {
                        if (hardSkill.HardSkillID == 0)
                        {
                            hardSkillMessage = "Please select hard skill";
                            break;
                        }
                        var skill = _context.Skills.Find(hardSkill.HardSkillID);
                        if (skill == null)
                        {
                            hardSkillMessage = "Hard skill not found";
                            break;
                        }
                        if (!skill.Status)
                        {
                            hardSkillMessage = "Skill " + skill.SkillName + " is not enable";
                            break;
                        }
                    }
                }

                if (pos.Language.Count() == 0)
                {
                    languageMessage = "Must have at least 1 language";
                }
                else
                {
                    foreach (var lang in pos.Language)
                    {
                        if (lang.LangID == 0)
                        {
                            languageMessage = "Please select language";
                            break;
                        }
                        var language = _context.Languages.Find(lang.LangID);
                        if (language == null)
                        {
                            languageMessage = "Language not found";
                            break;
                        }
                    }
                }

                if (pos.SoftSkillIDs.Count() == 0)
                {
                    softSkillMessage = "Must have at least 1 soft skill";
                }
                else
                {
                    foreach (var id in pos.SoftSkillIDs)
                    {
                        if (id == 0)
                        {
                            softSkillMessage = "Please select soft skill";
                            break;
                        }
                        var skill = _context.Skills.Find(id);
                        if (skill == null)
                        {
                            softSkillMessage = "Soft skill not found";
                            break;
                        }
                        if (!skill.Status)
                        {
                            softSkillMessage = "Skill " + skill.SkillName + " is not enable";
                            break;
                        }
                    }
                }
                if (!hardSkillMessage.Equals("") || !languageMessage.Equals("") || !softSkillMessage.Equals(""))
                {
                    string text = "";
                    if (!hardSkillMessage.Equals(""))
                    {
                        text += hardSkillMessage;
                        if (!languageMessage.Equals("") || !softSkillMessage.Equals(""))
                            text += ", ";
                    }
                    if (!languageMessage.Equals(""))
                    {
                        text += languageMessage;
                        if (!softSkillMessage.Equals(""))
                            text += ", ";
                    }
                    if (!softSkillMessage.Equals(""))
                    {
                        text += softSkillMessage;
                    }
                    message += text;
                    break;
                }
                else
                {
                    message = "";
                }
            }
            return message;
        }

        public async Task<ApiResult<List<RequiredPositionDetail>>> AddRequiredPosition(int projectID, AddRequiredPositionRequest request)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            string message = CheckStatus(request);
            if (!message.Equals(""))
            {
                UltilitiesService.AddOrUpdateError(errors, "RequiredPositions", message);
                return new ApiErrorResult<List<RequiredPositionDetail>>(errors);
            }
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
            var empQuery = from ep in _context.EmpPositionInProjects
                           join rp in _context.RequiredPositions on ep.RequiredPositionID equals rp.ID
                           select new { ep, rp };
            var empInProject = await empQuery.Where(x => x.rp.ProjectID.Equals(projectID)).Select(x => new EmpPositionInProject()
            {
                EmpID = x.ep.EmpID,
                RequiredPositionID = x.ep.RequiredPositionID,
                DateIn = x.ep.DateIn,
                DateOut = x.ep.DateOut,
                Status = x.ep.Status,
                Note = x.ep.Note
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
            var empQuery = from ep in _context.EmpPositionInProjects
                           join rp in _context.RequiredPositions on ep.RequiredPositionID equals rp.ID
                           select new { ep, rp };

            var projectquery = from p in _context.Projects
                               join rp in _context.RequiredPositions on p.ProjectID equals rp.ProjectID
                               join epip in _context.EmpPositionInProjects on rp.ID equals epip.RequiredPositionID
                               select new { rp, p, epip };
            if (request.Candidates.Count() != 0)
            {
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
                        var employees = await empQuery.Where(x => x.rp.ProjectID.Equals(projectID)
                        && x.rp.PositionID.Equals(pos.PosID) && x.ep.Status != ConfirmStatus.Reject).Select(x => x.ep.EmpID).Distinct().ToListAsync();
                        count -= employees.Count();
                        if (candidate.EmpIDs.Count() > count)
                        {
                            var message = "Can only add equal or less than " + count + " employee(s) for position " + pos.Name;
                            return new ApiErrorResult<bool>(message);
                        }
                        foreach (var emp in candidate.EmpIDs)
                        {
                            var check = true;
                            var empName = _context.Employees.Find(emp).Name;
                            var checkProjectDate = await projectquery.Where(x => x.p.Status != ProjectStatus.Finished
                            && x.epip.EmpID.Equals(emp) && x.rp.ProjectID != projectID
                            && x.epip.Status != ConfirmStatus.Reject)
                            .OrderBy(x => x.p.DateEstimatedEnd).Select(x => new Project()
                            {
                                DateBegin = x.p.DateBegin,
                                DateEstimatedEnd = x.p.DateEstimatedEnd
                            }).ToListAsync();
                            if (checkProjectDate.Count() != 0)
                            {
                                for (int i = 0; i < checkProjectDate.Count(); i++)
                                {
                                    if (DateTime.Compare(checkProjectDate[i].DateBegin, project.DateBegin) >= 0)
                                    {
                                        check = false;
                                    }
                                    if (check == false)
                                    {
                                        if (i == 0)
                                        {
                                            if (DateTime.Compare(project.DateEstimatedEnd, checkProjectDate[i].DateBegin.AddDays(-3)) > 0)
                                            {
                                                return new ApiErrorResult<bool>("Employee:" + empName + " are not avaliable for this project");
                                            }
                                        }
                                        if (i > 0)
                                        {
                                            if (DateTime.Compare(project.DateBegin, checkProjectDate[i - 1].DateEstimatedEnd.AddDays(3)) < 0)
                                            {
                                                return new ApiErrorResult<bool>("Employee:" + empName + " are not avaliable for this project");
                                            }
                                            if (DateTime.Compare(project.DateEstimatedEnd, checkProjectDate[i].DateBegin.AddDays(-3)) > 0)
                                            {
                                                return new ApiErrorResult<bool>("Employee:" + empName + " are not avaliable for this project");
                                            }
                                        }
                                        break;
                                    }
                                }
                                if (check == true)
                                {
                                    if (DateTime.Compare(project.DateBegin, checkProjectDate[checkProjectDate.Count() - 1].DateEstimatedEnd.AddDays(3)) < 0)
                                    {
                                        return new ApiErrorResult<bool>("Employee:" + empName + " are not avaliable for this project");
                                    }
                                }
                            }
                            //var listProjectCurrentlyIn = await projectquery.Where(x => x.p.ProjectID != projectID && x.p.Status != ProjectStatus.Finished && x.epip.EmpID.Equals(emp) && x.epip.Status != ConfirmStatus.Reject)
                            //    .Select(x => x.p.DateEstimatedEnd).ToListAsync();
                            //if (listProjectCurrentlyIn.Count() != 0)
                            //{
                            //    foreach (var date in listProjectCurrentlyIn)
                            //    {
                            //        if (DateTime.Compare(date.Date, project.DateBegin.Date) > 0)
                            //        {
                            //            var empName = _context.Employees.Find(emp).Name;
                            //            return new ApiErrorResult<bool>("Employee:" + empName + " are not avaliable for this project");
                            //        }
                            //    }
                            //}
                            //var checkEmp = await empQuery.Where(x => x.ep.EmpID.Equals(emp) && x.ep.DateIn != null
                            //&& x.ep.DateOut == null && x.rp.ProjectID != projectID).Select(x => x.ep.EmpID).FirstOrDefaultAsync();
                            //if (checkEmp != null)
                            //{
                            //    var empName = _context.Employees.Find(emp).Name;
                            //    return new ApiErrorResult<bool>("Employee:" + empName + " already in other project");
                            //}
                            var employee = await empQuery.Where(x => x.ep.EmpID.Equals(emp) && x.rp.ProjectID.Equals(projectID)
                            && x.ep.Status != ConfirmStatus.Reject)
                                .Select(x => new EmpPositionInProject()
                                {
                                    EmpID = emp,
                                    RequiredPositionID = x.ep.RequiredPositionID,
                                    DateIn = x.ep.DateIn,
                                    DateOut = x.ep.DateOut,
                                    Status = x.ep.Status,
                                    Note = x.ep.Note
                                }).FirstOrDefaultAsync();
                            if (employee != null)
                            {
                                if (employee.DateOut == null)
                                {
                                    return new ApiErrorResult<bool>("Employee:" + empName + " already added in this project");
                                }
                            }
                            else
                            {
                                var checkEmp = await _context.EmpPositionInProjects.FindAsync(emp, candidate.RequiredPosID);
                                if (checkEmp != null)
                                {
                                    checkEmp.Status = ConfirmStatus.New;
                                    checkEmp.Note = null;
                                    _context.EmpPositionInProjects.Update(checkEmp);
                                }
                                else
                                {
                                    employee = new EmpPositionInProject()
                                    {
                                        EmpID = emp,
                                        RequiredPositionID = candidate.RequiredPosID
                                    };
                                    _context.EmpPositionInProjects.Add(employee);
                                }
                            }
                        }
                        requiredPos.Status = RequirementStatus.Waiting;
                    }
                    _context.RequiredPositions.Update(requiredPos);
                }
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ApiErrorResult<bool>("add candidate failed");
                }
            }
            var listRequirePos = await _context.RequiredPositions.Where(x => x.ProjectID.Equals(projectID)
            && x.Status == 0).Select(x => new RequiredPosition()
            {
                ID = x.ID,
                PositionID = x.PositionID,
                ProjectID = x.ProjectID,
                CandidateNeeded = x.CandidateNeeded,
                DateCreated = x.DateCreated,
                MissingEmployee = x.MissingEmployee,
                Status = x.Status
            }).ToListAsync();
            if (listRequirePos.Count() != 0)
            {
                foreach (var rp in listRequirePos)
                {
                    rp.Status = RequirementStatus.Finished;
                    _context.RequiredPositions.Update(rp);
                }
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ApiErrorResult<bool>("add candidate failed");
                }
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<List<string>>> ConfirmCandidate(int projectID, ConfirmCandidateRequest request)
        {
            var project = await _context.Projects.FindAsync(projectID);
            if (project == null) return new ApiErrorResult<List<string>>("Project does not exist");
            var projectquery = from p in _context.Projects
                               join rp in _context.RequiredPositions on p.ProjectID equals rp.ProjectID
                               join epip in _context.EmpPositionInProjects on rp.ID equals epip.RequiredPositionID
                               select new { rp, p, epip };
            var empQuery = from ep in _context.EmpPositionInProjects
                           join rp in _context.RequiredPositions on ep.RequiredPositionID equals rp.ID
                           select new { ep, rp };
            List<string> listNotAvalEmp = new List<string>();
            List<string> listAddedEmp = new List<string>();
            List<string> listEmp = new List<string>();
            string notAvalMessage = "These employee are not avaliable for this project: ";
            string addedMessage = "These employee already in this project: ";
            //string message = "These employee already in other project: ";
            foreach (var position in request.Candidates)
            {
                var pos = await _context.Positions.FindAsync(position.PosID);
                if (pos == null) return new ApiErrorResult<List<string>>("Position not found");
                if (pos.Status == false) return new ApiErrorResult<List<string>>("Position:" + pos.Name + " is disable");
                var requiredPos = await _context.RequiredPositions.FindAsync(position.RequiredPosID);
                foreach (var id in position.EmpIDs)
                {
                    var employee = await _context.Employees.FindAsync(id.EmpID);
                    var empInPos = await _context.EmpPositionInProjects.FindAsync(id.EmpID, position.RequiredPosID);
                    if (id.IsAccept)
                    {
                        bool checkDate = false;
                        var check = true;
                        var empName = _context.Employees.Find(id.EmpID).Name;
                        var checkProjectDate = await projectquery.Where(x => x.p.Status != ProjectStatus.Finished
                        && x.epip.EmpID.Equals(id.EmpID) && x.rp.ProjectID != projectID
                        && x.epip.Status != ConfirmStatus.Reject)
                        .OrderBy(x => x.p.DateEstimatedEnd).Select(x => new Project()
                        {
                            DateBegin = x.p.DateBegin,
                            DateEstimatedEnd = x.p.DateEstimatedEnd
                        }).ToListAsync();
                        if (checkProjectDate.Count() != 0)
                        {
                            for (int i = 0; i < checkProjectDate.Count(); i++)
                            {
                                if (DateTime.Compare(checkProjectDate[i].DateBegin, project.DateBegin) >= 0)
                                {
                                    check = false;
                                }
                                if (check == false)
                                {
                                    if (i == 0)
                                    {
                                        if (DateTime.Compare(project.DateEstimatedEnd, checkProjectDate[i].DateBegin.AddDays(-3)) > 0)
                                        {
                                            checkDate = true;
                                            break;
                                        }
                                    }
                                    if (i > 0)
                                    {
                                        if (DateTime.Compare(project.DateBegin, checkProjectDate[i - 1].DateEstimatedEnd.AddDays(3)) < 0)
                                        {
                                            checkDate = true;
                                            break;
                                        }
                                        if (DateTime.Compare(project.DateEstimatedEnd, checkProjectDate[i].DateBegin.AddDays(-3)) > 0)
                                        {
                                            checkDate = true;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            if (check == true)
                            {
                                if (DateTime.Compare(project.DateBegin, checkProjectDate[checkProjectDate.Count() - 1].DateEstimatedEnd.AddDays(3)) < 0)
                                {
                                    checkDate = true;
                                }
                            }
                        }
                        //var projectquery = from p in _context.Projects
                        //                   join rp in _context.RequiredPositions on p.ProjectID equals rp.ProjectID
                        //                   join epip in _context.EmpPositionInProjects on rp.ID equals epip.RequiredPositionID
                        //                   select new { rp, p, epip };
                        //var listProjectCurrentlyIn = await projectquery.Where(x => x.p.ProjectID != projectID && x.p.Status != ProjectStatus.Finished && x.epip.EmpID.Equals(id.EmpID) && x.epip.Status != ConfirmStatus.Reject)
                        //        .Select(x => x.p.DateEstimatedEnd).ToListAsync();
                        //if (listProjectCurrentlyIn.Count() != 0)
                        //{
                        //    foreach (var date in listProjectCurrentlyIn)
                        //    {
                        //        if (DateTime.Compare(date.Date, project.DateBegin.Date) > 0)
                        //        {
                        //            checkDate = true;
                        //            break;
                        //        }
                        //    }
                        //}

                        //var checkEmp = await empQuery.Where(x => x.rp.ProjectID != projectID
                        //&& x.ep.Status.Equals(ConfirmStatus.Accept)).Select(x => new EmpPositionInProject()
                        //{
                        //    EmpID = x.ep.EmpID,
                        //    DateOut = x.ep.DateOut
                        //}).FirstOrDefaultAsync();
                        //if (checkEmp != null)
                        //{
                        //    if (checkEmp.DateOut == null)
                        //    {
                        //        listEmp.Add(id.EmpID);
                        //        message += employee.Name + ",";
                        //    }
                        //}
                        if (checkDate == true)
                        {
                            listNotAvalEmp.Add(id.EmpID);
                            notAvalMessage += employee.Name + ",";
                        }
                        else
                        {
                            var empInOtherPos = await empQuery.Where(x => x.rp.ProjectID.Equals(projectID) && x.ep.RequiredPositionID != position.RequiredPosID
                            && x.ep.EmpID.Equals(id.EmpID) && x.ep.Status.Equals(ConfirmStatus.Accept) && x.ep.DateOut == null)
                                .Select(x => new EmpPositionInProject()).FirstOrDefaultAsync();
                            if (empInOtherPos == null)
                            {
                                if (empInPos == null)
                                {
                                    empInPos = new EmpPositionInProject()
                                    {
                                        EmpID = id.EmpID,
                                        RequiredPositionID = position.RequiredPosID,
                                        DateIn = DateTime.Now,
                                        Status = ConfirmStatus.Accept
                                    };
                                    _context.EmpPositionInProjects.Add(empInPos);
                                    requiredPos.MissingEmployee -= 1;
                                }
                                else
                                {
                                    if (empInPos.Status != ConfirmStatus.Accept)
                                    {
                                        empInPos.DateIn = DateTime.Now;
                                        requiredPos.MissingEmployee -= 1;
                                        empInPos.Note = null;
                                    }
                                    empInPos.Status = ConfirmStatus.Accept;
                                    _context.EmpPositionInProjects.Update(empInPos);
                                }
                            }
                            else
                            {
                                listAddedEmp.Add(id.EmpID);
                                addedMessage += employee.Name + ",";
                            }
                        }
                    }
                    else
                    {
                        empInPos.Status = ConfirmStatus.Reject;
                        empInPos.Note = id.Note;
                        _context.EmpPositionInProjects.Update(empInPos);
                    }
                }
                if (listNotAvalEmp.Count() != 0)
                {
                    return new ApiResult<List<string>>()
                    {
                        IsSuccessed = false,
                        Message = notAvalMessage,
                        ResultObj = listNotAvalEmp
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
            project.Status = ProjectStatus.Confirmed;
            _context.Projects.Update(project);
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
                        join rp in _context.RequiredPositions on p.ProjectID equals rp.ProjectID
                        join ep in _context.EmpPositionInProjects on rp.ID equals ep.RequiredPositionID
                        select new { p, rp, ep };
            query = query.Where(x => x.ep.EmpID.Equals(empID) && x.ep.Status == ConfirmStatus.Accept);
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
                    DateIn = x.ep.DateIn
                }).ToListAsync();
            var posQuery = from po in _context.Positions
                           join rp in _context.RequiredPositions on po.PosID equals rp.PositionID
                           join ep in _context.EmpPositionInProjects on rp.ID equals ep.RequiredPositionID
                           select new { po, rp, ep };
            foreach (var project in data)
            {
                var position = await posQuery.Where(x => x.ep.EmpID.Equals(empID) && x.rp.ProjectID.Equals(project.ProjectID)
                && x.ep.Status.Equals(ConfirmStatus.Accept) && x.ep.DateIn.Equals(project.DateIn)).Select(x => x.po.Name).FirstOrDefaultAsync();
                project.PosName = position;
            }
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

        public async Task<ApiResult<List<CandidateInProject>>> GetCandidates(int projectID)
        {
            var query = from rp in _context.RequiredPositions
                        join po in _context.Positions on rp.PositionID equals po.PosID
                        select new { rp, po };
            var positions = await query.Where(x => x.rp.ProjectID.Equals(projectID) && x.rp.Status.Equals(RequirementStatus.Waiting))
                .Select(x => new CandidateInProject()
                {
                    RequiredPosID = x.rp.ID,
                    PosID = x.po.PosID,
                    PosName = x.po.Name,
                    CandidateNeeded = x.rp.CandidateNeeded
                }).ToListAsync();
            var empQuery = from ep in _context.EmpPositionInProjects
                           join e in _context.Employees on ep.EmpID equals e.Id
                           select new { ep, e };
            foreach (var pos in positions)
            {
                pos.Employees = await empQuery.Where(x => x.ep.RequiredPositionID.Equals(pos.RequiredPosID)
                && x.ep.Status.Equals(ConfirmStatus.New)).Select(x => new EmpInProject()
                {
                    EmpID = x.e.Id,
                    Name = x.e.Name,
                    Email = x.e.Email,
                    PhoneNumber = x.e.PhoneNumber,
                    Status = x.ep.Status,
                    DateIn = x.ep.DateIn
                }).ToListAsync();
                if (pos.Employees.Count() != 0)
                {
                    var projectQuery = from p in _context.Projects
                                       join rp in _context.RequiredPositions on p.ProjectID equals rp.ProjectID
                                       join ep in _context.EmpPositionInProjects on rp.ID equals ep.RequiredPositionID
                                       select new { p, rp, ep };
                    foreach (var emp in pos.Employees)
                    {
                        var projects = projectQuery.Where(x => x.ep.EmpID.Equals(emp.EmpID) && x.rp.ProjectID != projectID)
                            .Select(x => x.p.ProjectID).Distinct().ToList();
                        emp.NumberOfProject = projects.Count();
                    }
                }
            }
            return new ApiSuccessResult<List<CandidateInProject>>(positions);
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

        public List<ProjectVM> GetMissEmpProjects(string empID)
        {
            List<ProjectVM> list = new List<ProjectVM>();
            var projects = _context.Projects.Where(x => x.Status != ProjectStatus.Finished)
                .Select(x => new Project()
                {
                    ProjectID = x.ProjectID,
                    ProjectName = x.ProjectName,
                    ProjectFieldID = x.ProjectFieldID,
                    ProjectTypeID = x.ProjectTypeID,
                    ProjectManagerID = x.ProjectManagerID
                }).ToList();
            if (projects.Count() != 0)
            {
                var posQuery = from rp in _context.RequiredPositions
                               join po in _context.Positions on rp.PositionID equals po.PosID
                               select new { rp, po };
                foreach (var p in projects)
                {
                    var requirePos = posQuery.Where(x => x.rp.MissingEmployee > 0 && x.rp.Status != RequirementStatus.Waiting && x.rp.ProjectID.Equals(p.ProjectID))
                        .Select(x => new RequiredPosVM()
                        {
                            RequiredPosID = x.rp.ID,
                            PosID = x.rp.PositionID,
                            PosName = x.po.Name,
                            CandidateNeeded = x.rp.CandidateNeeded,
                            MissingEmployee = x.rp.MissingEmployee,
                        }).ToList();
                    if (requirePos.Count() != 0)
                    {
                        var empQuery = from ep in _context.EmpPositionInProjects
                                       join rp in _context.RequiredPositions on ep.RequiredPositionID equals rp.ID
                                       select new { ep, rp };
                        var checkEmp = empQuery.Where(x => x.ep.EmpID.Equals(empID) && x.rp.ProjectID.Equals(p.ProjectID)
                        && x.ep.Status != ConfirmStatus.Reject).FirstOrDefault();
                        var skillQuery = from rs in _context.RequiredSkills
                                         join s in _context.Skills on rs.SkillID equals s.SkillID
                                         select new { rs, s };
                        if (checkEmp == null)
                        {
                            foreach (var pos in requirePos)
                            {
                                pos.Language = _context.RequiredLanguages.Where(x => x.RequiredPositionID.Equals(pos.RequiredPosID))
                                    .Select(x => new LanguageDetail()
                                    {
                                        LangID = x.LangID,
                                        Priority = x.Priority
                                    }).ToList();
                                pos.SoftSkillIDs = skillQuery.Where(x => x.rs.RequiredPositionID.Equals(pos.RequiredPosID)
                                && x.s.SkillType.Equals(SkillType.SoftSkill)).Select(x => x.rs.SkillID).ToList();
                                pos.HardSkills = skillQuery.Where(x => x.rs.RequiredPositionID.Equals(pos.RequiredPosID)
                                && x.s.SkillType.Equals(SkillType.HardSkill)).Select(x => new HardSkillDetail()
                                {
                                    HardSkillID = x.rs.SkillID,
                                    SkillLevel = (int)x.rs.SkillLevel,
                                    CertificationLevel = (int)x.rs.CertificationLevel,
                                    Priority = (int)x.rs.Priority
                                }).ToList();
                            }
                            var projectVM = new ProjectVM()
                            {
                                ProjectID = p.ProjectID,
                                ProjectName = p.ProjectName,
                                TypeID = p.ProjectTypeID,
                                FieldID = p.ProjectFieldID,
                                ProjectManagerID = p.ProjectManagerID,
                                RequiredPositions = requirePos
                            };
                            list.Add(projectVM);
                        }
                    }
                }
            }
            return list;
        }

        public async Task<List<DeletedProject>> CheckNoEmpProject()
        {
            List<DeletedProject> list = new List<DeletedProject>();
            var projects = await _context.Projects.Where(x => x.Status.Equals(ProjectStatus.NoEmployee))
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
                var empQuery = from ep in _context.EmpPositionInProjects
                               join rp in _context.RequiredPositions on ep.RequiredPositionID equals rp.ID
                               select new { ep, rp };
                foreach (var p in projects)
                {
                    if (DateTime.Compare(p.DateBegin.Date, DateTime.Today) == 0)
                    {
                        var employees = empQuery.Where(x => x.rp.ProjectID.Equals(p.ProjectID)).Select(x => x.ep.EmpID);
                        if (employees.Count() == 0)
                        {
                            DeletedProject deletedProject = new DeletedProject()
                            {
                                ProjectManagerID = p.ProjectManagerID,
                                ProjectName = p.ProjectName
                            };
                            list.Add(deletedProject);
                            var listRequiredPos = await _context.RequiredPositions.Where(x => x.ProjectID.Equals(p.ProjectID))
                                .Select(x => new RequiredPosition()
                                {
                                    ID = x.ID,
                                    ProjectID = x.ProjectID,
                                    PositionID = x.PositionID,
                                    CandidateNeeded = x.CandidateNeeded,
                                    DateCreated = x.DateCreated,
                                    MissingEmployee = x.MissingEmployee,
                                    Status = x.Status
                                }).ToListAsync();
                            if (listRequiredPos.Count() != 0)
                            {
                                foreach (var pos in listRequiredPos)
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
                            _context.Projects.Remove(p);
                            await _context.SaveChangesAsync();
                        }
                    }
                }
            }
            return list;
        }

        public async Task<ApiResult<List<MissEmpPosition>>> GetMissEmpPos()
        {
            List<MissEmpPosition> list = new List<MissEmpPosition>();
            var positions = await _context.Positions.Select(x => new Position()
            {
                PosID = x.PosID,
                Name = x.Name
            }).ToListAsync();
            var posQuery = from rp in _context.RequiredPositions
                           join p in _context.Projects on rp.ProjectID equals p.ProjectID
                           select new { rp, p };
            foreach (var pos in positions)
            {
                var missEmpRequiredPos = await posQuery.Where(x => x.rp.PositionID.Equals(pos.PosID) && x.rp.MissingEmployee > 0
                && x.p.Status != ProjectStatus.Finished).Select(x => x.rp.MissingEmployee).ToListAsync();
                if (missEmpRequiredPos.Count() != 0)
                {
                    MissEmpPosition missEmpPos = new MissEmpPosition()
                    {
                        Name = pos.Name,
                        MissingEmp = 0
                    };
                    foreach (var rp in missEmpRequiredPos)
                    {
                        missEmpPos.MissingEmp += rp;
                    }
                    list.Add(missEmpPos);
                }
            }
            return new ApiSuccessResult<List<MissEmpPosition>>(list);
        }

        public async Task<ApiResult<List<SkillInPos>>> GetSkillInPos(int posID)
        {
            var list = new List<SkillInPos>();
            var listRequirePos = await _context.RequiredPositions.Where(x => x.PositionID.Equals(posID))
                .Select(x => x.ID).ToListAsync();
            if (listRequirePos.Count() != 0)
            {
                var skillQuery = from rs in _context.RequiredSkills
                                 join s in _context.Skills on rs.SkillID equals s.SkillID
                                 select new { rs, s };
                List<string> skillName = new List<string>();
                foreach (var rp in listRequirePos)
                {
                    var listRequireSkill = await skillQuery.Where(x => x.rs.RequiredPositionID.Equals(rp)
                    && x.s.SkillType.Equals(SkillType.HardSkill)).Select(x => x.s.SkillName).ToListAsync();
                    if (listRequireSkill.Count() != 0)
                    {
                        foreach (var rs in listRequireSkill)
                        {
                            skillName.Add(rs);
                        }
                    }
                }
                list = skillName.GroupBy(x => x).Select(x => new SkillInPos()
                {
                    HardSkill = x.Key,
                    NumberInRequire = x.Count()
                }).ToList();
            }
            return new ApiSuccessResult<List<SkillInPos>>(list);
        }

        public async Task<ApiResult<List<SkillInAllPos>>> GetSkillInAllPos()
        {
            var posQuery = from po in _context.Positions
                           join rp in _context.RequiredPositions on po.PosID equals rp.PositionID
                           select new { po, rp };
            var result = new List<SkillInAllPos>();
            var listPos = await posQuery.Where(x => x.rp.MissingEmployee > 0).Select(x => x.po.PosID).Distinct().ToListAsync();
            foreach (var pos in listPos)
            {
                var list = new List<SkillInPos>();
                var listRequirePos = await _context.RequiredPositions.Where(x => x.PositionID.Equals(pos) && x.MissingEmployee > 0)
                    .Select(x => x.ID).ToListAsync();
                if (listRequirePos.Count() != 0)
                {
                    var skillQuery = from rs in _context.RequiredSkills
                                     join s in _context.Skills on rs.SkillID equals s.SkillID
                                     select new { rs, s };
                    List<string> skillName = new List<string>();
                    foreach (var rp in listRequirePos)
                    {
                        var listRequireSkill = await skillQuery.Where(x => x.rs.RequiredPositionID.Equals(rp)
                        && x.s.SkillType.Equals(SkillType.HardSkill)).Select(x => x.s.SkillName).ToListAsync();
                        if (listRequireSkill.Count() != 0)
                        {
                            foreach (var rs in listRequireSkill)
                            {
                                skillName.Add(rs);
                            }
                        }
                    }
                    list = skillName.GroupBy(x => x).Select(x => new SkillInPos()
                    {
                        HardSkill = x.Key,
                        NumberInRequire = x.Count()
                    }).ToList();
                }
                result.Add(new SkillInAllPos()
                {
                    PosID = pos,
                    SkillInPos = list
                });
            }
            return new ApiSuccessResult<List<SkillInAllPos>>(result);
        }

        public async Task<ApiResult<List<EmpInProject>>> GetEmpByRequiredID(int requiredID)
        {
            var query = from e in _context.Employees
                        join ep in _context.EmpPositionInProjects on e.Id equals ep.EmpID
                        select new { e, ep };
            var employees = await query.Where(x => x.ep.RequiredPositionID.Equals(requiredID))
                .Select(x => new EmpInProject()
                {
                    EmpID = x.ep.EmpID,
                    Name = x.e.Name,
                    Email = x.e.Email,
                    DateIn = x.ep.DateIn,
                    PhoneNumber = x.e.PhoneNumber,
                    Status = x.ep.Status,
                    RejectReason = x.ep.Note
                }).ToListAsync();
            return new ApiSuccessResult<List<EmpInProject>>(employees);
        }

        public async Task<ApiResult<List<string>>> CheckCandidate(int projectID, AddCandidateRequest request)
        {
            List<string> list = new List<string>();
            var empQuery = from ep in _context.EmpPositionInProjects
                           join rp in _context.RequiredPositions on ep.RequiredPositionID equals rp.ID
                           select new { ep, rp };
            foreach (var req in request.Candidates)
            {
                foreach (var emp in req.EmpIDs)
                {
                    var checkEmp = await _context.EmpPositionInProjects.FindAsync(emp, req.RequiredPosID);
                    if (checkEmp != null)
                    {
                        if (checkEmp.Status.Equals(ConfirmStatus.Reject))
                        {
                            var employee = await _context.Employees.FindAsync(emp);
                            var message = employee.Name + " : " + checkEmp.Note;
                            list.Add(message);
                        }
                    }
                    else
                    {
                        var checkOtherPos = await empQuery.Where(x => x.rp.ProjectID.Equals(projectID)
                        && x.ep.Status.Equals(ConfirmStatus.Reject) && x.ep.RequiredPositionID != req.RequiredPosID
                        && x.ep.EmpID.Equals(emp)).Select(x => x.ep.Note).FirstOrDefaultAsync();
                        if (checkOtherPos != null)
                        {
                            var employee = await _context.Employees.FindAsync(emp);
                            var message = employee.Name + " : " + checkOtherPos;
                            list.Add(message);
                        }
                    }
                }
            }
            if (list.Count() != 0)
            {
                return new ApiResult<List<string>>()
                {
                    IsSuccessed = false,
                    Message = "List employee has been rejected",
                    ResultObj = list
                };
            }
            return new ApiResult<List<string>>()
            {
                IsSuccessed = true,
                Message = null,
                ResultObj = null
            };
        }
    }
}