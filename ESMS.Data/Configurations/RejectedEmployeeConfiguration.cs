using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class RejectedEmployeeConfiguration : IEntityTypeConfiguration<RejectedEmployee>
    {
        public void Configure(EntityTypeBuilder<RejectedEmployee> builder)
        {
            builder.ToTable("RejectedEmployees");
            builder.HasKey(x => new { x.EmpID, x.RequiredPositionID });
            builder.HasOne(x => x.Employee).WithMany(x => x.RejectedEmployees).HasForeignKey(x => x.EmpID);
            builder.HasOne(x => x.RequiredPosition).WithMany(x => x.RejectedEmployees).HasForeignKey(x => x.RequiredPositionID);
        }
    }
}