using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class CertificationConfiguration : IEntityTypeConfiguration<Certification>
    {
        public void Configure(EntityTypeBuilder<Certification> builder)
        {
            builder.ToTable("Certifications");
            builder.HasKey(x => x.CertificationID);
            builder.Property(x => x.CertificationID).UseIdentityColumn();
            builder.HasIndex(x => x.CertificationName).IsUnique();
            builder.Property(x => x.CertificationName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.Status).HasDefaultValue(true);
            builder.HasOne(x => x.Skill).WithMany(x => x.Certifications).HasForeignKey(x => x.SkillID);
        }
    }
}