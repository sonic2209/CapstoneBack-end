using ESMS.BackendAPI.Services.Projects;
using ESMS.BackendAPI.Ultilities;
using ESMS.BackendAPI.ViewModels.Certification;
using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Employees;
using ESMS.BackendAPI.ViewModels.Employees.Suggestion;
using ESMS.BackendAPI.ViewModels.Employees.Suggestion.SingleCandidate;
using ESMS.BackendAPI.ViewModels.Position;
using ESMS.BackendAPI.ViewModels.Project;
using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Data.Enums;
using ESMS.ViewModels.System.Employees;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PasswordGenerator;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
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
        private readonly string FILE_LOCATION = Path.Combine($"Excel");

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

        public async Task<ApiResult<LoginVm>> Authenticate(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return new ApiErrorResult<LoginVm>("This account does not exist");

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
            if (!result.Succeeded)
            {
                return new ApiErrorResult<LoginVm>("Password is not correct");
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
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                UltilitiesService.AddOrUpdateError(errors, "UserName", "This username alreadys exists");
                //return new ApiErrorResult<string>("Username: This username already exists");
            }
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                UltilitiesService.AddOrUpdateError(errors, "Email", "This email already exists");
                //return new ApiErrorResult<string>("Email: This email already exists");
            }
            if (errors.Count > 0)
            {
                return new ApiErrorResult<string>(errors);
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
            var pwd = new Password().IncludeLowercase().IncludeUppercase().IncludeNumeric().LengthRequired(8);
            var presult = pwd.Next();
            string password = "Abcd1234";

            string errorMessage = null;
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, request.RoleName) == false)
                {
                    var addtoroleResult = await _userManager.AddToRoleAsync(user, request.RoleName);
                    if (!addtoroleResult.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            errorMessage += error.Description + Environment.NewLine;
                        }
                        return new ApiErrorResult<string>("Register failed: " + errorMessage);
                    }
                }
                user = await _userManager.FindByNameAsync(request.UserName);
                string empID = user.Id;
                return new ApiSuccessResult<string>(empID);
            }

            foreach (var error in result.Errors)
            {
                errorMessage += error.Description + Environment.NewLine;
            }
            return new ApiErrorResult<string>("Register failed: " + errorMessage);
        }

        public async Task<ApiResult<bool>> AddEmpPosition(string empID, AddEmpPositionRequest request)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            if (request.HardSkills.Count() == 0)
            {
                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Employee must have at least 1 hard skill");
            }
            else
            {
                bool checkHardSkill = false;
                foreach (var hardSkill in request.HardSkills)
                {
                    if (hardSkill.SkillID == 0)
                    {
                        if (checkHardSkill == false)
                        {
                            UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Please select hard skill");
                            checkHardSkill = true;
                        }
                    }
                    var skill = await _context.Skills.FindAsync(hardSkill.SkillID);
                    if (skill == null)
                    {
                        if (checkHardSkill == false)
                        {
                            UltilitiesService.AddOrUpdateError(errors, "HardSkills", "HardSkill not found");
                            checkHardSkill = true;
                        }
                        //return new ApiErrorResult<bool>("HardSkill not found");
                    }
                    else
                    {
                        if (skill.Status == false)
                        {
                            if (checkHardSkill == false)
                            {
                                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Skill:" + skill.SkillName + " is disable");
                                checkHardSkill = true;
                            }
                            //return new ApiErrorResult<bool>("Skill:" + skill.SkillName + " is disable");
                        }
                        if (hardSkill.SkillLevel <= 0)
                        {
                            if (checkHardSkill == false)
                            {
                                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Please select level for skill " + skill.SkillName);
                                checkHardSkill = true;
                            }
                            //return new ApiErrorResult<bool>("Please select level for skill " + skill.SkillName);
                        }
                        if (hardSkill.SkillLevel != (int)SkillLevel.BasicKnowledge
                           && hardSkill.SkillLevel != (int)SkillLevel.LimitedExperience
                           && hardSkill.SkillLevel != (int)SkillLevel.Practical
                           && hardSkill.SkillLevel != (int)SkillLevel.AppliedTheory
                           && hardSkill.SkillLevel != (int)SkillLevel.RecognizedAuthority)
                        {
                            if (checkHardSkill == false)
                            {
                                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Please select level from select box only for skill " + skill.SkillName);
                                checkHardSkill = true;
                            }
                            //return new ApiErrorResult<bool>("Please select level from select box only for skill " + skill.SkillName);
                        }
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
                                DateTime dateTaken = new DateTime();
                                DateTime dateEnd = new DateTime();
                                if (certification.CertiID == 0)
                                {
                                    if (checkHardSkill == false)
                                    {
                                        UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Please select certificate for skill " + skill.SkillName);
                                        checkHardSkill = true;
                                    }
                                    //return new ApiErrorResult<bool>("Please select certification for skill " + skill.SkillName);
                                }
                                var certi = await _context.Certifications.FindAsync(certification.CertiID);
                                if (certi == null)
                                {
                                    if (checkHardSkill == false)
                                    {
                                        UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificatie for skill " + skill.SkillName + " not found");
                                        checkHardSkill = true;
                                    }
                                    //return new ApiErrorResult<bool>("Certification for skill " + skill.SkillName + " not found");
                                }
                                else
                                {
                                    if (certi.Status == false)
                                    {
                                        if (checkHardSkill == false)
                                        {
                                            UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " is disable");
                                            checkHardSkill = true;
                                        }
                                        //return new ApiErrorResult<bool>("Certification:" + certi.CertificationName + " for skill " + skill.SkillName + " is disable");
                                    }
                                    if (certification.DateTaken.Equals(""))
                                    {
                                        if (checkHardSkill == false)
                                        {
                                            UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " - Please input date taken");
                                            checkHardSkill = true;
                                        }
                                        //return new ApiErrorResult<bool>("Certification " + certi.CertificationName + " for skill " + skill.SkillName + " - Please input date taken");
                                    }
                                    else if (!DateTime.TryParse(certification.DateTaken, out dateTaken))
                                    {
                                        if (checkHardSkill == false)
                                        {
                                            UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " - date taken is wrong format");
                                            checkHardSkill = true;
                                        }
                                    }
                                    else
                                    {
                                        dateTaken = DateTime.Parse(certification.DateTaken);
                                    }

                                    if (DateTime.Compare(dateTaken, DateTime.Today) > 0)
                                    {
                                        if (checkHardSkill == false)
                                        {
                                            UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " - date taken is after today");
                                            checkHardSkill = true;
                                        }
                                        //return new ApiErrorResult<bool>("Certification " + certi.CertificationName + " for skill " + skill.SkillName + " - date taken is after today");
                                    }
                                    var empCertification = new EmpCertification()
                                    {
                                        EmpID = empID,
                                        CertificationID = certification.CertiID,
                                        DateTaken = dateTaken
                                    };
                                    if (certification.DateEnd.Equals(""))
                                    {
                                        empCertification.DateEnd = null;
                                    }
                                    else
                                    {
                                        if (!DateTime.TryParse(certification.DateEnd, out dateEnd))
                                        {
                                            if (checkHardSkill == false)
                                            {
                                                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " - date end is wrong format");
                                                checkHardSkill = true;
                                            }
                                        }
                                        else
                                        {
                                            dateEnd = DateTime.Parse(certification.DateEnd);
                                        }

                                        if (DateTime.Compare(dateTaken.Date, dateEnd.Date) == 0)
                                        {
                                            if (checkHardSkill == false)
                                            {
                                                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " - date expire is same as date taken");
                                                checkHardSkill = true;
                                            }
                                            //return new ApiErrorResult<bool>("Certification " + certi.CertificationName + " for skill " + skill.SkillName + " - date expire is earlier than date taken");
                                        }
                                        if (DateTime.Compare(empCertification.DateTaken.Date, dateEnd.Date) > 0)
                                        {
                                            if (checkHardSkill == false)
                                            {
                                                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " - date expire is earlier than date taken");
                                                checkHardSkill = true;
                                            }
                                            //return new ApiErrorResult<bool>("Certification " + certi.CertificationName + " for skill " + skill.SkillName + " - date expire is earlier than date taken");
                                        }
                                        if (DateTime.Compare(dateEnd.Date, DateTime.Today) < 0)
                                        {
                                            if (checkHardSkill == false)
                                            {
                                                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " has expired");
                                                checkHardSkill = true;
                                            }
                                            //return new ApiErrorResult<bool>("Certification " + certi.CertificationName + " for skill " + skill.SkillName + " has expired");
                                        }
                                        empCertification.DateEnd = dateEnd;
                                    }
                                    _context.EmpCertifications.Add(empCertification);
                                }
                            }
                        }
                    }
                }
            }
            if (request.Languages.Count() == 0)
            {
                UltilitiesService.AddOrUpdateError(errors, "Languages", "Employee must have at least 1 language");
            }
            else
            {
                bool checkLanguage = false;
                foreach (var language in request.Languages)
                {
                    if (language.LangID == 0)
                    {
                        if (checkLanguage == false)
                        {
                            UltilitiesService.AddOrUpdateError(errors, "Languages", "Please select language");
                            checkLanguage = true;
                        }
                        //return new ApiErrorResult<bool>("Please select language");
                    }
                    var lang = await _context.Languages.FindAsync(language.LangID);
                    if (lang == null)
                    {
                        if (checkLanguage == false)
                        {
                            UltilitiesService.AddOrUpdateError(errors, "Languages", "Language not found");
                            checkLanguage = true;
                        }
                        //return new ApiErrorResult<bool>("Language not found");
                    }
                    else
                    {
                        if (language.LangLevel == 0)
                        {
                            if (checkLanguage == false)
                            {
                                UltilitiesService.AddOrUpdateError(errors, "Languages", "Please select level for language " + lang.LangName);
                                checkLanguage = true;
                            }
                            //return new ApiErrorResult<bool>("Please select level for language " + lang.LangName);
                        }
                        var empLanguage = new EmpLanguage()
                        {
                            EmpID = empID,
                            LangID = language.LangID,
                            LangLevel = language.LangLevel
                        };
                        _context.EmpLanguages.Add(empLanguage);
                    }
                }
            }
            if (request.SoftSkills.Count() == 0)
            {
                UltilitiesService.AddOrUpdateError(errors, "SoftSkills", "Employee must have at least 1 soft skill");
            }
            else
            {
                foreach (var softSkill in request.SoftSkills)
                {
                    bool checkSoftSkill = false;
                    if (softSkill == 0)
                    {
                        if (checkSoftSkill == false)
                        {
                            UltilitiesService.AddOrUpdateError(errors, "SoftSkills", "Please select soft skill");
                            checkSoftSkill = true;
                        }
                    }
                    var skill = await _context.Skills.FindAsync(softSkill);
                    if (skill == null)
                    {
                        if (checkSoftSkill == false)
                        {
                            UltilitiesService.AddOrUpdateError(errors, "SoftSkills", "SoftSkill not found");
                        }
                        //return new ApiErrorResult<bool>("SoftSkill not found");
                    }
                    else
                    {
                        if (skill.Status == false)
                        {
                            if (checkSoftSkill == false)
                            {
                                UltilitiesService.AddOrUpdateError(errors, "SoftSkills", "Skill:" + skill.SkillName + " is disable");
                            }
                            //return new ApiErrorResult<bool>("Skill:" + skill.SkillName + " is disable");
                        }
                        var empSoftSkill = new EmpSkill()
                        {
                            EmpID = empID,
                            SkillID = softSkill
                        };
                        _context.EmpSkills.Add(empSoftSkill);
                    }
                }
            }
            if (errors.Count() > 0)
            {
                return new ApiErrorResult<bool>(errors);
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
                Email = user.Email.ToLower(),
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
                var query = from u in _userManager.Users
                            join ur in _context.UserRoles on u.Id equals ur.UserId
                            join r in _context.Roles on ur.RoleId equals r.Id
                            select new { u, r, ur };
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query.Where(x => (x.u.UserName.ToLower().Contains(request.Keyword.ToLower()) || x.u.Name.ToLower().Contains(request.Keyword.ToLower()) || x.u.PhoneNumber.Contains(request.Keyword)
                    || x.u.Email.Contains(request.Keyword)) && x.r.Name == request.RoleName);
                }
                else
                {
                    query = query.Where(x => x.r.Name == request.RoleName);
                }
                //3.Paging
                int totalRow = await query.CountAsync();

                var data = await query.OrderByDescending(x => x.u.DateCreated).Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new EmpVm()
                    {
                        Email = x.u.Email.ToLower(),
                        PhoneNumber = x.u.PhoneNumber,
                        Name = x.u.Name,
                        Id = x.u.Id,
                        UserName = x.u.UserName,
                        DateCreated = x.u.DateCreated,
                        RoleName = x.r.Name
                    }).ToListAsync();
                List<EmpVm> dataFilter = new List<EmpVm>();
                //foreach (var empUser in data)
                //{
                //    var user = await _userManager.FindByIdAsync(empUser.Id.ToString());
                //    var roles = await _userManager.GetRolesAsync(user);
                //    string currentRole = null;
                //    if (roles.Count > 0)
                //    {
                //        currentRole = roles[0];
                //    }
                //    if (currentRole == request.RoleName)
                //    {
                //        empUser.RoleName = currentRole;
                //        dataFilter.Add(empUser);
                //    }
                //    //empUser.RoleName = currentRole;
                //}

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

        public ApiResult<PagedResult<MatchViewModel>> SuggestCandidatePaging(List<MatchViewModel> listMatch, GetSuggestEmpPagingRequest request)
        {
            {
                var query = listMatch;
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query.Where(x => x.EmpName.ToLower().Contains(request.Keyword.ToLower())).ToList();
                }
                //3.Paging
                int totalRow = query.Count;

                var data = query.Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                //4.Select and projection
                var pagedResult = new PagedResult<MatchViewModel>()
                {
                    TotalRecords = totalRow,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Items = data
                };
                return new ApiSuccessResult<PagedResult<MatchViewModel>>(pagedResult);
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
                                        Languagematch += (double)((empl.LangLevel * language.Priority * 0.1) / requiredPosition.Language.Count);
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
                                        Softskillmatch += (double)(10 / (requiredPosition.SoftSkillIDs.Count));
                                    }
                                }
                                //match += Math.Round(Softskillmatch, 2);
                            }
                            //add match vao hardskill
                            var listEmpHardSkillquery = listEmpSkillquery.Where(x => x.s.SkillType == SkillType.HardSkill && x.es.DateEnd == null && x.es.EmpID.Equals(emp.EmpId));
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
                                        var listCertiSkill = await certiquery.Where(x => x.ec.EmpID.Equals(emphs.EmpID) && x.c.SkillID == emphs.SkillID && (x.ec.DateEnd > DateTime.Now || x.ec.DateEnd == null)).Select(x => new CertiInSkill
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

                            var currentProjectDate = await _context.Projects.Where(x => x.ProjectID == projectID).Select(x => new Project()
                            {
                                DateBegin = x.DateBegin,
                                DateEstimatedEnd = x.DateEstimatedEnd
                            }).FirstOrDefaultAsync();
                            DateTime dateBegin = currentProjectDate.DateBegin;
                            DateTime dateEstimatedEnd = currentProjectDate.DateEstimatedEnd;
                            var listProjectCurrentlyIn = await projectquery.Where(x => x.p.Status != ProjectStatus.Finished && x.epip.EmpID.Equals(emp.EmpId) && x.epip.Status != ConfirmStatus.Reject).OrderBy(x => x.p.DateEstimatedEnd)
                                .Select(x => new Project()
                                {
                                    ProjectID = x.p.ProjectID,
                                    DateBegin = x.p.DateBegin,
                                    DateEstimatedEnd = x.p.DateEstimatedEnd
                                }).ToListAsync();
                            //var projectOnGoingDateEnd = await projectquery.Where(x => (x.p.Status == ProjectStatus.OnGoing || x.p.Status == ProjectStatus.Confirmed) && x.epip.EmpID.Equals(emp.EmpId)).Select(x => x.p.DateEstimatedEnd).ToListAsync();
                            bool checkProjectDate = false;
                            var check = true;
                            if (listProjectCurrentlyIn.Count > 0)
                            {
                                for (int i = 0; i < listProjectCurrentlyIn.Count(); i++)
                                {
                                    if (listProjectCurrentlyIn[i].ProjectID == projectID)
                                    {
                                        checkProjectDate = true;
                                        break;
                                    }
                                    if (DateTime.Compare(listProjectCurrentlyIn[i].DateBegin, dateBegin) >= 0)
                                    {
                                        check = false;
                                    }
                                    if (check == false)
                                    {
                                        if (i == 0)
                                        {
                                            if (DateTime.Compare(dateEstimatedEnd, listProjectCurrentlyIn[i].DateBegin.AddDays(-3)) > 0)
                                            {
                                                checkProjectDate = true;
                                                break;
                                            }
                                        }
                                        if (i > 0)
                                        {
                                            if (DateTime.Compare(dateBegin, listProjectCurrentlyIn[i - 1].DateEstimatedEnd.AddDays(3)) < 0)
                                            {
                                                checkProjectDate = true;
                                                break;
                                            }
                                            if (DateTime.Compare(dateEstimatedEnd, listProjectCurrentlyIn[i].DateBegin.AddDays(-3)) > 0)
                                            {
                                                checkProjectDate = true;
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                                if (check == true)
                                {
                                    if (DateTime.Compare(dateBegin, listProjectCurrentlyIn[listProjectCurrentlyIn.Count() - 1].DateEstimatedEnd.AddDays(3)) < 0)
                                    {
                                        checkProjectDate = true;
                                    }
                                }
                            }
                            if (checkProjectDate == true)
                            {
                                continue;
                            }
                            //Add match theo projecttype
                            var listProjectWithType = await projectquery.Where(x => x.p.ProjectTypeID == ProjectTypeID && x.epip.EmpID.Equals(emp.EmpId) && x.epip.Status == ConfirmStatus.Accept).Select(x => x.p.ProjectID).ToListAsync();
                            var numberOfProjectWithType = listProjectWithType.Count();
                            if (numberOfProjectWithType == 0)
                            {
                                ProjectTypeMatch = 0;
                            }
                            if (numberOfProjectWithType >= 1 && numberOfProjectWithType < 5)
                            {
                                ProjectTypeMatch = 3;
                                //match += ProjectTypeMatch;
                            }
                            if (numberOfProjectWithType >= 5 && numberOfProjectWithType < 10)
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
                            if (numberOfProjectWithField >= 1 && numberOfProjectWithField < 5)
                            {
                                ProjectFieldMatch = 3;
                                //match += ProjectFieldMatch;
                            }
                            if (numberOfProjectWithField >= 5 && numberOfProjectWithField < 10)
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
                            if (Hardskillmatch <= 0 || match <= 0)
                            {
                                continue;
                            }
                            matchDetail = new MatchViewModel()
                            {
                                EmpID = emp.EmpId,
                                EmpName = emp.EmpName,
                                LanguageMatch = Math.Round(Languagematch, 2),
                                SoftSkillMatch = Math.Round(Softskillmatch, 2),
                                HardSkillMatch = Math.Round(Hardskillmatch, 2),
                                ProjectTypeMatch = ProjectTypeMatch,
                                ProjectFieldMatch = ProjectFieldMatch,
                                OverallMatch = match,
                            };
                            listMatchDetail.Add(matchDetail);
                        }
                    }
                    listMatchDetail = listMatchDetail.OrderByDescending(x => x.OverallMatch).ToList();
                    if (listMatchDetail.Count < requiredPosition.CandidateNeeded)
                    {
                        foreach (var matchDetail in listMatchDetail)
                        {
                            matchDetail.IsHighest = true;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < requiredPosition.CandidateNeeded; i++)
                        {
                            if (listMatchDetail[i] != null)
                            {
                                listMatchDetail[i].IsHighest = true;
                            }
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

        public async Task<ApiResult<List<CandidateViewModel>>> SuggestCandidateWithoutMinimumPoint(int projectID, SuggestCadidateRequest request)
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
                                        var listCertiSkill = await certiquery.Where(x => x.ec.EmpID.Equals(emphs.EmpID) && x.c.SkillID == emphs.SkillID && x.ec.DateEnd > DateTime.Now).Select(x => new CertiInSkill
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
                            //if (Hardskillmatch <= 0 || match <= 0)
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

        public async Task<ApiResult<List<ProjectVM>>> SingleCandidateSuggest(string empID)
        {
            var empName = _context.Employees.Where(x => x.Id.Equals(empID)).Select(x => x.Name).FirstOrDefault();
            var listProject = _projectService.GetMissEmpProjects(empID);
            List<ProjectVM> listResult = new List<ProjectVM>();

            foreach (var project in listProject)
            {
                int ProjectTypeID = project.TypeID;
                int ProjectFieldID = project.FieldID;
                List<RequiredPosVM> listMatchInPosDetail = new List<RequiredPosVM>();

                //Loc nhung project ko available dua theo thoi gian ket thuc du an dang tien hanh
                var projectquery = from p in _context.Projects
                                   join rp in _context.RequiredPositions on p.ProjectID equals rp.ProjectID
                                   join epip in _context.EmpPositionInProjects on rp.ID equals epip.RequiredPositionID
                                   select new { p, epip, rp };

                var currentProjectDate = await _context.Projects.Where(x => x.ProjectID == project.ProjectID).Select(x => new Project()
                {
                    DateBegin = x.DateBegin,
                    DateEstimatedEnd = x.DateEstimatedEnd
                }).FirstOrDefaultAsync();
                DateTime dateBegin = currentProjectDate.DateBegin;
                DateTime dateEstimatedEnd = currentProjectDate.DateEstimatedEnd;
                var listProjectCurrentlyIn = await projectquery.Where(x => x.p.Status != ProjectStatus.Finished && x.epip.EmpID.Equals(empID) && x.epip.Status != ConfirmStatus.Reject).OrderBy(x => x.p.DateEstimatedEnd).Select(x => new Project()
                {
                    ProjectID = x.p.ProjectID,
                    DateBegin = x.p.DateBegin,
                    DateEstimatedEnd = x.p.DateEstimatedEnd
                }).ToListAsync();
                //var projectOnGoingDateEnd = await projectquery.Where(x => (x.p.Status == ProjectStatus.OnGoing || x.p.Status == ProjectStatus.Confirmed) && x.epip.EmpID.Equals(emp.EmpId)).Select(x => x.p.DateEstimatedEnd).ToListAsync();
                bool checkProjectDate = false;
                var check = true;
                if (listProjectCurrentlyIn.Count > 0)
                {
                    for (int i = 0; i < listProjectCurrentlyIn.Count(); i++)
                    {
                        if (listProjectCurrentlyIn[i].ProjectID == project.ProjectID)
                        {
                            checkProjectDate = true;
                            break;
                        }
                        if (DateTime.Compare(listProjectCurrentlyIn[i].DateBegin, dateBegin) >= 0)
                        {
                            check = false;
                        }
                        if (check == false)
                        {
                            if (i == 0)
                            {
                                if (DateTime.Compare(dateEstimatedEnd, listProjectCurrentlyIn[i].DateBegin.AddDays(-3)) > 0)
                                {
                                    checkProjectDate = true;
                                    break;
                                }
                            }
                            if (i > 0)
                            {
                                if (DateTime.Compare(dateBegin, listProjectCurrentlyIn[i - 1].DateEstimatedEnd.AddDays(3)) < 0)
                                {
                                    checkProjectDate = true;
                                    break;
                                }
                                if (DateTime.Compare(dateEstimatedEnd, listProjectCurrentlyIn[i].DateBegin.AddDays(-3)) > 0)
                                {
                                    checkProjectDate = true;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    if (check == true)
                    {
                        if (DateTime.Compare(dateBegin, listProjectCurrentlyIn[listProjectCurrentlyIn.Count() - 1].DateEstimatedEnd.AddDays(3)) < 0)
                        {
                            checkProjectDate = true;
                        }
                    }
                }

                //    foreach (var dateEnd in listProjectCurrentlyIn)
                //    {
                //        if (dateEnd > currentProjectBeginDate)
                //        {
                //            checkProjectDate = true;
                //            break;
                //        }
                //    }
                if (checkProjectDate == true)
                {
                    continue;
                }
                foreach (RequiredPosVM requiredPosition in project.RequiredPositions)
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
                            return new ApiErrorResult<List<ProjectVM>>("This user is a PM or HR");
                        }
                    }
                    else
                        return new ApiErrorResult<List<ProjectVM>>("This user is a PM or HR");
                    double match = 0;
                    double Languagematch = 0;
                    double Softskillmatch = 0;
                    double Hardskillmatch = 0;
                    double ProjectTypeMatch = 0;
                    double ProjectFieldMatch = 0;
                    MatchViewModel matchDetail = new MatchViewModel();

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
                                Languagematch += (double)((empl.LangLevel * language.Priority * 0.1) / requiredPosition.Language.Count);
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
                                Softskillmatch += (double)(10 / (requiredPosition.SoftSkillIDs.Count));
                            }
                        }
                    }
                    //add match vao hardskill
                    var listEmpHardSkillquery = listEmpSkillquery.Where(x => x.s.SkillType == SkillType.HardSkill && x.es.DateEnd == null && x.es.EmpID.Equals(empID));
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
                                var listCertiSkill = await certiquery.Where(x => x.ec.EmpID.Equals(emphs.EmpID) && x.c.SkillID == emphs.SkillID && (x.ec.DateEnd > DateTime.Now || x.ec.DateEnd == null)).Select(x => new CertiInSkill
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
                    if (numberOfProjectWithType >= 1 && numberOfProjectWithType < 5)
                    {
                        ProjectTypeMatch = 3;
                    }
                    if (numberOfProjectWithType >= 5 && numberOfProjectWithType < 10)
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
                    if (numberOfProjectWithField >= 1 && numberOfProjectWithField < 5)
                    {
                        ProjectFieldMatch = 3;
                    }
                    if (numberOfProjectWithField >= 5 && numberOfProjectWithField < 10)
                    {
                        ProjectFieldMatch = 6;
                    }
                    if (numberOfProjectWithField > 9)
                    {
                        ProjectFieldMatch = 10;
                    }
                    match = Math.Round(Languagematch + Softskillmatch + Hardskillmatch + ProjectTypeMatch + ProjectFieldMatch, 2);
                    if (Hardskillmatch <= 0 || match <= 0)
                    {
                        continue;
                    }
                    matchDetail = new MatchViewModel()
                    {
                        EmpID = empID,
                        EmpName = empName,
                        LanguageMatch = Math.Round(Languagematch, 2),
                        SoftSkillMatch = Math.Round(Softskillmatch, 2),
                        HardSkillMatch = Math.Round(Hardskillmatch, 2),
                        ProjectTypeMatch = ProjectTypeMatch,
                        ProjectFieldMatch = ProjectFieldMatch,
                        OverallMatch = match,
                    };
                    RequiredPosVM requiredPos = new RequiredPosVM()
                    {
                        RequiredPosID = requiredPosition.RequiredPosID,
                        CandidateNeeded = requiredPosition.CandidateNeeded,
                        HardSkills = requiredPosition.HardSkills,
                        SoftSkillIDs = requiredPosition.SoftSkillIDs,
                        Language = requiredPosition.Language,
                        MissingEmployee = requiredPosition.MissingEmployee,
                        PosID = requiredPosition.PosID,
                        PosName = requiredPosition.PosName,
                        MatchDetail = matchDetail
                    };
                    listMatchInPosDetail.Add(requiredPos);
                }
                if (listMatchInPosDetail.Count > 0)
                {
                    ProjectVM result = new ProjectVM()
                    {
                        ProjectID = project.ProjectID,
                        FieldID = project.FieldID,
                        ProjectManagerID = project.ProjectManagerID,
                        ProjectName = project.ProjectName,
                        TypeID = project.TypeID,
                        RequiredPositions = listMatchInPosDetail
                    };
                    listResult.Add(result);
                }
                //};
                //listMatchInPosDetail.Add(matchDetail);
            }
            //result.Add(new SingleCandidateViewModel()
            //{
            //    ProjectInfo = project,
            //    MatchInEachPos = listMatchInPosDetail,
            //});
            //}
            return new ApiSuccessResult<List<ProjectVM>>(listResult);
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
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

            if (await _userManager.Users.AnyAsync(x => x.Email.Equals(request.Email) && x.Id != id))
            {
                UltilitiesService.AddOrUpdateError(errors, "Email", "Email already exists");
            }
            if (errors.Count > 0)
            {
                return new ApiErrorResult<bool>(errors);
            }
            var user = await _userManager.FindByIdAsync(id.ToString());
            user.IdentityNumber = request.IdentityNumber;
            user.Address = request.Address;
            user.Email = request.Email;
            user.Name = request.Name;
            user.PhoneNumber = request.PhoneNumber;
            var result = await _userManager.UpdateAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            string errorMessage = null;
            if (await _userManager.IsInRoleAsync(user, request.RoleName) == false)
            {
                foreach (string rolename in roles)
                {
                    if (await _userManager.IsInRoleAsync(user, rolename) == true)
                    {
                        var removefromroleResult = await _userManager.RemoveFromRoleAsync(user, rolename);
                        if (!removefromroleResult.Succeeded)
                        {
                            foreach (var error in result.Errors)
                            {
                                errorMessage += error.Description + Environment.NewLine;
                            }
                            return new ApiErrorResult<bool>("Update user failed: " + errorMessage);
                        }
                    }
                }
                var addtoroleResult = await _userManager.AddToRoleAsync(user, request.RoleName);
                if (!addtoroleResult.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        errorMessage += error.Description + Environment.NewLine;
                    }
                    return new ApiErrorResult<bool>("Update user failed: " + errorMessage);
                }
            }
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            foreach (var error in result.Errors)
            {
                errorMessage += error.Description + Environment.NewLine;
            }
            return new ApiErrorResult<bool>("Update user failed: " + errorMessage);
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
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            var query = from es in _context.EmpSkills
                        join s in _context.Skills on es.SkillID equals s.SkillID
                        select new { es, s };
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
                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Employee must have at least 1 hard skill");
                //if (listHardSkill.Count() != 0)
                //{
                //    foreach (var skill in listHardSkill)
                //    {
                //        skill.DateEnd = DateTime.Now;
                //        _context.EmpSkills.Update(skill);
                //    }
                //}
            }
            else
            {
                bool checkHardSkill = false;
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
                foreach (var hardSkill in request.HardSkills)
                {
                    if (hardSkill.SkillID == 0)
                    {
                        if (checkHardSkill == false)
                        {
                            UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Please select hard skill");
                            checkHardSkill = true;
                        }
                    }
                    var skill = await _context.Skills.FindAsync(hardSkill.SkillID);
                    if (skill == null)
                    {
                        if (checkHardSkill == false)
                        {
                            UltilitiesService.AddOrUpdateError(errors, "HardSkills", "HardSkill not found");
                            checkHardSkill = true;
                        }
                        //return new ApiErrorResult<bool>("HardSkill not found");
                    }
                    else
                    {
                        if (skill.Status == false)
                        {
                            if (checkHardSkill == false)
                            {
                                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Skill:" + skill.SkillName + " is disable");
                                checkHardSkill = true;
                            }
                            //return new ApiErrorResult<bool>("Skill:" + skill.SkillName + " is disable");
                        }
                        if (hardSkill.SkillLevel <= 0)
                        {
                            if (checkHardSkill == false)
                            {
                                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Please select level for skill " + skill.SkillName);
                                checkHardSkill = true;
                            }
                            //return new ApiErrorResult<bool>("Please select level for skill " + skill.SkillName);
                        }
                        if (hardSkill.SkillLevel != (int)SkillLevel.BasicKnowledge
                           && hardSkill.SkillLevel != (int)SkillLevel.LimitedExperience
                           && hardSkill.SkillLevel != (int)SkillLevel.Practical
                           && hardSkill.SkillLevel != (int)SkillLevel.AppliedTheory
                           && hardSkill.SkillLevel != (int)SkillLevel.RecognizedAuthority)
                        {
                            if (checkHardSkill == false)
                            {
                                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Please select level from select box only for skill " + skill.SkillName);
                                checkHardSkill = true;
                            }
                            //return new ApiErrorResult<bool>("Please select level from select box only for skill " + skill.SkillName);
                        }
                        var checkEmpSkill = await _context.EmpSkills.FindAsync(empID, hardSkill.SkillID);
                        if (checkEmpSkill == null)
                        {
                            var empSkill = new EmpSkill()
                            {
                                EmpID = empID,
                                SkillID = hardSkill.SkillID,
                                SkillLevel = (SkillLevel)hardSkill.SkillLevel,
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
                            checkEmpSkill.SkillLevel = (SkillLevel)hardSkill.SkillLevel;
                            _context.EmpSkills.Update(checkEmpSkill);
                        }
                        //Update Certification for that skill
                        var certiQuery = from ec in _context.EmpCertifications
                                         join c in _context.Certifications on ec.CertificationID equals c.CertificationID
                                         select new { ec, c };
                        var listCerti = await certiQuery.Where(x => x.ec.EmpID.Equals(empID) && x.c.SkillID.Equals(hardSkill.SkillID))
                                    .Select(x => new EmpCertification()
                                    {
                                        EmpID = x.ec.EmpID,
                                        CertificationID = x.ec.CertificationID,
                                        DateTaken = x.ec.DateTaken,
                                        DateEnd = x.ec.DateEnd
                                    }).ToListAsync();
                        if (hardSkill.EmpCertifications.Count() == 0)
                        {
                            if (listCerti.Count() != 0)
                            {
                                foreach (var c in listCerti)
                                {
                                    _context.EmpCertifications.Remove(c);
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
                                    foreach (var certi in hardSkill.EmpCertifications)
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
                            foreach (var certification in hardSkill.EmpCertifications)
                            {
                                DateTime dateTaken = new DateTime();
                                DateTime dateEnd = new DateTime();
                                if (certification.CertiID == 0)
                                {
                                    if (checkHardSkill == false)
                                    {
                                        UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Please select certificate for skill " + skill.SkillName);
                                        checkHardSkill = true;
                                    }
                                    //return new ApiErrorResult<bool>("Please select certification for skill " + skill.SkillName);
                                }
                                var certi = await _context.Certifications.FindAsync(certification.CertiID);
                                if (certi == null)
                                {
                                    if (checkHardSkill == false)
                                    {
                                        UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate for skill " + skill.SkillName + " not found");
                                        checkHardSkill = true;
                                    }
                                    //return new ApiErrorResult<bool>("Certification for skill " + skill.SkillName + " not found");
                                }
                                else
                                {
                                    if (certi.Status == false)
                                    {
                                        if (checkHardSkill == false)
                                        {
                                            UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " is disable");
                                            checkHardSkill = true;
                                        }
                                        //return new ApiErrorResult<bool>("Certification:" + certi.CertificationName + " for skill " + skill.SkillName + " is disable");
                                    }
                                    if (certification.DateTaken.Equals(""))
                                    {
                                        if (checkHardSkill == false)
                                        {
                                            UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " - Please input date taken");
                                            checkHardSkill = true;
                                        }
                                        //return new ApiErrorResult<bool>("Certification " + certi.CertificationName + " for skill " + skill.SkillName + " - Please input date taken");
                                    }
                                    else if (!DateTime.TryParse(certification.DateTaken, out dateTaken))
                                    {
                                        if (checkHardSkill == false)
                                        {
                                            UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " - date taken is wrong format");
                                            checkHardSkill = true;
                                        }
                                    }
                                    else
                                    {
                                        dateTaken = DateTime.Parse(certification.DateTaken);
                                    }
                                    if (DateTime.Compare(dateTaken, DateTime.Today) > 0)
                                    {
                                        if (checkHardSkill == false)
                                        {
                                            UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " - date taken is after today");
                                            checkHardSkill = true;
                                        }
                                        //return new ApiErrorResult<bool>("Certification " + certi.CertificationName + " for skill " + skill.SkillName + " - date taken is after today");
                                    }

                                    if (!certification.DateEnd.Equals(""))
                                    {
                                        if (!DateTime.TryParse(certification.DateEnd, out dateEnd))
                                        {
                                            if (checkHardSkill == false)
                                            {
                                                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " - date end is wrong format");
                                                checkHardSkill = true;
                                            }
                                        }
                                        else
                                        {
                                            dateEnd = DateTime.Parse(certification.DateEnd);
                                        }
                                        if (DateTime.Compare(dateTaken.Date, dateEnd.Date) > 0)
                                        {
                                            if (checkHardSkill == false)
                                            {
                                                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " - date expire is earlier than date taken");
                                                checkHardSkill = true;
                                            }
                                            //return new ApiErrorResult<bool>("Certification " + certi.CertificationName + " for skill " + skill.SkillName + " - date expire is earlier than date taken");
                                        }
                                        if (DateTime.Compare(dateTaken.Date, dateEnd.Date) == 0)
                                        {
                                            if (checkHardSkill == false)
                                            {
                                                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " - date expire is same as date taken");
                                                checkHardSkill = true;
                                            }
                                            //return new ApiErrorResult<bool>("Certification " + certi.CertificationName + " for skill " + skill.SkillName + " - date expire is earlier than date taken");
                                        }
                                        if (DateTime.Compare(dateEnd.Date, DateTime.Today) < 0)
                                        {
                                            if (checkHardSkill == false)
                                            {
                                                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " has expired");
                                                checkHardSkill = true;
                                            }
                                            //return new ApiErrorResult<bool>("Certification " + certi.CertificationName + " for skill " + skill.SkillName + " has expired");
                                        }
                                    }
                                    var checkEmpCerti = await _context.EmpCertifications.FindAsync(empID, certification.CertiID);
                                    if (checkEmpCerti == null)
                                    {
                                        var empCerti = new EmpCertification()
                                        {
                                            EmpID = empID,
                                            CertificationID = certification.CertiID,
                                            DateTaken = dateTaken,
                                        };
                                        if (certification.DateEnd.Equals(""))
                                        {
                                            empCerti.DateEnd = null;
                                        }
                                        else
                                        {
                                            empCerti.DateEnd = dateEnd;
                                        }
                                        _context.EmpCertifications.Add(empCerti);
                                    }
                                    else
                                    {
                                        checkEmpCerti.DateTaken = dateTaken;
                                        //if (DateTime.Compare(checkEmpCerti.DateTaken.Date, dateTaken.Date) != 0)
                                        //{
                                        //    if (!certification.DateEnd.Equals(""))
                                        //    {
                                        //        if (DateTime.Compare(dateTaken.Date, dateEnd.Date) > 0)
                                        //        {
                                        //            if (checkHardSkill == false)
                                        //            {
                                        //                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " - date taken is later than date expired");
                                        //                checkHardSkill = true;
                                        //            }
                                        //            //return new ApiErrorResult<bool>("Certification " + certi.CertificationName + " for skill " + skill.SkillName + " - date taken is later than date expired");
                                        //        }
                                        //    }
                                        //}
                                        if (certification.DateEnd.Equals(""))
                                        {
                                            checkEmpCerti.DateEnd = null;
                                        }
                                        else
                                        {
                                            //if (checkEmpCerti.DateEnd != null)
                                            //{
                                            //    if (DateTime.Compare(DateTime.Parse(checkEmpCerti.DateEnd.ToString()).Date, dateEnd.Date) != 0)
                                            //    {
                                            //        if (DateTime.Compare(checkEmpCerti.DateTaken.Date, dateEnd.Date) > 0)
                                            //        {
                                            //            if (checkHardSkill == false)
                                            //            {
                                            //                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " - date expire is earlier than date taken");
                                            //                checkHardSkill = true;
                                            //            }
                                            //            //return new ApiErrorResult<bool>("Certification " + certi.CertificationName + " for skill " + skill.SkillName + " - date expire is earlier than date taken");
                                            //        }
                                            //        if (DateTime.Compare(dateEnd.Date, DateTime.Today) < 0)
                                            //        {
                                            //            if (checkHardSkill == false)
                                            //            {
                                            //                UltilitiesService.AddOrUpdateError(errors, "HardSkills", "Certificate " + certi.CertificationName + " for skill " + skill.SkillName + " has expired");
                                            //                checkHardSkill = true;
                                            //            }
                                            //            //return new ApiErrorResult<bool>("Certification " + certi.CertificationName + " for skill " + skill.SkillName + " has expired");
                                            //        }
                                            //    }
                                            //}
                                            checkEmpCerti.DateEnd = dateEnd;
                                        }
                                        _context.EmpCertifications.Update(checkEmpCerti);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Update Language
            var listLanguage = await _context.EmpLanguages.Where(x => x.EmpID.Equals(empID)).Select(x => new EmpLanguage()
            {
                EmpID = x.EmpID,
                LangID = x.LangID,
                LangLevel = x.LangLevel
            }).ToListAsync();
            if (request.Languages.Count() == 0)
            {
                UltilitiesService.AddOrUpdateError(errors, "Languages", "Employee must have at least 1 language");
                //if (listLanguage.Count() != 0)
                //{
                //    foreach (var lang in listLanguage)
                //    {
                //        _context.EmpLanguages.Remove(lang);
                //    }
                //}
            }
            else
            {
                var checkLanguage = false;
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
                foreach (var language in request.Languages)
                {
                    if (language.LangID == 0)
                    {
                        if (checkLanguage == false)
                        {
                            UltilitiesService.AddOrUpdateError(errors, "Languages", "Please select language");
                            checkLanguage = true;
                        }
                        //return new ApiErrorResult<bool>("Please select language");
                    }
                    var lang = await _context.Languages.FindAsync(language.LangID);
                    if (lang == null)
                    {
                        if (checkLanguage == false)
                        {
                            UltilitiesService.AddOrUpdateError(errors, "Languages", "Language not found");
                            checkLanguage = true;
                        }
                        //return new ApiErrorResult<bool>("Language not found");
                    }
                    else
                    {
                        if (language.LangLevel == 0)
                        {
                            if (checkLanguage == false)
                            {
                                UltilitiesService.AddOrUpdateError(errors, "Languages", "Please select level for language " + lang.LangName);
                                checkLanguage = true;
                            }
                            //return new ApiErrorResult<bool>("Please select level for language " + lang.LangName);
                        }
                        var empLang = new EmpLanguage()
                        {
                            EmpID = empID,
                            LangID = language.LangID,
                            LangLevel = language.LangLevel
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
                }
            }

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
                UltilitiesService.AddOrUpdateError(errors, "SoftSkills", "Employee must have at least 1 soft skill");
                //if (listSoftSkill.Count() != 0)
                //{
                //    foreach (var skill in listSoftSkill)
                //    {
                //        _context.EmpSkills.Remove(skill);
                //    }
                //}
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
                foreach (var softSkill in request.SoftSkills)
                {
                    var checkSoftSkill = false;
                    if (softSkill == 0)
                    {
                        if (checkSoftSkill == false)
                        {
                            UltilitiesService.AddOrUpdateError(errors, "SoftSkills", "Please select soft skill");
                            checkSoftSkill = true;
                        }
                    }
                    var skill = await _context.Skills.FindAsync(softSkill);
                    if (skill == null)
                    {
                        if (checkSoftSkill == false)
                        {
                            UltilitiesService.AddOrUpdateError(errors, "SoftSkills", "SoftSkill not found");
                        }
                        //return new ApiErrorResult<bool>("SoftSkill not found");
                    }
                    else
                    {
                        if (skill.Status == false)
                        {
                            if (checkSoftSkill == false)
                            {
                                UltilitiesService.AddOrUpdateError(errors, "SoftSkills", "Skill:" + skill.SkillName + " is disable");
                            }
                            //return new ApiErrorResult<bool>("Skill:" + skill.SkillName + " is disable");
                        }
                        var checkEmpSkill = await _context.EmpSkills.FindAsync(empID, softSkill);
                        if (checkEmpSkill == null)
                        {
                            var empSkill = new EmpSkill()
                            {
                                EmpID = empID,
                                SkillID = softSkill
                            };
                            _context.EmpSkills.Add(empSkill);
                        }
                        else
                        {
                            _context.EmpSkills.Update(checkEmpSkill);
                        }
                    }
                }
            }
            if (errors.Count() != 0)
            {
                return new ApiErrorResult<bool>(errors);
            }

            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update Employee Info failed");
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
                        .OrderBy(x => x.CertiLevel).Select(x => new ListCertificationViewModel()
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

        public async Task<ApiResult<bool>> ChangePassword(string id, ChangePasswordRequest request)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return new ApiErrorResult<bool>("This account does not exist");
            }
            var result1 = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
            if (result1 != PasswordVerificationResult.Success)
            {
                UltilitiesService.AddOrUpdateError(errors, "CurrentPassword", "Your current password is not correct");
            }
            if (errors.Count > 0)
            {
                return new ApiErrorResult<bool>(errors);
            }

            string errorMessage = null;

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            foreach (var error in result.Errors)
            {
                errorMessage += error.Description + Environment.NewLine;
            }
            return new ApiErrorResult<bool>("Change password failed" + errorMessage);
        }
        public async Task<string> HandleFile(IFormFile file, string productId)
        {
            //if (file.Length > MaxFileSize)
            //{
            //    throw new Exception("File size too large. File name: " + file.FileName);
            //}

            string fileType = "image/jpeg";
            if (file.ContentType != null && !string.IsNullOrEmpty(file.ContentType))
            {
                fileType = file.ContentType;
            }

            //var image = new ProductImage()
            //{
            //    ProductId = productId,
            //    FileExtension = Path.GetExtension(file.FileName),
            //    FileType = fileType
            //};

            //_dbContext.ProductImages.Add(image);

            var pictureName = $"{productId}{Path.GetExtension(file.FileName)}";

            using (var fileStream = new FileStream(Path.Combine(FILE_LOCATION, pictureName), FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
            return productId;
        }

        public FileModel GetFileById()
        {
            var result = new FileModel();
            //var productImage = _dbContext.ProductImages.FirstOrDefault(e => e.Id == fileId);

            //if (productImage == null)
            //{
            //    throw new Exception("Invalid Id");
            //}

            var data = File.ReadAllBytes(FILE_LOCATION + "/" + "Book1.xlsx");
            result.FileName = "Tuan";
            result.Id = "Tuan";
            result.Data = data;
            result.FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return result;
        }
        private async Task<EmpVm> GetEmpById(string id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return null;
            }
            var empVm = new EmpVm()
            {
                Email = user.Email.ToLower(),
                PhoneNumber = user.PhoneNumber,
                Name = user.Name,
                Address = user.Address,
                IdentityNumber = user.IdentityNumber,
                UserName = user.UserName,
            };
            return empVm;
        }
        public async Task<FileModel> ExportEmployeeInfo(string id)
        {
            var result = new FileModel();
            var user = await GetEmpById(id);
            var excelName = "export.xlsx";
            var excelPath = Path.Combine(FILE_LOCATION, excelName);

            ExcelService.CreateExcelFile(excelPath, "Sheet1");
            ExcelService.InsertTextExistingExcel(excelPath, user.Name, "A", 2);
            ExcelService.InsertTextExistingExcel(excelPath, user.Address, "B", 2);
            ExcelService.InsertTextExistingExcel(excelPath, user.IdentityNumber, "C", 2);
            ExcelService.InsertTextExistingExcel(excelPath, user.Email, "D", 2);
            ExcelService.InsertTextExistingExcel(excelPath, user.PhoneNumber, "E", 2);

            var data = File.ReadAllBytes(excelPath);
            result.FileName = "export";
            result.Id = "export";
            result.Data = data;
            result.FileType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            return result;
        }
        public async Task<ApiResult<bool>> ImportEmployeeInfo(IFormFile file)
        {
            var fileName = "importTemp.xlsx";
            var filePath = Path.Combine(FILE_LOCATION, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                fileStream.Flush();
                fileStream.Close();
            }
            var user = new Employee()
            {
                Name = ExcelService.GetCellValue(filePath, "Sheet1", "A2"),
                Address = ExcelService.GetCellValue(filePath, "Sheet1", "B2"),
                IdentityNumber = ExcelService.GetCellValue(filePath, "Sheet1", "C2"),
                Email = ExcelService.GetCellValue(filePath, "Sheet1", "D2"),
                PhoneNumber = ExcelService.GetCellValue(filePath, "Sheet1", "E2"),
                DateCreated = DateTime.Now,
            };
            string password = "Abcd1234";
            string errorMessage = null;
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, "Employee") == false)
                {
                    var addtoroleResult = await _userManager.AddToRoleAsync(user, "Employee");
                    if (!addtoroleResult.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            errorMessage += error.Description + Environment.NewLine;
                        }
                        return new ApiErrorResult<bool>("Register failed: " + errorMessage);
                    }
                }
                return new ApiSuccessResult<bool>();
            }
            foreach (var error in result.Errors)
            {
                errorMessage += error.Description + Environment.NewLine;
            }
            return new ApiErrorResult<bool>("Register failed: " + errorMessage);
        }
    }
}