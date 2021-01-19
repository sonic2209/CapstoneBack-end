using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.ToTable("Team");
            builder.HasKey(x => x.TeamID);
            builder.Property(x => x.TeamID).UseIdentityColumn();

            builder.HasOne(x => x.Project).WithMany(x => x.Teams).HasForeignKey(x => x.ProjectID);
            builder.HasOne(x => x.Employee).WithMany(x => x.Teams).HasForeignKey(x => x.EmpID);
            builder.HasOne(x => x.Position).WithMany(x => x.Teams).HasForeignKey(x => x.PosID);
        }
    }
}