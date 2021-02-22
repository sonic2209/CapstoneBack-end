using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class EmpPositionConfiguration : IEntityTypeConfiguration<EmpPosition>
    {
        public void Configure(EntityTypeBuilder<EmpPosition> builder)
        {
            builder.ToTable("EmpPositions");
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).UseIdentityColumn();
            builder.Property(x => x.DateIn).HasColumnType("date");
            builder.Property(x => x.DateOut).HasColumnType("date");
            builder.HasOne(x => x.Employee).WithMany(x => x.EmpPositions).HasForeignKey(x => x.EmpID);
            builder.HasOne(x => x.Position).WithMany(x => x.EmpPositions).HasForeignKey(x => x.PosID);
        }
    }
}