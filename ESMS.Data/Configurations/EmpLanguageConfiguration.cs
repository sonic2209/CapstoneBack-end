using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class EmpLanguageConfiguration : IEntityTypeConfiguration<EmpLanguage>
    {
        public void Configure(EntityTypeBuilder<EmpLanguage> builder)
        {
            builder.ToTable("EmpLanguages");
            builder.HasKey(x => new { x.EmpID, x.LangID });
            builder.HasOne(x => x.Language).WithMany(x => x.EmpLanguages).HasForeignKey(x => x.LangID);
            builder.HasOne(x => x.Employee).WithMany(x => x.EmpLanguages).HasForeignKey(x => x.EmpID);
        }
    }
}