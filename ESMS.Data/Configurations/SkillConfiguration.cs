using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.ToTable("Skills");
            builder.HasKey(x => x.SkillID);
            builder.Property(x => x.SkillID).UseIdentityColumn();
            builder.Property(x => x.SkillName).HasMaxLength(200).IsRequired();
        }
    }
}