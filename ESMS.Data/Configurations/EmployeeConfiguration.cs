using ESMS.Data.Entities;
using ESMS.Data.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.UserName).IsUnique();
            builder.Property(x => x.Name).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(50).IsRequired();
            builder.Property(x => x.NormalizedEmail).HasMaxLength(50).IsRequired();
            builder.Property(x => x.PhoneNumber).HasMaxLength(10);
            builder.Property(x => x.UserName).HasMaxLength(20).IsRequired();
            builder.Property(x => x.NormalizedUserName).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Address).HasMaxLength(100).IsRequired();
            builder.Property(x => x.IdentityNumber).HasMaxLength(12).IsRequired();
            builder.Property(x => x.Status).HasDefaultValue(EmployeeStatus.OnGoing);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}