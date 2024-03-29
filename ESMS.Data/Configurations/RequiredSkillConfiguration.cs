﻿using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESMS.Data.Configurations
{
    public class RequiredSkillConfiguration : IEntityTypeConfiguration<RequiredSkill>
    {
        public void Configure(EntityTypeBuilder<RequiredSkill> builder)
        {
            builder.ToTable("RequiredSkills");
            builder.HasKey(x => new { x.SkillID, x.RequiredPositionID });
            builder.HasOne(x => x.Skill).WithMany(x => x.RequiredSkills).HasForeignKey(x => x.SkillID);
            builder.HasOne(x => x.RequiredPosition).WithMany(x => x.RequiredSkills).HasForeignKey(x => x.RequiredPositionID);
        }
    }
}