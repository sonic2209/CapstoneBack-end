﻿using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class EmpCertificationConfiguration : IEntityTypeConfiguration<EmpCertification>
    {
        public void Configure(EntityTypeBuilder<EmpCertification> builder)
        {
            builder.ToTable("EmpCertifications");
            builder.HasKey(x => new { x.EmpID, x.CertificationID });
            builder.Property(x => x.DateTaken).HasColumnType("date");
            builder.Property(x => x.DateEnd).HasColumnType("date");
            builder.HasOne(x => x.Employee).WithMany(x => x.EmpCertifications).HasForeignKey(x => x.EmpID);
            builder.HasOne(x => x.Certification).WithMany(x => x.EmpCertifications).HasForeignKey(x => x.CertificationID);
        }
    }
}