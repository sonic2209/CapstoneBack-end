using ESMS.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESMS.Data.Configurations
{
    public class SkillRequirementConfiguration : IEntityTypeConfiguration<SkillRequirement>
    {
        public void Configure(EntityTypeBuilder<SkillRequirement> builder)
        {
            builder.ToTable("RequiredSkills");
            builder.HasKey(x => x.ID);
            builder.Property(x => x.ID).UseIdentityColumn();
            builder.HasOne(x => x.Skill).WithMany(x => x.SkillRequirements).HasForeignKey(x => x.SkillID);
            builder.HasOne(x => x.PositionRequirement).WithMany(x => x.SkillRequirements).HasForeignKey(x => x.PositionRequirementID);
        }
    }
}