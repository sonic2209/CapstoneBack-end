using ESMS.Data.Configurations;
using ESMS.Data.Entities;
using ESMS.Data.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.EF
{
    public class ESMSDbContext : IdentityDbContext<Employee, Role, string>
    {
        public ESMSDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //configure fluent API
            modelBuilder.ApplyConfiguration(new AppConfigConfiguration());
            modelBuilder.ApplyConfiguration(new CertificationConfiguration());
            modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
            modelBuilder.ApplyConfiguration(new EmpSkillConfiguration());
            modelBuilder.ApplyConfiguration(new PositionConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new SkillConfiguration());
            modelBuilder.ApplyConfiguration(new EmpPositionInProjectConfiguration());
            modelBuilder.ApplyConfiguration(new EmpCertificationConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new RequiredPositionConfiguration());
            modelBuilder.ApplyConfiguration(new SkillInPositionConfiguration());
            modelBuilder.ApplyConfiguration(new RequiredSkillConfiguration());
            modelBuilder.ApplyConfiguration(new EmpPositionConfiguration());

            modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims");
            modelBuilder.Entity<IdentityUserRole<string>>().ToTable("AppUserRoles").HasKey(x => new { x.UserId, x.RoleId });
            modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("AppEmpLogins").HasKey(x => x.UserId);
            modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaims");
            modelBuilder.Entity<IdentityUserToken<string>>().ToTable("AppEmpTokens").HasKey(x => x.UserId);

            //Data seeding
            modelBuilder.Seed();
        }

        public DbSet<Certification> Certifications { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmpSkill> EmpSkills { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<EmpCertification> EmpCertifications { get; set; }
        public DbSet<EmpPositionInProject> EmpPositionInProjects { get; set; }
        public DbSet<RequiredPosition> RequiredPositions { get; set; }
        public DbSet<SkillInPosition> SkillInPositions { get; set; }
        public DbSet<RequiredSkill> RequiredSkills { get; set; }
        public DbSet<EmpPosition> EmpPositions { get; set; }
    }
}