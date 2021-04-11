using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class SkillInProjectFieldConfiguration : IEntityTypeConfiguration<SkillInProjectField>
    {
        public void Configure(EntityTypeBuilder<SkillInProjectField> builder)
        {
            builder.ToTable("SkillInProjectFields");
            builder.HasKey(x => new { x.FieldID, x.SkillID });
            builder.HasOne(x => x.ProjectField).WithMany(x => x.SkillInProjectFields).HasForeignKey(x => x.FieldID);
            builder.HasOne(x => x.Skill).WithMany(x => x.SkillInProjectFields).HasForeignKey(x => x.SkillID);
        }
    }
}