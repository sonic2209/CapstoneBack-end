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
            builder.HasIndex(x => x.ProjectName).IsUnique();
            builder.Property(x => x.ProjectName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.Status).HasDefaultValue(ProjectStatus.Pending);
            builder.Property(x => x.DateBegin).HasColumnType("date");
            builder.Property(x => x.DateEstimatedEnd).HasColumnType("date");
            builder.Property(x => x.EmailStatus).HasDefaultValue(false);
            builder.HasOne(x => x.Employee).WithMany(x => x.Projects).HasForeignKey(x => x.ProjectManagerID);
            builder.HasOne(x => x.ProjectType).WithMany(x => x.Projects).HasForeignKey(x => x.ProjectTypeID);
            builder.HasOne(x => x.ProjectField).WithMany(x => x.Projects).HasForeignKey(x => x.ProjectFieldID);
        }
    }
}