using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Employees;
using ESMS.BackendAPI.ViewModels.Employees.Suggestion;
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

        public EmployeeService(UserManager<Employee> userManager, SignInManager<Employee> signInManager,
            RoleManager<Role> roleManager, IConfiguration config, ESMSDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            _context = context;
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
                return new ApiErrorResult<LoginVm>("Email/Username or password is not correct");
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

        public async Task<ApiResult<bool>> Create(EmpCreateRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user != null)
            {
                return new ApiErrorResult<bool>("Account existed");
            }
            if (await _userManager.FindByEmailAsync(request.Email) != null)
            {
                return new ApiErrorResult<bool>("Email existed");
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
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Register failed");
        }

        public async Task<ApiResult<bool>> AddEmpPosition(string empID, AddEmpPositionRequest request)
        {
            var empPosition = new EmpPosition()
            {
                EmpID = empID,
                PosID = request.PosID,
                NameExp = (NameExp)request.NameExp,
                DateIn = DateTime.Now
            };
            _context.EmpPositions.Add(empPosition);
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
            foreach (var softSkill in request.SoftSkills)
            {
                var empSoftSkill = new EmpSkill()
                {
                    EmpID = empID,
                    SkillID = softSkill
                };
                _context.EmpSkills.Add(empSoftSkill);
            }
            foreach (var hardSkill in request.HardSkills)
            {
                var empHardSkill = new EmpSkill()
                {
                    EmpID = empID,
                    SkillID = hardSkill.SkillID,
                    SkillLevel = (SkillLevel)hardSkill.SkillLevel,
                    DateStart = DateTime.Now
                };
                _context.EmpSkills.Add(empHardSkill);
                foreach (var certification in hardSkill.EmpCertifications)
                {
                    var empCertification = new EmpCertification()
                    {
                        EmpID = empID,
                        CertificationID = certification.CertiID,
                        DateTaken = certification.DateTaken,
                        DateEnd = certification.DateEnd
                    };
                    _context.EmpCertifications.Add(empCertification);
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
            var empVm = new EmpVm()
            {
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Name = user.Name,
                DateCreated = (DateTime)user.DateCreated,
                Id = user.Id,
                Roles = roles
            };
            return new ApiSuccessResult<EmpVm>(empVm);
        }

        public async Task<ApiResult<PagedResult<EmpVm>>> GetEmpsPaging(GetEmpPagingRequest request)
        {
            {
                var query = _userManager.Users;
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    query = query.Where(x => x.UserName.Contains(request.Keyword) || x.PhoneNumber.Contains(request.Keyword)
                    || x.Email.Contains(request.Keyword));
                }
                //3.Paging
                int totalRow = await query.CountAsync();

                var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new EmpVm()
                    {
                        Email = x.Email,
                        PhoneNumber = x.PhoneNumber,
                        Name = x.Name,
                        Id = x.Id,
                        UserName = x.UserName
                    }).ToListAsync();

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

        public async Task<List<CandidateViewModel>> SuggestCandidate(int projectID, SuggestCadidateRequest request)
        {
            List<CandidateViewModel> result = new List<CandidateViewModel>();
            foreach (RequiredPositionDetail requiredPosition in request.RequiredPositions)
            {
                var dicCandidate = new Dictionary<string, double>();
                if (requiredPosition.SoftSkillIDs != null)
                {
                    //Get List Emp Theo Position
                    List<MatchViewModel> listMatchDetail = new List<MatchViewModel>();
                    var Position = _context.Positions.Where(x => x.PosID == requiredPosition.PosID).Select(x => x.Name).FirstOrDefault();
                    var PosId = _context.Positions.Where(x => x.PosID == requiredPosition.PosID).Select(x => x.PosID).FirstOrDefault();
                    var ListEmpPosquery = from ep in _context.EmpPositions
                                          join p in _context.Positions on ep.PosID equals p.PosID
                                          join e in _context.Employees on ep.EmpID equals e.Id
                                          select new { ep, p, e };

                    var ListEmpInPos = await ListEmpPosquery.Where(x => x.ep.PosID == requiredPosition.PosID && x.ep.DateOut == null).Select(x => new EmpInPos()
                    {
                        EmpId = x.ep.EmpID,
                        DateIn = x.ep.DateIn,
                        NameExp = x.ep.NameExp,
                        EmpName = x.e.Name,
                        Position = x.p.Name,
                    }).ToListAsync();
                    if (ListEmpInPos != null)
                    {
                        foreach (EmpInPos emp in ListEmpInPos)
                        {
                            double match = 60;
                            double Languagematch = 0;
                            double Softskillmatch = 0;
                            double Hardskillmatch = 0;
                            MatchViewModel matchDetail = new MatchViewModel();

                            //add match theo kinh nghiem
                            //dicCandidate.Add(emp.EmpId, 60);
                            switch (emp.NameExp)
                            {
                                case NameExp.Intern:
                                    match += (10 / 5) * 1;
                                    break;

                                case NameExp.Fresher:
                                    match += (10 / 5) * 2;
                                    break;

                                case NameExp.Junior:
                                    match += (10 / 5) * 3;
                                    break;

                                case NameExp.Senior:
                                    match += (10 / 5) * 4;
                                    break;

                                case NameExp.Master:
                                    match += (10 / 5) * 5;
                                    break;
                            }
                            //add match theo ngon ngu
                            var query = from ep in _context.EmpPositions
                                        join el in _context.EmpLanguages on ep.EmpID equals el.EmpID
                                        select new { ep, el };

                            foreach (LanguageDetail language in requiredPosition.Language)
                            {
                                var ListEmpInLang = await _context.EmpLanguages.Where(x => x.EmpID.Equals(emp.EmpId) && x.LangID == language.LangID).Select(x => new EmpInLang()
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

                                if (ListEmpInLang != null)
                                {
                                    foreach (EmpInLang empl in ListEmpInLang)
                                    {
                                        Languagematch += (empl.LangLevel * language.Priority * 0.1) / requiredPosition.Language.Count;
                                    }
                                    match += Math.Round(Languagematch, 2);
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
                                match += Math.Round(Softskillmatch, 2);
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
                                        if (HighestCerti.HighestCertiLevel >= hardskill.CertificationLevel)
                                        {
                                            Hardskillmatch = (((HighestCerti.HighestCertiLevel - hardskill.CertificationLevel) * 0.5) + ((int)emphs.SkillLevel * 2 * 0.5)) * hardskill.Priority / 10 * requiredPosition.HardSkills.Count;
                                            match += Math.Round(Hardskillmatch, 2);
                                        }
                                        else
                                        {
                                            Hardskillmatch = (((int)emphs.SkillLevel * 2 * 0.5)) * hardskill.Priority / 10 * requiredPosition.HardSkills.Count;
                                            match += Math.Round(Hardskillmatch, 2);
                                        }
                                    }
                                }
                            }
                            matchDetail = new MatchViewModel()
                            {
                                EmpID = emp.EmpId,
                                EmpName = emp.EmpName,
                                LanguageMatch = Languagematch,
                                SoftSkillMatch = Softskillmatch,
                                HardSkillMatch = Hardskillmatch,
                                OverallMatch = match,
                            };
                            listMatchDetail.Add(matchDetail);
                        }

                        result.Add(new CandidateViewModel()
                        {
                            Position = Position,
                            PosId = PosId,
                            MatchDetail = listMatchDetail,
                        });
                    }
                }
            }
            return result;
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
            if (result.Succeeded)
            {
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>("Update user failed");
        }
    }
}