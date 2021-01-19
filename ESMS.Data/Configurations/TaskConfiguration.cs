using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.ToTable("Tasks");
            builder.HasKey(x => x.TaskID);
            builder.Property(x => x.TaskID).UseIdentityColumn();

            builder.HasOne(x => x.Team).WithMany(x => x.Tasks).HasForeignKey(x => x.TeamID);
        }
    }
}