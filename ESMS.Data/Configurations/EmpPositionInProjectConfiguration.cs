using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class EmpPositionInProjectConfiguration : IEntityTypeConfiguration<EmpPositionInProject>
    {
        public void Configure(EntityTypeBuilder<EmpPositionInProject> builder)
        {
            builder.ToTable("EmpPositionInProjects");
            builder.HasKey(x => new { x.EmpID, x.RequiredPositionID });
            builder.Property(x => x.DateIn).HasColumnType("date");
            builder.Property(x => x.DateOut).HasColumnType("date");
            builder.HasOne(x => x.Employee).WithMany(x => x.EmpPosInProjects).HasForeignKey(x => x.EmpID);
            builder.HasOne(x => x.RequiredPosition).WithMany(x => x.EmpPositionInProjects).HasForeignKey(x => x.RequiredPositionID);
        }
    }
}