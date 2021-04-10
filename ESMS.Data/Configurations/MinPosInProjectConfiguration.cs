using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class MinPosInProjectConfiguration : IEntityTypeConfiguration<MinPosInProject>
    {
        public void Configure(EntityTypeBuilder<MinPosInProject> builder)
        {
            builder.ToTable("MinPosInProjects");
            builder.HasKey(x => new { x.TypeID, x.PosID });
            builder.HasOne(x => x.ProjectType).WithMany(x => x.MinPosInProjects).HasForeignKey(x => x.TypeID);
            builder.HasOne(x => x.Position).WithMany(x => x.MinPosInProjects).HasForeignKey(x => x.PosID);
        }
    }
}