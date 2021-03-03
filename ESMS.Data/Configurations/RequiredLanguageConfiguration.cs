using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Configurations
{
    public class RequiredLanguageConfiguration : IEntityTypeConfiguration<RequiredLanguage>
    {
        public void Configure(EntityTypeBuilder<RequiredLanguage> builder)
        {
            builder.ToTable("RequiredLanguages");
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).UseIdentityColumn();
            builder.HasOne(x => x.RequiredPosition).WithMany(x => x.RequiredLanguages).HasForeignKey(x => x.RequiredPositionID);
            builder.HasOne(x => x.Language).WithMany(x => x.RequiredLanguages).HasForeignKey(x => x.LangID);
        }
    }
}