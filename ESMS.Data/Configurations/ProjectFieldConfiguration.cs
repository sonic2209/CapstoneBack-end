using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class ProjectFieldConfiguration : IEntityTypeConfiguration<ProjectField>
    {
        public void Configure(EntityTypeBuilder<ProjectField> builder)
        {
            builder.ToTable("ProjectFields");
            builder.HasKey(x => x.ID);
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        }
    }
}