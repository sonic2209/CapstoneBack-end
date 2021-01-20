using ESMS.Data.Entities;
using ESMS.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("Projects");
            builder.HasKey(x => x.ProjectID);
            builder.Property(x => x.ProjectID).UseIdentityColumn();
            builder.Property(x => x.ProjectName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Skateholder).IsRequired();
            builder.Property(x => x.Status).HasDefaultValue(ProjectStatus.Pending);
            builder.HasOne(x => x.Employee).WithMany(x => x.Projects).HasForeignKey(x => x.ProjectManagerID);
        }
    }
}