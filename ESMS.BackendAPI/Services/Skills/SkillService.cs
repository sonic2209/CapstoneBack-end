using ESMS.Data.EF;
using ESMS.Data.Entities;
using ESMS.Data.Enums;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Skill;
using ESMS.BackendAPI.ViewModels.Certification;

using ESMS.BackendAPI.Ultilities;

namespace ESMS.BackendAPI.Services.Skills
{
    public class SkillService : ISkillService
    {
        private readonly ESMSDbContext _context;

        public SkillService(ESMSDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<bool>> ChangeStatus(int skillID)
        {
            var skill = _context.Skills.Find(skillID);
            if (skill == null) return new ApiErrorResult<bool>("Skill does not exist");
            if (skill.Status)
            {
                var empSkill = await _context.EmpSkills.Where(x => x.SkillID.Equals(skillID) && x.DateEnd == null)
                    .Select(x => x.EmpID).ToListAsync();
                if (empSkill.Count() != 0)
                {
                    return new ApiErrorResult<bool>("This skill is assigned to some employees");
                }
                var requiredSkill = await _context.RequiredSkills.Where(x => x.SkillID.Equals(skillID))
                    .Select(x => new RequiredSkill()).ToListAsync();
                if (requiredSkill.Count() != 0)
                {
                    return new ApiErrorResult<bool>("This skill is in project's requirement");
                }
                skill.Status = false;
            }
            else
            {
                skill.Status = true;
            }
            _context.Skills.Update(skill);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update skill failed");
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<bool>> Create(SkillCreateRequest request)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            var checkName = await _context.Skills.Where(x => x.SkillName.Equals(request.SkillName))
                .Select(x => new Skill()).FirstOrDefaultAsync();
            if (checkName != null)
            {
                UltilitiesService.AddOrUpdateError(errors, "SkillName", "This skill name already exist");
                //return new ApiErrorResult<bool>("This skill name already exist");
            }
            if (request.SkillType != (int)SkillType.HardSkill && request.SkillType != (int)SkillType.SoftSkill)
            {
                ultilities.AddOrUpdateError(errors, "SkillType", "Please select skill type(hard skill or soft skill)");
            }
            if (errors.Count() > 0)
            {
                return new ApiErrorResult<bool>(errors);
            }
            var skill = new Skill()
            {
                SkillName = request.SkillName,
                SkillType = (SkillType)request.SkillType
            };
            _context.Skills.Add(skill);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Create skill failed");
            }
            if (request.SkillType.Equals((int)SkillType.HardSkill))
            {
                if (request.HardSkillOption.Count() != 0)
                {
                    foreach (var type in request.HardSkillOption)
                    {
                        foreach (var pos in type.Position)
                        {
                            var minPos = new MinPosInProject()
                            {
                                TypeID = type.ProjectType,
                                PosID = pos,
                                SkillID = skill.SkillID
                            };
                            var checkSkill = await _context.MinPosInProjects.FindAsync(minPos.TypeID, minPos.PosID, minPos.SkillID);
                            if (checkSkill == null)
                            {
                                _context.MinPosInProjects.Add(minPos);
                                result = await _context.SaveChangesAsync();
                                if (result == 0)
                                {
                                    return new ApiErrorResult<bool>("Add project field failed");
                                }
                            }
                        }
                    }
                }
            }
            if (request.SkillType.Equals((int)SkillType.SoftSkill))
            {
                if (request.SoftSkillOption.Count() != 0)
                {
                    foreach (var field in request.SoftSkillOption)
                    {
                        var skillField = new SkillInProjectField()
                        {
                            FieldID = field,
                            SkillID = skill.SkillID
                        };
                        var checkSkill = await _context.SkillInProjectFields.FindAsync(skillField.FieldID, skillField.SkillID);
                        if (checkSkill == null)
                        {
                            _context.SkillInProjectFields.Add(skillField);
                            result = await _context.SaveChangesAsync();
                            if (result == 0)
                            {
                                return new ApiErrorResult<bool>("Add project field failed");
                            }
                        }
                    }
                }
            }
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<SkillDetailVM>> GetByID(int skillID)
        {
            var skillVM = new SkillDetailVM();
            var skill = await _context.Skills.FindAsync(skillID);
            if (skill == null) return new ApiErrorResult<SkillDetailVM>("Skill does not exist");
            skillVM.SkillID = skillID;
            skillVM.SkillName = skill.SkillName;
            skillVM.SkillType = (int)skill.SkillType;
            skillVM.HardSkillOption = new List<HardSkillOption>();
            skillVM.SoftSkillOption = new List<int>();
            if (skill.SkillType.Equals(SkillType.HardSkill))
            {
                skillVM.HardSkillOption = await _context.MinPosInProjects.Where(x => x.SkillID.Equals(skillID))
                    .Select(x => new HardSkillOption()
                    {
                        ProjectType = x.TypeID
                    }).Distinct().ToListAsync();
                foreach (var type in skillVM.HardSkillOption)
                {
                    type.Position = await _context.MinPosInProjects.Where(x => x.SkillID.Equals(skillID)
                    && x.TypeID.Equals(type.ProjectType)).Select(x => x.PosID).ToListAsync();
                }
            }
            if (skill.SkillType.Equals(SkillType.SoftSkill))
            {
                skillVM.SoftSkillOption = await _context.SkillInProjectFields.Where(x => x.SkillID.Equals(skillID))
                    .Select(x => x.FieldID).ToListAsync();
            }
            return new ApiSuccessResult<SkillDetailVM>(skillVM);
        }

        public async Task<ApiResult<List<HardSkillVM>>> GetHardSkills(int typeID, int posID)
        {
            var query = from mp in _context.MinPosInProjects
                        join s in _context.Skills on mp.SkillID equals s.SkillID
                        select new { mp, s };
            var skills = await query.Where(x => x.mp.TypeID.Equals(typeID) && x.mp.PosID.Equals(posID))
                .Select(x => new HardSkillVM()
                {
                    SkillID = x.mp.SkillID,
                    SkillName = x.s.SkillName
                }).ToListAsync();
            if (skills.Count() != 0)
            {
                foreach (var s in skills)
                {
                    s.Certifications = await _context.Certifications.Where(x => x.SkillID.Equals(s.SkillID))
                        .OrderBy(x => x.CertiLevel).Select(x => new ListCertificationViewModel()
                        {
                            CertificationID = x.CertificationID,
                            CertificationName = x.CertificationName,
                            CertiLevel = x.CertiLevel
                        }).ToListAsync();
                }
            }
            return new ApiSuccessResult<List<HardSkillVM>>(skills);
        }

        public async Task<ApiResult<PagedResult<SkillViewModel>>> GetSkillPaging(GetSkillPagingRequest request)
        {
            var query = from s in _context.Skills
                        select new { s };
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(x => x.s.SkillName.Contains(request.Keyword) && x.s.SkillType == request.SkillType);
            }
            else
            {
                query = query.Where(x => x.s.SkillType == request.SkillType);
            }
            int totalRow = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new SkillViewModel()
                {
                    SkillID = x.s.SkillID,
                    SkillName = x.s.SkillName,
                    SkillType = x.s.SkillType,
                    Status = x.s.Status
                }).ToListAsync();

            var pagedResult = new PagedResult<SkillViewModel>()
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return new ApiSuccessResult<PagedResult<SkillViewModel>>(pagedResult);
        }

        public async Task<ApiResult<List<ListSkillViewModel>>> GetSkills(int skillType)
        {
            var skills = await _context.Skills.Where(x => x.SkillType.Equals((SkillType)skillType)).Select(x => new ListSkillViewModel()
            {
                SkillID = x.SkillID,
                SkillName = x.SkillName
            }).ToListAsync();
            if (skills.Count() == 0)
            {
                return new ApiErrorResult<List<ListSkillViewModel>>("List skill not found");
            }
            return new ApiSuccessResult<List<ListSkillViewModel>>(skills);
        }

        public async Task<ApiResult<List<int>>> GetSoftSkills(int fieldID)
        {
            var query = from sp in _context.SkillInProjectFields
                        join s in _context.Skills on sp.SkillID equals s.SkillID
                        select new { sp, s };
            var skills = await query.Where(x => x.sp.FieldID.Equals(fieldID))
                .Select(x => x.sp.SkillID).ToListAsync();
            if (skills.Count() == 0)
            {
                return new ApiErrorResult<List<int>>("List soft skill not found");
            }
            return new ApiSuccessResult<List<int>>(skills);
        }

        public async Task<ApiResult<bool>> Update(int skillID, SkillUpdateRequest request)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            var skill = _context.Skills.Find(skillID);
            if (skill == null) return new ApiErrorResult<bool>("Skill does not exist");
            if (!skill.SkillName.Equals(request.SkillName))
            {
                var checkName = await _context.Skills.Where(x => x.SkillName.Equals(request.SkillName))
                .Select(x => new Skill()).FirstOrDefaultAsync();
                if (checkName != null)
                {
                    UltilitiesService.AddOrUpdateError(errors, "SkillName", "This skill name already exist");
                    //return new ApiErrorResult<bool>("This skill name already exist");
                }
            }
            if (request.SkillType != (int)SkillType.HardSkill && request.SkillType != (int)SkillType.SoftSkill)
            {
                ultilities.AddOrUpdateError(errors, "SkillType", "Please select skill type(hard skill or soft skill)");
            }
            else if (!skill.SkillType.Equals((SkillType)request.SkillType))
            {
                if (skill.SkillType.Equals(SkillType.HardSkill))
                {
                    var checkSkill = await _context.MinPosInProjects.Where(x => x.SkillID.Equals(skill.SkillID))
                         .Select(x => new MinPosInProject()).ToListAsync();
                    if (checkSkill.Count() != 0)
                    {
                        return new ApiErrorResult<bool>("Cannot change skill type");
                    }
                }
                if (skill.SkillType.Equals(SkillType.SoftSkill))
                {
                    var checkSkill = await _context.SkillInProjectFields.Where(x => x.SkillID.Equals(skill.SkillID))
                         .Select(x => new SkillInProjectField()).ToListAsync();
                    if (checkSkill.Count() != 0)
                    {
                        return new ApiErrorResult<bool>("Cannot change skill type");
                    }
                }
            }
            if (errors.Count() > 0)
            {
                return new ApiErrorResult<bool>(errors);
            }
            skill.SkillName = request.SkillName;
            skill.SkillType = (SkillType)request.SkillType;
            _context.Skills.Update(skill);
            var result = await _context.SaveChangesAsync();
            if (result == 0)
            {
                return new ApiErrorResult<bool>("Update skill failed");
            }
            if (request.SkillType.Equals((int)SkillType.HardSkill))
            {
                var MinPos = await _context.MinPosInProjects.Where(x => x.SkillID.Equals(skillID))
                    .Select(x => new MinPosInProject()
                    {
                        SkillID = x.SkillID,
                        TypeID = x.TypeID,
                        PosID = x.PosID
                    }).ToListAsync();
                if (request.HardSkillOption.Count() != 0)
                {
                    if (MinPos.Count() != 0)
                    {
                        foreach (var mp in MinPos)
                        {
                            var check = false;
                            foreach (var type in request.HardSkillOption)
                            {
                                foreach (var pos in type.Position)
                                {
                                    if (mp.TypeID.Equals(type.ProjectType) && mp.PosID.Equals(pos))
                                    {
                                        check = true;
                                    }
                                }
                            }
                            if (check == false)
                            {
                                _context.MinPosInProjects.Remove(mp);
                                result = await _context.SaveChangesAsync();
                                if (result == 0)
                                {
                                    return new ApiErrorResult<bool>("Remove skill failed");
                                }
                            }
                        }
                    }
                    foreach (var type in request.HardSkillOption)
                    {
                        foreach (var pos in type.Position)
                        {
                            var minPos = new MinPosInProject()
                            {
                                TypeID = type.ProjectType,
                                PosID = pos,
                                SkillID = skill.SkillID
                            };
                            var checkSkill = await _context.MinPosInProjects.FindAsync(minPos.TypeID, minPos.PosID, minPos.SkillID);
                            if (checkSkill == null)
                            {
                                _context.MinPosInProjects.Add(minPos);
                                result = await _context.SaveChangesAsync();
                                if (result == 0)
                                {
                                    return new ApiErrorResult<bool>("Add project field failed");
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (MinPos.Count() != 0)
                    {
                        foreach (var mp in MinPos)
                        {
                            _context.MinPosInProjects.Remove(mp);
                        }
                        result = await _context.SaveChangesAsync();
                        if (result == 0)
                        {
                            return new ApiErrorResult<bool>("Remove skill failed");
                        }
                    }
                }
            }
            if (request.SkillType.Equals((int)SkillType.SoftSkill))
            {
                var skillInFields = await _context.SkillInProjectFields.Where(x => x.SkillID.Equals(skillID))
                    .Select(x => new SkillInProjectField()
                    {
                        SkillID = x.SkillID,
                        FieldID = x.FieldID
                    }).ToListAsync();
                if (request.SoftSkillOption.Count() != 0)
                {
                    if (skillInFields.Count() != 0)
                    {
                        foreach (var sf in skillInFields)
                        {
                            var check = false;
                            foreach (var field in request.SoftSkillOption)
                            {
                                if (sf.FieldID.Equals(field))
                                {
                                    check = true;
                                }
                            }
                            if (check == false)
                            {
                                _context.SkillInProjectFields.Remove(sf);
                                result = await _context.SaveChangesAsync();
                                if (result == 0)
                                {
                                    return new ApiErrorResult<bool>("Remove skill failed");
                                }
                            }
                        }
                    }
                    foreach (var field in request.SoftSkillOption)
                    {
                        var skillField = new SkillInProjectField()
                        {
                            FieldID = field,
                            SkillID = skill.SkillID
                        };
                        var checkSkill = await _context.SkillInProjectFields.FindAsync(skillField.FieldID, skillField.SkillID);
                        if (checkSkill == null)
                        {
                            _context.SkillInProjectFields.Add(skillField);
                            result = await _context.SaveChangesAsync();
                            if (result == 0)
                            {
                                return new ApiErrorResult<bool>("Add project field failed");
                            }
                        }
                    }
                }
                else
                {
                    if (skillInFields.Count() != 0)
                    {
                        foreach (var sf in skillInFields)
                        {
                            _context.SkillInProjectFields.Remove(sf);
                        }
                        result = await _context.SaveChangesAsync();
                        if (result == 0)
                        {
                            return new ApiErrorResult<bool>("Remove skill failed");
                        }
                    }
                }
            }
            return new ApiSuccessResult<bool>();
        }
    }
}