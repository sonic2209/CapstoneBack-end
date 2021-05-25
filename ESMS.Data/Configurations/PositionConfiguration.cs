using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.ToTable("Positions");
            builder.HasKey(x => x.PosID);
            builder.Property(x => x.PosID).UseIdentityColumn();
            builder.HasIndex(x => x.Name).IsUnique();
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.Status).HasDefaultValue(true);
        }
    }
}