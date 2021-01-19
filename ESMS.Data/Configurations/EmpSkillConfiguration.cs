using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class EmpSkillConfiguration : IEntityTypeConfiguration<EmpSkill>
    {
        public void Configure(EntityTypeBuilder<EmpSkill> builder)
        {
            builder.ToTable("EmpSkills");
            builder.HasKey(x => x.EmpSkillID);
            builder.Property(x => x.EmpSkillID).UseIdentityColumn();

            builder.HasOne(x => x.Employee).WithMany(x => x.EmpSkills).HasForeignKey(x => x.EmpID);
            builder.HasOne(x => x.Skill).WithMany(x => x.EmpSkills).HasForeignKey(x => x.SkillID);
        }
    }
}