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
            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Address).IsRequired();
            builder.Property(x => x.IdentityNumber).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Status).HasDefaultValue(EmployeeStatus.OnGoing);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}