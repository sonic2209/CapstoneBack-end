using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class RequiredPositionConfiguration : IEntityTypeConfiguration<RequiredPosition>
    {
        public void Configure(EntityTypeBuilder<RequiredPosition> builder)
        {
            builder.ToTable("RequiredPositions");
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).UseIdentityColumn();
            builder.HasOne(x => x.Project).WithMany(x => x.RequiredPositions).HasForeignKey(x => x.ProjectID);
            builder.HasOne(x => x.Position).WithMany(x => x.RequiredPositions).HasForeignKey(x => x.PositionID);
        }
    }
}