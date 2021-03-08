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
            if (user ==null)
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
            var dicCandidate = new Dictionary<string, int>();
            foreach (RequiredPositionDetail requiredPosition in request.RequiredPositions)
            {
                if (requiredPosition.SoftSkillIDs != null)
                {                  

                    //list <Candidate> result = null   //candidate{empID, candidateMatch = 60}

                    //var match = 0

                    //function kiem Candidate trong result theo empID (findCandidate(empID))

                    //Get List Emp Theo Position
                    var ListEmpInPos = await _context.EmpPositions.Where(x => x.PosID == requiredPosition.PosID).Select(x => new EmpInPos()
                    {
                        EmpId = x.EmpID,
                        DateIn = x.DateIn,                   
                        NameExp = x.NameExp,
                    }).ToListAsync();
                    if (ListEmpInPos != null) {                  
                    foreach (EmpInPos emp in ListEmpInPos)
                    {
                        //add match theo kinh nghiem
                        dicCandidate.Add(emp.EmpId, 60);
                        switch (emp.NameExp)
                        {
                            case NameExp.Intern:
                                dicCandidate[emp.EmpId] += (10 / 5) * 1;
                                break;
                            case NameExp.Fresher:
                                dicCandidate[emp.EmpId] += (10 / 5) * 2;
                                break;
                            case NameExp.Junior:
                                dicCandidate[emp.EmpId] += (10 / 5) * 3;
                                break;
                            case NameExp.Senior:
                                dicCandidate[emp.EmpId] += (10 / 5) * 4;
                                break;
                            case NameExp.Master:
                                dicCandidate[emp.EmpId] += (10 / 5) * 5;
                                break;
                        }
                            //add match theo ngon ngu
                        var query = from ep in _context.EmpPositions
                                    join el in _context.EmpLanguages on ep.EmpID equals el.EmpID
                                    select new { ep, el };

                        var ListEmpInLang = await query.Where(x => x.el.EmpID.Equals(emp.EmpId) && x.el.LangID == requiredPosition.LanguageID).Select(x => new EmpInLang()
                        {
                            EmpId = x.el.EmpID,
                            LangLevel = x.el.LangLevel,
                        }).ToListAsync();

                        if (ListEmpInLang != null)
                        {
                            foreach (EmpInLang empl in ListEmpInLang)
                            {
                                switch (empl.LangLevel)
                                {
                                    case 1:
                                        dicCandidate[emp.EmpId] += (10 / 5) * 1;
                                        break;
                                    case 2:
                                        dicCandidate[emp.EmpId] += (10 / 5) * 2;
                                        break;
                                    case 3:
                                        dicCandidate[emp.EmpId] += (10 / 5) * 3;
                                        break;
                                    case 4:
                                        dicCandidate[emp.EmpId] += (10 / 5) * 4;
                                        break;
                                    case 5:
                                        dicCandidate[emp.EmpId] += (10 / 5) * 5;
                                        break;
                                }
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
                                if (softSkill.Equals(softskillId)){
                                        dicCandidate[emp.EmpId] += 10 / (requiredPosition.SoftSkillIDs.Count);
                                }
                            }
                                
                                //var SoftSkillCount = new Dictionary<string, int>();

                                //SoftSkillCount = listEmpSoftSkill.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

                                //var softSkillPoint = 0;
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
                                if (emphs.SkillID.Equals(hardskill.HardSkillID)) { 
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
                                    dicCandidate[emp.EmpId] += ((HighestCerti.HighestCertiLevel - hardskill.CertificationLevel) * 5) + ((int)emphs.SkillLevel * 5) * hardskill.Priority / requiredPosition.HardSkills.Count;
                                 }
                                }
                            }
                        }
                        
                    }
                }
                //- Get Emp in Pos by emp id -
                //var ListEmpInPos = select empID, DateIn, DateOut, NameExp from EmpPositions where PosID = required.PosID and DateOut == null
                //foreach (Emp emp in ListEmpInPos) 
                //////switch (emp.NameExp):
                ////////case intern : match += (10/5)*1; result.add(Candidate(empid, match))
                ////////case fresher : match += (10/5)*2
                ////////case junior : match += (10/5)*3
                ////////case senior : match += (10/5)*4
                ////////case master : match += (10/5)*5

                //- Get Emp in Lang by emp id -
                //var ListEmpLang = select empID, langLevel from EmpLan where LangId = require.langid and empid = ListEmpInPos.empID
                //foreach (Emp emp in ListEmpLang) 
                //////switch (langLevel):
                ////////case 1 : match += (10/5)*1 ; findCandidate(emp.empID).candidateMatch + match 
                ////////case 2 : match += (10/5)*2
                ////////case 3 : match += (10/5)*3
                ////////case 4 : match += (10/5)*4
                ////////case 5 : match += (10/5)*5


                //--tinh diem soft skill
                //var listEmpSoftSkill
                //foreach (int softskillID in request.SoftSkillIDs) 
                ////listEmpSoftSkill = select empID from EmpSkill where empID=ListEmpInPos.empID and skillid = softskillID join bang skill vs skill type  = softskill
                /////**{1,1,1,1,2,2,2,2,4,4,4,...}
                ////function count so phan tu lap trong list return  Map<key = Set empSkill, value = so phan tu giong nhau> map
                ///var softSkillPoint = 0
                //foreach (var value in listEmpSoftSkill) 
                ////softSkillPoint = (map.value) * 10  / (request.SoftSkillIDs.length); findCandidate(emp.empID).candidateMatch + match 

                ////Get highest certi level in hard skill of emp          
                ///get requiredCertiLevel
                //////Map<empId,List<obj(highestCertLevel,hardSkillLevel)>> empHardSkillCerti
                ///foreach(var list in empHardSkillCerti.key){
                /////match1 =0;
                /////foreach(var obj in list)
                ////////match1 += ((obj.highestCertLevel - requiredCertiLevel) * 5) + (hardSkillLevel * 5)) * requirePriority / request.HardSkills.length
                /////findCandidate(empHardSkillCerti.key).candidateMatch + match1
                ///

            }
            foreach (var item in dicCandidate)
            {
                result.Add(new CandidateViewModel { EmpID = item.Key, Match = item.Value });
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
