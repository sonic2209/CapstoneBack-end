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
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).UseIdentityColumn();

            builder.HasOne(x => x.Project).WithMany(x => x.EmpPosInProjects).HasForeignKey(x => x.ProjectID);
            builder.HasOne(x => x.Employee).WithMany(x => x.EmpPosInProjects).HasForeignKey(x => x.EmpID);
            builder.HasOne(x => x.Position).WithMany(x => x.EmpPosInProjects).HasForeignKey(x => x.PosID);
        }
    }
}