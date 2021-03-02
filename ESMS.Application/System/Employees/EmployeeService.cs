using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Data.Enums;
using ESMS.ViewModels.Common;
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

namespace ESMS.Application.System.Employees
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

        public async Task<ApiResult<string>> Authenticate(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return new ApiErrorResult<string>("Account does not exist");

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
            if (!result.Succeeded)
            {
                return new ApiErrorResult<string>("Email or password is not correct");
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

            return new ApiSuccessResult<string>(new JwtSecurityTokenHandler().WriteToken(token));
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
            List<CandidateViewModel> candidates = new List<CandidateViewModel>();
            var query = _userManager.Users;
            var data = await query.Select(x => new EmpVm()
            {
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                Name = x.Name,
                Id = x.Id,
                UserName = x.UserName
            }).ToListAsync();
            foreach (var item in data)
            {
                int match = 0;
                var empPosition = from ep in _context.EmpPositions
                                  select new { ep };
                empPosition = empPosition.Where(x => x.ep.EmpID.Equals(item.Id));
                var positions = await empPosition.Select(x => new EmpPosition()
                {
                    PosID = x.ep.PosID,
                    DateIn = x.ep.DateIn,
                    DateOut = x.ep.DateOut,
                    NameExp = x.ep.NameExp
                }).ToListAsync();
                var empCerti = from ec in _context.EmpCertifications
                               select new { ec };
                empCerti = empCerti.Where(x => x.ec.EmpID.Equals(item.Id));
                var certifications = await empCerti.Select(x => new EmpCertification()
                {
                    CertificationID = x.ec.CertificationID,
                    DateTaken = x.ec.DateTaken,
                    DateEnd = x.ec.DateEnd
                }).ToListAsync();
                var empSkill = from es in _context.EmpSkills
                               select new { es };
                empSkill = empSkill.Where(x => x.es.EmpID.Equals(item.Id));
                var skills = await empSkill.Select(x => new EmpSkill()
                {
                    SkillID = x.es.SkillID
                }).ToListAsync();
                foreach (var requiredPosition in request.RequiredPositions)
                {
                    foreach (var position in positions)
                    {
                        if (position.ID.Equals(requiredPosition.PosID))
                        {
                            switch (position.NameExp)
                            {
                                case NameExp.Fresher:
                                    match = match + 5;
                                    break;

                                case NameExp.Intern:
                                    match = match + 10;
                                    break;

                                case NameExp.Junior:
                                    match = match + 15;
                                    break;

                                case NameExp.Senior:
                                    match = match + 20;
                                    break;

                                case NameExp.Master:
                                    match = match + 25;
                                    break;
                            }
                            foreach (var skill in skills)
                            {
                                var s = await _context.Skills.FindAsync(skill.SkillID);
                                if (s.SkillType == SkillType.SoftSkill)
                                {
                                    foreach (var requiredSoftSkill in requiredPosition.SoftSkillIDs)
                                    {
                                        if (skill.SkillID.Equals(requiredSoftSkill))
                                        {
                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var requiredHardSKill in requiredPosition.HardSkills)
                                    {
                                        if (skill.SkillID.Equals(requiredHardSKill.HardSkillID))
                                        {
                                            switch (skill.SkillLevel)
                                            {
                                                case SkillLevel.BasicKnowledge:
                                                    match = match + 5;
                                                    break;

                                                case SkillLevel.LimitedExperience:
                                                    match = match + 10;
                                                    break;

                                                case SkillLevel.Practical:
                                                    match = match + 15;
                                                    break;

                                                case SkillLevel.AppliedTheory:
                                                    match = match + 20;
                                                    break;

                                                case SkillLevel.RecognizedAuthority:
                                                    match = match + 25;
                                                    break;
                                            }
                                        }
                                        foreach (var certi in certifications)
                                        {
                                            if (certi.ID.Equals(requiredHardSKill.CertificationID))
                                            {
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (match >= 50)
                {
                    var candidate = new CandidateViewModel()
                    {
                        Emp = item,
                        Match = match
                    };
                    candidates.Add(candidate);
                }
            }
            return candidates;
        }

        public async Task<ApiResult<bool>> Update(string id, EmpUpdateRequest request)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email.Equals(request.Email) && x.Id != id))
            {
                return new ApiErrorResult<bool>("Email existed");
            }
            var user = await _userManager.FindByIdAsync(id.ToString());
            user.DateCreated = request.DateCreated;
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