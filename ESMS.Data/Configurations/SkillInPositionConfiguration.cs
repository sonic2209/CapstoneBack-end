using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class SkillInPositionConfiguration : IEntityTypeConfiguration<SkillInPosition>
    {
        public void Configure(EntityTypeBuilder<SkillInPosition> builder)
        {
            builder.ToTable("SkillInPositions");
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).UseIdentityColumn();
            builder.HasOne(x => x.Skill).WithMany(x => x.SkillInPositions).HasForeignKey(x => x.SkillID);
            builder.HasOne(x => x.Position).WithMany(x => x.SkillInPositions).HasForeignKey(x => x.PositionID);
        }
    }
}