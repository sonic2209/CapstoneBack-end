using ESMS.BackendAPI.Services.Projects;
using ESMS.BackendAPI.ViewModels.Certification;
using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Employees;
using ESMS.BackendAPI.ViewModels.Employees.Suggestion;
using ESMS.BackendAPI.ViewModels.Employees.Suggestion.SingleCandidate;
using ESMS.BackendAPI.ViewModels.Position;
using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Data.Enums;
using ESMS.ViewModels.System.Employees;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ESMS.BackendAPI.Services.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _config;
        private readonly ESMSDbContext _context;
        private readonly IProjectService _projectService;

        public EmployeeService(UserManager<Employee> userManager, SignInManager<Employee> signInManager,
            RoleManager<Role> roleManager, IConfiguration config, ESMSDbContext context, IProjectService projectService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            _context = context;
            _projectService = projectService;
        }

        public EmployeeService()
        {
        }

        public async Task<ApiResult<LoginVm>> Authenticate(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                user = await _userManager.FindByNameAsync(request.Email);
            if (user == null)
                return new ApiErrorResult<LoginVm>("Account does not exist");

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
            if (!result.Succeeded)
            {
                return new ApiErrorResult<LoginVm>("Email or password is not correct");
            }
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email,user.Email),
                new Claim(ClaimTypes.GivenName, user.Name),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.Role, string.Join(";",roles))
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new ApiSuccessResult<LoginVm>(new LoginVm()
            {
                EmpId = user.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            }
            );
        }

        public async Task<ApiResult<string>> Create(EmpCreateRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                return new ApiErrorResult<string>("Account existed");
            }
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return new ApiErrorResult<string>("Email existed");
            }
            user = new Employee()
            {
                Name = request.Name,
                DateCreated = DateTime.Now,
                IdentityNumber = request.IdentityNumber,
                Address = request.Address,
                Email = request.Email,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, request.RoleName) == false)
                {
                    await _userManager.AddToRoleAsync(user, request.RoleName);
                }
                user = await _userManager.FindByNameAsync(request.UserName);
                string empID = user.Id;
                return new ApiSuccessResult<string>(empID);
            }

            return new ApiErrorResult<string>("Register failed");
        }

        public async Task<ApiResult<bool>> AddEmpPosition(string empID, AddEmpPositionRequest request)
        {
            foreach (var language in request.Languages)
            {
                var empLanguage = new EmpLanguage()
                {
                    EmpID = empID,
                    LangID = language.LangID,
                    LangLevel = language.LangLevel
                };
                _context.EmpLanguages.Add(empLanguage);
            }
            if (request.SoftSkills.Count() != 0)
            {
                foreach (var softSkill in request.SoftSkills)
                {
                    var skill = await _context.Skills.FindAsync(softSkill);
                    if (skill == null) return new ApiErrorResult<bool>("Skill:" + skill.SkillName + " not found");
                    if (skill.Status == false) return new ApiErrorResult<bool>("Skill:" + skill.SkillName + " is disable");
                    var empSoftSkill = new EmpSkill()
                    {
                        EmpID = empID,
                        SkillID = softSkill
                    };
                    _context.EmpSkills.Add(empSoftSkill);
                }
            }
            foreach (var hardSkill in request.HardSkills)
            {
                var skill = await _context.Skills.FindAsync(hardSkill.SkillID);
                if (skill == null) return new ApiErrorResult<bool>("Skill:" + skill.SkillName + " not found");
                if (skill.Status == false) return new ApiErrorResult<bool>("Skill:" + skill.SkillName + " is disable");
                var empHardSkill = new EmpSkill()
                {
                    EmpID = empID,
                    SkillID = hardSkill.SkillID,
                    SkillLevel = (SkillLevel)hardSkill.SkillLevel,
                    DateStart = DateTime.Now
                };
                _context.EmpSkills.Add(empHardSkill);
                if (hardSkill.EmpCertifications.Count() != 0)
                {
                    foreach (var certification in hardSkill.EmpCertifications)
                    {
                        var certi = await _context.Certifications.FindAsync(certification.CertiID);
                        if (certi == null) return new ApiErrorResult<bool>("Certification:" + certi.CertificationName + " not found");
                        if (certi.Status == false) return new ApiErrorResult<bool>("Certification:" + certi.CertificationName + " is disable");
                        if (DateTime.Compare(DateTime.Parse(certification.DateTaken).Date, DateTime.Today) < 0)
                        {
                            return new ApiErrorResult<bool>("Certification " + certi.CertificationName + "-date taken is earlier than today");
                        }
                        var empCertification = new EmpCertification()
                        {
                            EmpID = empID,
                            CertificationID = certification.CertiID,
                            DateTaken = DateTime.Parse(certification.DateTaken)
                        };
                        if (certification.DateEnd.Equals(""))
                        {
                            empCertification.DateEnd = null;
                        }
                        else
                        {
                            if (DateTime.Compare(DateTime.Parse(certification.DateEnd).Date, DateTime.Today) < 0)
                            {
                                return new ApiErrorResult<bool>("Certification " + certi.CertificationName + "-date expire is earlier than today");
                            }
                            if (DateTime.Compare(empCertification.DateTaken.Date, DateTime.Parse(certification.DateEnd).Date) > 0)
                            {
                                return new ApiErrorResult<bool>("Certification " + certi.CertificationName + "-date expire is earlier than date taken");
                            }
                            empCertification.DateEnd = DateTime.Parse(certification.DateEnd);
                        }
                        _context.EmpCertifications.Add(empCertification);
                    }
                }
            }
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Add employee'infomation failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<bool>> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("User does not exist");
            }
            user.DateEnd = DateTime.Now;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Delete user failed");
        }

        public async Task<ApiResult<EmpVm>> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<EmpVm>("User does not exist");
            }
            var roles = await _userManager.GetRolesAsync(user);
            string currentRole = null;
            if (roles.Count > 0)
            {
                currentRole = roles[0];
            }
            var empVm = new EmpVm()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Name = user.Name,
                Address = user.Address,
                IdentityNumber = user.IdentityNumber,
                UserName = user.UserName,
                Id = user.Id,
                RoleName = currentRole
            };
            return new ApiSuccessResult<EmpVm>(empVm);
        }

        public async Task<ApiResult<PagedResult<EmpVm>>> GetEmpsPaging(GetEmpPagingRequest request)
        {
            {
                var query = _userManager.Users;
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query.Where(x => x.UserName.Contains(request.Keyword) || x.Name.Contains(request.Keyword) || x.PhoneNumber.Contains(request.Keyword)
                    || x.Email.Contains(request.Keyword));
                }
                //3.Paging
                int totalRow = await query.CountAsync();

                var data = await query.OrderByDescending(x => x.DateCreated).Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new EmpVm()
                    {
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        Name = x.Name,
                        Id = x.Id,
                        UserName = x.UserName,
                        DateCreated = x.DateCreated
                    }).ToListAsync();
                foreach (var empUser in data)
                {
                    var user = await _userManager.FindByIdAsync(empUser.Id.ToString());
                    var roles = await _userManager.GetRolesAsync(user);
                    string currentRole = null;
                    if (roles.Count > 0)
                    {
                        currentRole = roles[0];
                    }
                    empUser.RoleName = currentRole;
                }

                //4.Select and projection
                var pagedResult = new PagedResult<EmpVm>()
                {
                    TotalRecords = totalRow,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Items = data
                };
                return new ApiSuccessResult<PagedResult<EmpVm>>(pagedResult);
            }
        }

        public async Task<ApiResult<List<CandidateViewModel>>> SuggestCandidate(int projectID, SuggestCadidateRequest request)
        {
            List<CandidateViewModel> result = new List<CandidateViewModel>();
            int ProjectTypeID = request.ProjectTypeID;
            int ProjectFieldID = request.ProjectFieldID;
            foreach (RequiredPositionDetail requiredPosition in request.RequiredPositions)
            {
                var dicCandidate = new Dictionary<string, double>();
                {
                    List<MatchViewModel> listMatchDetail = new List<MatchViewModel>();
                    var Position = _context.Positions.Where(x => x.PosID == requiredPosition.PosID).Select(x => x.Name).FirstOrDefault();
                    var PosId = _context.Positions.Where(x => x.PosID == requiredPosition.PosID).Select(x => x.PosID).FirstOrDefault();

                    var ListEmpInPos = await _context.Employees.Select(x => new EmpInPos()
                    {
                        EmpId = x.Id,
                        EmpName = x.Name,
                    }).ToListAsync();
                    if (ListEmpInPos.Count > 0)
                    {
                        foreach (EmpInPos emp in ListEmpInPos)
                        {
                            var user = await _userManager.FindByIdAsync(emp.EmpId.ToString());
                            var roles = await _userManager.GetRolesAsync(user);
                            string currentRole = null;
                            if (roles.Count > 0)
                            {
                                currentRole = roles[0];
                                if (!currentRole.Equals("Employee"))
                                {
                                    continue;
                                }
                            }
                            else
                                continue;
                            double match = 0;
                            double Languagematch = 0;
                            double Softskillmatch = 0;
                            double Hardskillmatch = 0;
                            double ProjectTypeMatch = 0;
                            double ProjectFieldMatch = 0;
                            MatchViewModel matchDetail = new MatchViewModel();

                            //add match theo ngon ngu
                            foreach (LanguageDetail language in requiredPosition.Language)
                            {
                                var ListEmpInLang = await _context.EmpLanguages.Where(x => x.EmpID.Equals(emp.EmpId) && x.LangID == language.LangID).Select(x => new EmpInLang()
                                {
                                    EmpId = x.EmpID,
                                    LangLevel = x.LangLevel,
                                }).ToListAsync();

                                if (ListEmpInLang.Count > 0)
                                {
                                    foreach (EmpInLang empl in ListEmpInLang)
                                    {
                                        Languagematch += (empl.LangLevel * language.Priority * 0.1) / requiredPosition.Language.Count;
                                    }
                                    //match += Math.Round(Languagematch, 2);
                                }
                            }
                            //Add theo softskill
                            var listEmpSkillquery = from es in _context.EmpSkills
                                                    join s in _context.Skills on es.SkillID equals s.SkillID
                                                    select new { es, s };
                            var listEmpSoftSkillquery = listEmpSkillquery.Where(x => x.s.SkillType == SkillType.SoftSkill && x.es.EmpID.Equals(emp.EmpId));
                            var listEmpSoftSkill = await listEmpSoftSkillquery.Select(x => x.es.SkillID).ToListAsync();
                            foreach (int softskillId in requiredPosition.SoftSkillIDs)
                            {
                                foreach (var softSkill in listEmpSoftSkill)
                                {
                                    if (softSkill.Equals(softskillId))
                                    {
                                        Softskillmatch += 10 / (requiredPosition.SoftSkillIDs.Count);
                                    }
                                }
                                //match += Math.Round(Softskillmatch, 2);
                            }
                            //add match vao hardskill
                            var listEmpHardSkillquery = listEmpSkillquery.Where(x => x.s.SkillType == SkillType.HardSkill && x.es.EmpID.Equals(emp.EmpId));
                            var listEmpHardSkill = await listEmpHardSkillquery.Select(x => new EmpInHardSkill()
                            {
                                EmpID = emp.EmpId,
                                SkillID = x.s.SkillID,
                                SkillLevel = x.es.SkillLevel,
                            }).ToListAsync();
                            foreach (HardSkillDetail hardskill in requiredPosition.HardSkills)
                            {
                                foreach (EmpInHardSkill emphs in listEmpHardSkill)
                                {
                                    if (emphs.SkillID.Equals(hardskill.HardSkillID))
                                    {
                                        var certiquery = from c in _context.Certifications
                                                         join ec in _context.EmpCertifications on c.CertificationID equals ec.CertificationID
                                                         select new { c, ec };
                                        var listCertiSkill = await certiquery.Where(x => x.ec.EmpID.Equals(emphs.EmpID) && x.c.SkillID == emphs.SkillID).Select(x => new CertiInSkill
                                        {
                                            CertiID = x.c.CertificationID,
                                            SkillID = x.c.SkillID,
                                            CertiLevel = x.c.CertiLevel
                                        }).ToListAsync();
                                        var HighestCerti = new EmpHighestCerti()
                                        {
                                            EmpID = emphs.EmpID,
                                            HighestCertiLevel = listCertiSkill.Any() ? listCertiSkill.Max(x => x.CertiLevel) : 0,
                                        };
                                        //if (HighestCerti.HighestCertiLevel >= hardskill.CertificationLevel)
                                        //{
                                        Hardskillmatch += (double)(((HighestCerti.HighestCertiLevel - hardskill.CertificationLevel)) + ((int)emphs.SkillLevel - hardskill.SkillLevel)) * hardskill.Priority / (18 * requiredPosition.HardSkills.Count);
                                        //match += Math.Round(Hardskillmatch, 2);
                                        //}
                                        //else
                                        //{
                                        //    Hardskillmatch = (((int)emphs.SkillLevel * 2 * 0.5)) * hardskill.Priority / 10 * requiredPosition.HardSkills.Count;
                                        //    match += Math.Round(Hardskillmatch, 2);
                                        //}
                                    }
                                }
                            }

                            //Merge code mới
                            //Loc nhung nhan vien ko available dua theo thoi gian ket thuc du an dang tien hanh
                            var projectquery = from p in _context.Projects
                                               join rp in _context.RequiredPositions on p.ProjectID equals rp.ProjectID
                                               join epip in _context.EmpPositionInProjects on rp.ID equals epip.RequiredPositionID
                                               select new { rp, p, epip };

                            var currentProjectBeginDate = await _context.Projects.Where(x => x.ProjectID == projectID).Select(x => x.DateBegin).FirstOrDefaultAsync();
                            var listProjectCurrentlyIn = await projectquery.Where(x => x.p.Status != ProjectStatus.Finished && x.epip.EmpID.Equals(emp.EmpId) && x.epip.Status != ConfirmStatus.Reject)
                                .Select(x => x.p.DateEstimatedEnd).ToListAsync();
                            //var projectOnGoingDateEnd = await projectquery.Where(x => (x.p.Status == ProjectStatus.OnGoing || x.p.Status == ProjectStatus.Confirmed) && x.epip.EmpID.Equals(emp.EmpId)).Select(x => x.p.DateEstimatedEnd).ToListAsync();
                            bool checkProjectDate = false;
                            if (listProjectCurrentlyIn.Count > 0)
                            {
                                foreach (var dateEnd in listProjectCurrentlyIn)
                                {
                                    if (dateEnd > currentProjectBeginDate)
                                    {
                                        checkProjectDate = true;
                                        break;
                                    }
                                }
                                if (checkProjectDate == true)
                                {
                                    continue;
                                }
                            }
                            //Add match theo projecttype
                            var listProjectWithType = await projectquery.Where(x => x.p.ProjectTypeID == ProjectTypeID && x.epip.EmpID.Equals(emp.EmpId) && x.epip.Status == ConfirmStatus.Accept).Select(x => x.p.ProjectID).ToListAsync();
                            var numberOfProjectWithType = listProjectWithType.Count();
                            if (numberOfProjectWithType == 0)
                            {
                                ProjectTypeMatch = 0;
                            }
                            if (numberOfProjectWithType > 2 && numberOfProjectWithType < 5)
                            {
                                ProjectTypeMatch = 3;
                                //match += ProjectTypeMatch;
                            }
                            if (numberOfProjectWithType > 5 && numberOfProjectWithType < 10)
                            {
                                ProjectTypeMatch = 6;
                                //match += ProjectTypeMatch;
                            }
                            if (numberOfProjectWithType > 9)
                            {
                                ProjectTypeMatch = 10;
                                //match += ProjectTypeMatch;
                            }

                            //Add match theo projectfield
                            var listProjectWithField = await projectquery.Where(x => x.p.ProjectFieldID == ProjectFieldID && x.epip.EmpID.Equals(emp.EmpId) && x.epip.Status == ConfirmStatus.Accept).Select(x => x.p.ProjectID).ToListAsync();
                            var numberOfProjectWithField = listProjectWithField.Count();
                            if (numberOfProjectWithField == 0)
                            {
                                ProjectFieldMatch = 0;
                            }
                            if (numberOfProjectWithField > 2 && numberOfProjectWithField < 5)
                            {
                                ProjectFieldMatch = 3;
                                //match += ProjectFieldMatch;
                            }
                            if (numberOfProjectWithField > 5 && numberOfProjectWithField < 10)
                            {
                                ProjectFieldMatch = 6;
                                //match += ProjectFieldMatch;
                            }
                            if (numberOfProjectWithField > 9)
                            {
                                ProjectFieldMatch = 10;
                                //match += ProjectFieldMatch;
                            }
                            match = Math.Round(Languagematch + Softskillmatch + Hardskillmatch + ProjectTypeMatch + ProjectFieldMatch, 2);
                            //Loc nhung nhan vien khong du diem toi thieu
                            //if (Hardskillmatch < 4 || Softskillmatch < 4 || Languagematch < 4 || match < 12.5)
                            //{
                            //    continue;
                            //}
                            matchDetail = new MatchViewModel()
                            {
                                EmpID = emp.EmpId,
                                EmpName = emp.EmpName,
                                LanguageMatch = Languagematch,
                                SoftSkillMatch = Softskillmatch,
                                HardSkillMatch = Math.Round(Hardskillmatch, 2),
                                ProjectTypeMatch = ProjectTypeMatch,
                                ProjectFieldMatch = ProjectFieldMatch,
                                OverallMatch = match,
                            };
                            listMatchDetail.Add(matchDetail);
                        }
                    }
                    //}
                    result.Add(new CandidateViewModel()
                    {
                        Position = Position,
                        PosId = PosId,
                        MatchDetail = listMatchDetail,
                    });
                }
            }
            return new ApiSuccessResult<List<CandidateViewModel>>(result);
        }

        public async Task<ApiResult<List<SingleCandidateViewModel>>> SingleCandidateSuggest(string empID)
        {
            var empName = _context.Employees.Where(x => x.Id.Equals(empID)).Select(x => x.Name).FirstOrDefault();
            var listProject = _projectService.GetMissEmpProjects(empID);
            List<SingleCandidateViewModel> result = new List<SingleCandidateViewModel>();

            foreach (var project in listProject)
            {
                int ProjectTypeID = project.TypeID;
                int ProjectFieldID = project.FieldID;
                List<SingleCandidateMatchInPos> listMatchInPosDetail = new List<SingleCandidateMatchInPos>();

                //Loc nhung project ko available dua theo thoi gian ket thuc du an dang tien hanh
                var projectquery = from p in _context.Projects
                                   join rp in _context.RequiredPositions on p.ProjectID equals rp.ProjectID
                                   join epip in _context.EmpPositionInProjects on rp.ID equals epip.RequiredPositionID
                                   select new { p, epip };

                var currentProjectBeginDate = await _context.Projects.Where(x => x.ProjectID == project.ProjectID).Select(x => x.DateBegin).FirstOrDefaultAsync();
                var listProjectCurrentlyIn = await projectquery.Where(x => (x.p.Status == ProjectStatus.OnGoing || x.p.Status == ProjectStatus.Confirmed) && x.epip.EmpID.Equals(empID) && x.epip.Status == ConfirmStatus.Accept && x.epip.Status == ConfirmStatus.New).Select(x => x.p.DateEstimatedEnd).ToListAsync();
                //var projectOnGoingDateEnd = await projectquery.Where(x => (x.p.Status == ProjectStatus.OnGoing || x.p.Status == ProjectStatus.Confirmed) && x.epip.EmpID.Equals(emp.EmpId)).Select(x => x.p.DateEstimatedEnd).ToListAsync();
                bool checkProjectDate = false;
                if (listProjectCurrentlyIn.Count > 0)
                {
                    foreach (var dateEnd in listProjectCurrentlyIn)
                    {
                        if (dateEnd > currentProjectBeginDate)
                        {
                            checkProjectDate = true;
                            break;
                        }
                    }
                    if (checkProjectDate == true)
                    {
                        continue;
                    }
                }

                foreach (RequiredPosVM requiredPosition in project.RequiredPositions)
                {
                    {
                        List<MatchViewModel> listMatchDetail = new List<MatchViewModel>();
                        var Position = _context.Positions.Where(x => x.PosID == requiredPosition.PosID).Select(x => x.Name).FirstOrDefault();
                        var PosId = _context.Positions.Where(x => x.PosID == requiredPosition.PosID).Select(x => x.PosID).FirstOrDefault();

                        var user = await _userManager.FindByIdAsync(empID.ToString());
                        var roles = await _userManager.GetRolesAsync(user);
                        string currentRole = null;
                        if (roles.Count > 0)
                        {
                            currentRole = roles[0];
                            if (!currentRole.Equals("Employee"))
                            {
                                return new ApiErrorResult<List<SingleCandidateViewModel>>("This user is a PM or HR");
                            }
                        }
                        else
                            return new ApiErrorResult<List<SingleCandidateViewModel>>("This user is a PM or HR");
                        double match = 0;
                        double Languagematch = 0;
                        double Softskillmatch = 0;
                        double Hardskillmatch = 0;
                        double ProjectTypeMatch = 0;
                        double ProjectFieldMatch = 0;
                        SingleCandidateMatchInPos matchDetail = new SingleCandidateMatchInPos();

                        //Merge code mới

                        //var query = from ep in _context.emppositions
                        //            join el in _context.EmpLanguages on ep.empid equals el.empid
                        //            select new { ep, el };

                        //add match theo ngon ngu
                        foreach (LanguageDetail language in requiredPosition.Language)
                        {
                            var ListEmpInLang = await _context.EmpLanguages.Where(x => x.EmpID.Equals(empID) && x.LangID == language.LangID).Select(x => new EmpInLang()
                            {
                                EmpId = x.EmpID,
                                LangLevel = x.LangLevel,
                            }).ToListAsync();
                            //var ListEmpInLang = await query.Where(x => x.el.EmpID.Equals(emp.EmpId) && x.el.LangID == language.LangID).Select(x => new EmpInLang()
                            //{
                            //    EmpId = x.el.EmpID,
                            //    LangLevel = x.el.LangLevel,
                            //}).ToListAsync();
                            //match += (langlevel1*prio/10)/tong so requiredlang

                            if (ListEmpInLang.Count > 0)
                            {
                                foreach (EmpInLang empl in ListEmpInLang)
                                {
                                    Languagematch += (empl.LangLevel * language.Priority * 0.1) / requiredPosition.Language.Count;
                                }
                            }
                        }
                        //Add theo softskill
                        var listEmpSkillquery = from es in _context.EmpSkills
                                                join s in _context.Skills on es.SkillID equals s.SkillID
                                                select new { es, s };
                        var listEmpSoftSkillquery = listEmpSkillquery.Where(x => x.s.SkillType == SkillType.SoftSkill && x.es.EmpID.Equals(empID));
                        var listEmpSoftSkill = await listEmpSoftSkillquery.Select(x => x.es.SkillID).ToListAsync();
                        foreach (int softskillId in requiredPosition.SoftSkillIDs)
                        {
                            foreach (var softSkill in listEmpSoftSkill)
                            {
                                if (softSkill.Equals(softskillId))
                                {
                                    Softskillmatch += 10 / (requiredPosition.SoftSkillIDs.Count);
                                }
                            }
                        }
                        //add match vao hardskill
                        var listEmpHardSkillquery = listEmpSkillquery.Where(x => x.s.SkillType == SkillType.HardSkill && x.es.EmpID.Equals(empID));
                        var listEmpHardSkill = await listEmpHardSkillquery.Select(x => new EmpInHardSkill()
                        {
                            EmpID = empID,
                            SkillID = x.s.SkillID,
                            SkillLevel = x.es.SkillLevel,
                        }).ToListAsync();
                        foreach (HardSkillDetail hardskill in requiredPosition.HardSkills)
                        {
                            foreach (EmpInHardSkill emphs in listEmpHardSkill)
                            {
                                if (emphs.SkillID.Equals(hardskill.HardSkillID))
                                {
                                    var certiquery = from c in _context.Certifications
                                                     join ec in _context.EmpCertifications on c.CertificationID equals ec.CertificationID
                                                     select new { c, ec };
                                    var listCertiSkill = await certiquery.Where(x => x.ec.EmpID.Equals(emphs.EmpID) && x.c.SkillID == emphs.SkillID).Select(x => new CertiInSkill
                                    {
                                        CertiID = x.c.CertificationID,
                                        SkillID = x.c.SkillID,
                                        CertiLevel = x.c.CertiLevel
                                    }).ToListAsync();
                                    var HighestCerti = new EmpHighestCerti()
                                    {
                                        EmpID = emphs.EmpID,
                                        HighestCertiLevel = listCertiSkill.Any() ? listCertiSkill.Max(x => x.CertiLevel) : 0,
                                    };
                                    //if (HighestCerti.HighestCertiLevel >= hardskill.CertificationLevel)
                                    //{
                                    Hardskillmatch += (double)(((HighestCerti.HighestCertiLevel - hardskill.CertificationLevel)) + ((int)emphs.SkillLevel - hardskill.SkillLevel)) * hardskill.Priority / (18 * requiredPosition.HardSkills.Count);

                                    //}
                                    //else
                                    //{
                                    //    Hardskillmatch = (((int)emphs.SkillLevel * 2 * 0.5)) * hardskill.Priority / 10 * requiredPosition.HardSkills.Count;
                                    //    match += Math.Round(Hardskillmatch, 2);
                                    //}
                                }
                            }
                        }
                        //Add match theo projecttype
                        var listProjectWithType = await projectquery.Where(x => x.p.ProjectTypeID == ProjectTypeID && x.epip.EmpID.Equals(empID) && x.epip.Status == ConfirmStatus.Accept).Select(x => x.p.ProjectID).ToListAsync();
                        var numberOfProjectWithType = listProjectWithType.Count();
                        if (numberOfProjectWithType == 0)
                        {
                            ProjectTypeMatch = 0;
                        }
                        if (numberOfProjectWithType > 2 && numberOfProjectWithType < 5)
                        {
                            ProjectTypeMatch = 3;
                        }
                        if (numberOfProjectWithType > 5 && numberOfProjectWithType < 10)
                        {
                            ProjectTypeMatch = 6;
                        }
                        if (numberOfProjectWithType > 9)
                        {
                            ProjectTypeMatch = 10;
                        }

                        //Add match theo projectfield
                        var listProjectWithField = await projectquery.Where(x => x.p.ProjectFieldID == ProjectFieldID && x.epip.EmpID.Equals(empID) && x.epip.Status == ConfirmStatus.Accept).Select(x => x.p.ProjectID).ToListAsync();
                        var numberOfProjectWithField = listProjectWithField.Count();
                        if (numberOfProjectWithField == 0)
                        {
                            ProjectFieldMatch = 0;
                        }
                        if (numberOfProjectWithField > 2 && numberOfProjectWithField < 5)
                        {
                            ProjectFieldMatch = 3;
                        }
                        if (numberOfProjectWithField > 5 && numberOfProjectWithField < 10)
                        {
                            ProjectFieldMatch = 6;
                        }
                        if (numberOfProjectWithField > 9)
                        {
                            ProjectFieldMatch = 10;
                        }
                        match = Math.Round(Languagematch + Softskillmatch + Hardskillmatch + ProjectTypeMatch + ProjectFieldMatch, 2);
                        //if (Hardskillmatch < 4 || Softskillmatch < 4 || Languagematch < 4 || match < 12.5)
                        //{
                        //    continue;
                        //}
                        matchDetail = new SingleCandidateMatchInPos()
                        {
                            PosId = PosId,
                            Position = Position,
                            EmpID = empID,
                            EmpName = empName,
                            LanguageMatch = Languagematch,
                            SoftSkillMatch = Softskillmatch,
                            HardSkillMatch = Math.Round(Hardskillmatch, 2),
                            ProjectTypeMatch = ProjectTypeMatch,
                            ProjectFieldMatch = ProjectFieldMatch,
                            OverallMatch = match,
                        };
                        listMatchInPosDetail.Add(matchDetail);
                    }
                }
                result.Add(new SingleCandidateViewModel()
                {
                    ProjectInfo = project,
                    MatchInEachPos = listMatchInPosDetail,
                });
            }
            return new ApiSuccessResult<List<SingleCandidateViewModel>>(result);
        }

        //public async Task<List<CandidateViewModel>> SuggestCandidate(int projectID, SuggestCadidateRequest request)
        //{
        //    List<CandidateViewModel> candidates = new List<CandidateViewModel>();
        //    var query = _userManager.Users;
        //    var listEmp = await query.Select(x => new EmpVm()
        //    {
        //        Email = x.Email,
        //        PhoneNumber = x.PhoneNumber,
        //        Name = x.Name,
        //        Id = x.Id,
        //        UserName = x.UserName
        //    }).ToListAsync();
        //    foreach (var item in listEmp)
        //    {
        //        int match = 0;
        //        var empPosition = from ep in _context.EmpPositions
        //                          select new { ep };
        //        empPosition = empPosition.Where(x => x.ep.EmpID.Equals(item.Id));
        //        var positions = await empPosition.Select(x => new EmpPosition()
        //        {
        //            PosID = x.ep.PosID,
        //            DateIn = x.ep.DateIn,
        //            DateOut = x.ep.DateOut,
        //            NameExp = x.ep.NameExp
        //        }).ToListAsync();
        //        var empCerti = from ec in _context.EmpCertifications
        //                       select new { ec };
        //        empCerti = empCerti.Where(x => x.ec.EmpID.Equals(item.Id));
        //        var certifications = await empCerti.Select(x => new EmpCertification()
        //        {
        //            CertificationID = x.ec.CertificationID,
        //            DateTaken = x.ec.DateTaken,
        //            DateEnd = x.ec.DateEnd
        //        }).ToListAsync();
        //        var empSkill = from es in _context.EmpSkills
        //                       select new { es };
        //        empSkill = empSkill.Where(x => x.es.EmpID.Equals(item.Id));
        //        var skills = await empSkill.Select(x => new EmpSkill()
        //        {
        //            SkillID = x.es.SkillID
        //        }).ToListAsync();
        //        foreach (var requiredPosition in request.RequiredPositions)
        //        {
        //            foreach (var position in positions)
        //            {
        //                if (position.ID.Equals(requiredPosition.PosID))
        //                {
        //                    switch (position.NameExp)
        //                    {
        //                        case NameExp.Fresher:
        //                            match = match + 5;
        //                            break;

        //                        case NameExp.Intern:
        //                            match = match + 10;
        //                            break;

        //                        case NameExp.Junior:
        //                            match = match + 15;
        //                            break;

        //                        case NameExp.Senior:
        //                            match = match + 20;
        //                            break;

        //                        case NameExp.Master:
        //                            match = match + 25;
        //                            break;
        //                    }
        //                    foreach (var skill in skills)
        //                    {
        //                        var s = await _context.Skills.FindAsync(skill.SkillID);
        //                        if (s.SkillType == SkillType.SoftSkill)
        //                        {
        //                            foreach (var requiredSoftSkill in requiredPosition.SoftSkillIDs)
        //                            {
        //                                if (skill.SkillID.Equals(requiredSoftSkill))
        //                                {
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            foreach (var requiredHardSKill in requiredPosition.HardSkills)
        //                            {
        //                                if (skill.SkillID.Equals(requiredHardSKill.HardSkillID))
        //                                {
        //                                    switch (skill.SkillLevel)
        //                                    {
        //                                        case SkillLevel.BasicKnowledge:
        //                                            match = match + 5;
        //                                            break;

        //                                        case SkillLevel.LimitedExperience:
        //                                            match = match + 10;
        //                                            break;

        //                                        case SkillLevel.Practical:
        //                                            match = match + 15;
        //                                            break;

        //                                        case SkillLevel.AppliedTheory:
        //                                            match = match + 20;
        //                                            break;

        //                                        case SkillLevel.RecognizedAuthority:
        //                                            match = match + 25;
        //                                            break;
        //                                    }
        //                                }
        //                                foreach (var certi in certifications)
        //                                {
        //                                    if (certi.ID.Equals(requiredHardSKill.CertificationID))
        //                                    {
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        if (match >= 50)
        //        {
        //            var candidate = new CandidateViewModel()
        //            {
        //                Emp = item,
        //                Match = match
        //            };
        //            candidates.Add(candidate);
        //        }
        //    }
        //    return candidates;
        //}

        public async Task<ApiResult<bool>> Update(string id, EmpUpdateRequest request)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email.Equals(request.Email) && x.Id != id))
            {
                return new ApiErrorResult<bool>("Email existed");
            }
            var user = await _userManager.FindByIdAsync(id.ToString());
            user.IdentityNumber = request.IdentityNumber;
            user.Address = request.Address;
            user.Email = request.Email;
            user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            if (await _userManager.IsInRoleAsync(user, request.RoleName) == false)
            {
                foreach (string rolename in roles)
                {
                    if (await _userManager.IsInRoleAsync(user, rolename) == true)
                    {
                        await _userManager.RemoveFromRoleAsync(user, rolename);
                    }
                }
                await _userManager.AddToRoleAsync(user, request.RoleName);
            }
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Update user failed");
        }

        public async Task<ApiResult<EmpInfoViewModel>> GetEmpInfo(string empID)
        {
            EmpInfoViewModel info = new EmpInfoViewModel();
            info.Languages = new List<LanguageInfo>();
            info.SoftSkills = new List<SoftSkillInfo>();
            info.HardSkills = new List<HardSkillInfo>();
            var langQuery = from el in _context.EmpLanguages
                            join l in _context.Languages on el.LangID equals l.LangID
                            select new { el, l };
            info.Languages = await langQuery.Where(x => x.el.EmpID.Equals(empID)).Select(x => new LanguageInfo()
            {
                LangID = x.el.LangID,
                LangName = x.l.LangName,
                LangLevel = x.el.LangLevel
            }).ToListAsync();
            var listSkill = await _context.EmpSkills.Where(x => x.EmpID.Equals(empID) && x.DateEnd == null)
                .Select(x => new EmpSkill()
                {
                    SkillID = x.SkillID,
                    SkillLevel = x.SkillLevel
                }).ToListAsync();
            var certiQuery = from ec in _context.EmpCertifications
                             join c in _context.Certifications on ec.CertificationID equals c.CertificationID
                             select new { ec, c };
            foreach (var s in listSkill)
            {
                var skill = await _context.Skills.FindAsync(s.SkillID);
                if (skill.SkillType == SkillType.SoftSkill)
                {
                    var softSkill = new SoftSkillInfo()
                    {
                        SkillID = skill.SkillID,
                        SkillName = skill.SkillName
                    };
                    info.SoftSkills.Add(softSkill);
                }
                else
                {
                    var empCerti = certiQuery.Where(x => x.ec.EmpID.Equals(empID) && x.c.SkillID.Equals(s.SkillID))
                        .Select(x => new CertiInfo()
                        {
                            CertiID = x.ec.CertificationID,
                            CertiName = x.c.CertificationName,
                            DateEnd = x.ec.DateEnd,
                            DateTaken = x.ec.DateTaken,
                            CertiLevel = x.c.CertiLevel
                        }).ToList();
                    HardSkillInfo hardSkill = new HardSkillInfo()
                    {
                        SkillID = skill.SkillID,
                        SkillName = skill.SkillName,
                        SkillLevel = (int)s.SkillLevel,
                        Certifications = empCerti
                    };
                    info.HardSkills.Add(hardSkill);
                }
            }
            return new ApiSuccessResult<EmpInfoViewModel>(info);
        }

        public async Task<ApiResult<bool>> UpdateEmpInfo(string empID, AddEmpPositionRequest request)
        {
            //Update Language
            var listLanguage = await _context.EmpLanguages.Where(x => x.EmpID.Equals(empID)).Select(x => new EmpLanguage()
            {
                EmpID = x.EmpID,
                LangID = x.LangID,
                LangLevel = x.LangLevel
            }).ToListAsync();
            if (request.Languages.Count() == 0)
            {
                if (listLanguage.Count() != 0)
                {
                    foreach (var lang in listLanguage)
                    {
                        _context.EmpLanguages.Remove(lang);
                    }
                    var result = await _context.SaveChangesAsync();
                    if (result == 0)
                    {
                        return new ApiErrorResult<bool>("Update Language Info failed");
                    }
                }
            }
            else
            {
                if (listLanguage.Count() != 0)
                {
                    foreach (var lang in listLanguage)
                    {
                        var check = false;
                        foreach (var l in request.Languages)
                        {
                            if (lang.LangID.Equals(l.LangID))
                            {
                                check = true;
                            }
                        }
                        if (check == false)
                        {
                            _context.EmpLanguages.Remove(lang);
                        }
                    }
                }
                foreach (var el in request.Languages)
                {
                    var empLang = new EmpLanguage()
                    {
                        EmpID = empID,
                        LangID = el.LangID,
                        LangLevel = el.LangLevel
                    };
                    var checkEmpLang = await _context.EmpLanguages.FindAsync(empID, empLang.LangID);
                    if (checkEmpLang == null)
                    {
                        _context.EmpLanguages.Add(empLang);
                    }
                    else
                    {
                        checkEmpLang.LangLevel = empLang.LangLevel;
                        _context.EmpLanguages.Update(checkEmpLang);
                    }
                }
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ApiErrorResult<bool>("Update Language Info failed");
                }
            }

            var query = from es in _context.EmpSkills
                        join s in _context.Skills on es.SkillID equals s.SkillID
                        select new { es, s };

            // Update SoftSkill
            var listSoftSkill = await query.Where(x => x.es.EmpID.Equals(empID) && x.s.SkillType.Equals(SkillType.SoftSkill))
                .Select(x => new EmpSkill()
                {
                    EmpID = x.es.EmpID,
                    SkillID = x.es.SkillID,
                    SkillLevel = x.es.SkillLevel,
                    DateStart = x.es.DateStart,
                    DateEnd = x.es.DateEnd
                }).ToListAsync();
            if (request.SoftSkills.Count() == 0)
            {
                if (listSoftSkill.Count() != 0)
                {
                    foreach (var skill in listSoftSkill)
                    {
                        _context.EmpSkills.Remove(skill);
                    }
                    var result = await _context.SaveChangesAsync();
                    if (result == 0)
                    {
                        return new ApiErrorResult<bool>("Update SoftSkill Info failed");
                    }
                }
            }
            else
            {
                if (listSoftSkill.Count() != 0)
                {
                    foreach (var skill in listSoftSkill)
                    {
                        var check = false;
                        foreach (var sk in request.SoftSkills)
                        {
                            if (skill.SkillID.Equals(sk))
                            {
                                check = true;
                            }
                        }
                        if (check == false)
                        {
                            _context.EmpSkills.Remove(skill);
                        }
                    }
                }
                foreach (var sk in request.SoftSkills)
                {
                    var checkEmpSkill = await _context.EmpSkills.FindAsync(empID, sk);
                    if (checkEmpSkill == null)
                    {
                        var empSkill = new EmpSkill()
                        {
                            EmpID = empID,
                            SkillID = sk
                        };
                        _context.EmpSkills.Add(empSkill);
                    }
                    else
                    {
                        _context.EmpSkills.Update(checkEmpSkill);
                    }
                }
                var result = await _context.SaveChangesAsync();
                if (result == 0)
                {
                    return new ApiErrorResult<bool>("Update SoftSkill Info failed");
                }
            }

            //Update HardSkill
            var listHardSkill = await query.Where(x => x.es.EmpID.Equals(empID) && x.s.SkillType.Equals(SkillType.HardSkill))
                .Select(x => new EmpSkill()
                {
                    EmpID = x.es.EmpID,
                    SkillID = x.es.SkillID,
                    SkillLevel = x.es.SkillLevel,
                    DateStart = x.es.DateStart,
                    DateEnd = x.es.DateEnd
                }).ToListAsync();
            if (request.HardSkills.Count() == 0)
            {
                if (listHardSkill.Count() != 0)
                {
                    foreach (var skill in listHardSkill)
                    {
                        skill.DateEnd = DateTime.Now;
                        _context.EmpSkills.Update(skill);
                    }
                    var result = await _context.SaveChangesAsync();
                    if (result == 0)
                    {
                        return new ApiErrorResult<bool>("Update Skill Info failed");
                    }
                }
            }
            else
            {
                var certiQuery = from ec in _context.EmpCertifications
                                 join c in _context.Certifications on ec.CertificationID equals c.CertificationID
                                 select new { ec, c };
                foreach (var skill in listHardSkill)
                {
                    var check = false;
                    foreach (var sk in request.HardSkills)
                    {
                        if (skill.SkillID.Equals(sk))
                        {
                            check = true;
                        }
                    }
                    if (check == false)
                    {
                        skill.DateEnd = DateTime.Now;
                        _context.EmpSkills.Update(skill);
                    }
                }
                foreach (var sk in request.HardSkills)
                {
                    var checkEmpSkill = await _context.EmpSkills.FindAsync(empID, sk.SkillID);
                    if (checkEmpSkill == null)
                    {
                        var empSkill = new EmpSkill()
                        {
                            EmpID = empID,
                            SkillID = sk.SkillID,
                            SkillLevel = (SkillLevel)sk.SkillLevel,
                            DateStart = DateTime.Now
                        };
                        _context.EmpSkills.Add(empSkill);
                    }
                    else
                    {
                        if (checkEmpSkill.DateEnd != null)
                        {
                            checkEmpSkill.DateEnd = null;
                        }
                        checkEmpSkill.SkillLevel = (SkillLevel)sk.SkillLevel;
                        _context.EmpSkills.Update(checkEmpSkill);
                    }
                    var result = await _context.SaveChangesAsync();
                    if (result == 0)
                    {
                        return new ApiErrorResult<bool>("Update Skill Info failed");
                    }

                    //Update Certification
                    var listCerti = await certiQuery.Where(x => x.ec.EmpID.Equals(empID) && x.c.SkillID.Equals(sk.SkillID))
                            .Select(x => new EmpCertification()
                            {
                                EmpID = x.ec.EmpID,
                                CertificationID = x.ec.CertificationID,
                                DateTaken = x.ec.DateTaken,
                                DateEnd = x.ec.DateEnd
                            }).ToListAsync();
                    if (sk.EmpCertifications.Count() == 0)
                    {
                        if (listCerti.Count() != 0)
                        {
                            foreach (var c in listCerti)
                            {
                                _context.EmpCertifications.Remove(c);
                            }
                            result = await _context.SaveChangesAsync();
                            if (result == 0)
                            {
                                return new ApiErrorResult<bool>("Update Certification Info failed");
                            }
                        }
                    }
                    else
                    {
                        if (listCerti.Count() != 0)
                        {
                            foreach (var c in listCerti)
                            {
                                var check = false;
                                foreach (var certi in sk.EmpCertifications)
                                {
                                    if (c.CertificationID.Equals(certi.CertiID))
                                    {
                                        check = true;
                                    }
                                }
                                if (check == false)
                                {
                                    _context.EmpCertifications.Remove(c);
                                }
                            }
                        }
                        foreach (var certi in sk.EmpCertifications)
                        {
                            var certification = await _context.Certifications.FindAsync(certi.CertiID);
                            var checkEmpCerti = await _context.EmpCertifications.FindAsync(empID, certi.CertiID);
                            if (checkEmpCerti == null)
                            {
                                if (DateTime.Compare(DateTime.Parse(certi.DateTaken).Date, DateTime.Today) < 0)
                                {
                                    return new ApiErrorResult<bool>("Certification " + certification.CertificationName + "-date taken is earlier than today");
                                }
                                var empCerti = new EmpCertification()
                                {
                                    EmpID = empID,
                                    CertificationID = certi.CertiID,
                                    DateTaken = DateTime.Parse(certi.DateTaken),
                                };
                                if (certi.DateEnd.Equals(""))
                                {
                                    empCerti.DateEnd = null;
                                }
                                else
                                {
                                    if (DateTime.Compare(DateTime.Parse(certi.DateEnd).Date, DateTime.Today) < 0)
                                    {
                                        return new ApiErrorResult<bool>("Certification " + certification.CertificationName + "-date expire is earlier than today");
                                    }
                                    if (DateTime.Compare(empCerti.DateTaken.Date, DateTime.Parse(certi.DateEnd).Date) > 0)
                                    {
                                        return new ApiErrorResult<bool>("Certification " + certification.CertificationName + "-date expire is earlier than date taken");
                                    }
                                    empCerti.DateEnd = DateTime.Parse(certi.DateEnd);
                                }
                                _context.EmpCertifications.Add(empCerti);
                            }
                            else
                            {
                                if (DateTime.Compare(checkEmpCerti.DateTaken.Date, DateTime.Parse(certi.DateTaken).Date) != 0)
                                {
                                    if (DateTime.Compare(DateTime.Parse(certi.DateTaken).Date, DateTime.Today) < 0)
                                    {
                                        return new ApiErrorResult<bool>("Certification " + certification.CertificationName + "-date taken is earlier than today");
                                    }
                                    if (!certi.DateEnd.Equals(""))
                                    {
                                        if (DateTime.Compare(DateTime.Parse(certi.DateTaken).Date, DateTime.Parse(certi.DateEnd).Date) > 0)
                                        {
                                            return new ApiErrorResult<bool>("Certification " + certification.CertificationName + "-date taken is later than date taken");
                                        }
                                    }
                                    checkEmpCerti.DateTaken = DateTime.Parse(certi.DateTaken);
                                }
                                if (certi.DateEnd.Equals(""))
                                {
                                    checkEmpCerti.DateEnd = null;
                                }
                                else
                                {
                                    if (checkEmpCerti.DateEnd != null)
                                    {
                                        if (DateTime.Compare(DateTime.Parse(checkEmpCerti.DateEnd.ToString()).Date, DateTime.Parse(certi.DateEnd).Date) != 0)
                                        {
                                            if (DateTime.Compare(DateTime.Parse(certi.DateEnd).Date, DateTime.Today) < 0)
                                            {
                                                return new ApiErrorResult<bool>("Certification " + certification.CertificationName + "-date expire is earlier than today");
                                            }
                                            if (DateTime.Compare(checkEmpCerti.DateTaken.Date, DateTime.Parse(certi.DateEnd).Date) > 0)
                                            {
                                                return new ApiErrorResult<bool>("Certification " + certification.CertificationName + "-date expire is earlier than date taken");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (DateTime.Compare(DateTime.Parse(certi.DateEnd).Date, DateTime.Today) < 0)
                                        {
                                            return new ApiErrorResult<bool>("Certification " + certification.CertificationName + "-date expire is earlier than today");
                                        }
                                        if (DateTime.Compare(checkEmpCerti.DateTaken.Date, DateTime.Parse(certi.DateEnd).Date) > 0)
                                        {
                                            return new ApiErrorResult<bool>("Certification " + certification.CertificationName + "-date expire is earlier than date taken");
                                        }
                                    }
                                    checkEmpCerti.DateEnd = DateTime.Parse(certi.DateEnd);
                                }
                                _context.EmpCertifications.Update(checkEmpCerti);
                            }
                        }
                        result = await _context.SaveChangesAsync();
                        if (result == 0)
                        {
                            return new ApiErrorResult<bool>("Update Certification Info failed");
                        }
                    }
                }
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<LoadEmpInfoVM>> LoadEmpInfo(string empID)
        {
            LoadEmpInfoVM info = new LoadEmpInfoVM();
            info.Languages = new List<EmpLanguageDetail>();
            info.SoftSkills = new List<int>();
            info.HardSkills = new List<EmpHardSkillVM>();
            info.Languages = await _context.EmpLanguages.Where(x => x.EmpID.Equals(empID)).Select(x => new EmpLanguageDetail()
            {
                LangID = x.LangID,
                LangLevel = x.LangLevel
            }).ToListAsync();
            var listSkill = await _context.EmpSkills.Where(x => x.EmpID.Equals(empID) && x.DateEnd == null)
                .Select(x => new EmpSkill()
                {
                    SkillID = x.SkillID,
                    SkillLevel = x.SkillLevel
                }).ToListAsync();
            var certiQuery = from ec in _context.EmpCertifications
                             join c in _context.Certifications on ec.CertificationID equals c.CertificationID
                             select new { ec, c };
            foreach (var s in listSkill)
            {
                var skill = await _context.Skills.FindAsync(s.SkillID);
                if (skill.SkillType == SkillType.SoftSkill)
                {
                    info.SoftSkills.Add(s.SkillID);
                }
                else
                {
                    var empCerti = certiQuery.Where(x => x.ec.EmpID.Equals(empID) && x.c.SkillID.Equals(s.SkillID))
                        .Select(x => new EmpCertificationDetail()
                        {
                            CertiID = x.ec.CertificationID,
                            DateEnd = x.ec.DateEnd.ToString(),
                            DateTaken = x.ec.DateTaken.ToString()
                        }).ToList();
                    foreach (var c in empCerti)
                    {
                        if (c.DateEnd == null)
                        {
                            c.DateEnd = "";
                        }
                    }
                    var certiList = _context.Certifications.Where(x => x.SkillID.Equals(s.SkillID))
                        .Select(x => new ListCertificationViewModel()
                        {
                            CertificationID = x.CertificationID,
                            CertificationName = x.CertificationName,
                            CertiLevel = x.CertiLevel
                        }).ToList();
                    EmpHardSkillVM hardSkill = new EmpHardSkillVM()
                    {
                        SkillID = skill.SkillID,
                        SkillLevel = (int)s.SkillLevel,
                        EmpCertifications = empCerti,
                        CertiList = certiList
                    };
                    info.HardSkills.Add(hardSkill);
                }
            }
            return new ApiSuccessResult<LoadEmpInfoVM>(info);
        }
    }
}