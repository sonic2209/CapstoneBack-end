﻿// <auto-generated />
using System;
using ESMS.Data.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ESMS.Data.Migrations
{
    [DbContext(typeof(ESMSDbContext))]
    partial class ESMSDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("ESMS.Data.Entities.AppConfig", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Key");

                    b.ToTable("AppConfigs");

                    b.HasData(
                        new
                        {
                            Key = "HomeTitle",
                            Value = "This is home page of ESMS system"
                        },
                        new
                        {
                            Key = "HomeKeyword",
                            Value = "This is keyword of ESMS system"
                        },
                        new
                        {
                            Key = "HomeDescription",
                            Value = "This is description of ESMS system"
                        });
                });

            modelBuilder.Entity("ESMS.Data.Entities.Certification", b =>
                {
                    b.Property<int>("CertificationID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("CertificationName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CertificationID");

                    b.ToTable("Certifications");
                });

            modelBuilder.Entity("ESMS.Data.Entities.EmpCertification", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("CertificationID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateTaken")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmpID")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("ID");

                    b.HasIndex("CertificationID");

                    b.HasIndex("EmpID");

                    b.ToTable("EmpCertifications");
                });

            modelBuilder.Entity("ESMS.Data.Entities.EmpPositionInProject", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("EmpID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("PosID")
                        .HasColumnType("int");

                    b.Property<int>("ProjectID")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.HasIndex("EmpID");

                    b.HasIndex("PosID");

                    b.HasIndex("ProjectID");

                    b.ToTable("EmpPositionInProjects");
                });

            modelBuilder.Entity("ESMS.Data.Entities.EmpSkill", b =>
                {
                    b.Property<int>("EmpSkillID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("EmpID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("SkillID")
                        .HasColumnType("int");

                    b.HasKey("EmpSkillID");

                    b.HasIndex("EmpID");

                    b.HasIndex("SkillID");

                    b.ToTable("EmpSkills");
                });

            modelBuilder.Entity("ESMS.Data.Entities.Employee", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateEnd")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("IdentityNumber")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Employees");

                    b.HasData(
                        new
                        {
                            Id = "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                            AccessFailedCount = 0,
                            Address = "580 Quang Trung P10",
                            ConcurrencyStamp = "6d0278b1-e6d1-4f14-86d5-93094f544493",
                            DateCreated = new DateTime(2021, 1, 27, 12, 14, 19, 649, DateTimeKind.Local).AddTicks(7703),
                            Email = "resker123@gmail.com",
                            EmailConfirmed = true,
                            IdentityNumber = "0123456789",
                            LockoutEnabled = false,
                            Name = "Pham Tuan",
                            NormalizedEmail = "resker123@gmail.com",
                            NormalizedUserName = "admin",
                            PasswordHash = "AQAAAAEAACcQAAAAEELGL/ir9ABuXHNWEZymwZZcVSLqzq+AC711aG5tdOGuAR7scjDAMxfo966uJlvmbw==",
                            PhoneNumberConfirmed = false,
                            SecurityStamp = "",
                            Status = 0,
                            TwoFactorEnabled = false,
                            UserName = "admin"
                        });
                });

            modelBuilder.Entity("ESMS.Data.Entities.Position", b =>
                {
                    b.Property<int>("PosID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("PosID");

                    b.ToTable("Positions");
                });

            modelBuilder.Entity("ESMS.Data.Entities.Project", b =>
                {
                    b.Property<int>("ProjectID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateEnd")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProjectManagerID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Skateholder")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.HasKey("ProjectID");

                    b.HasIndex("ProjectManagerID");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("ESMS.Data.Entities.Role", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = "8D04DCE2-969A-435D-BBA4-DF3F325983DC",
                            ConcurrencyStamp = "9a5e54f7-0858-4434-9d55-86703b382450",
                            Description = "Administrator role",
                            Name = "admin",
                            NormalizedName = "admin"
                        });
                });

            modelBuilder.Entity("ESMS.Data.Entities.Skill", b =>
                {
                    b.Property<int>("SkillID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("SkillName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<int>("SkillType")
                        .HasColumnType("int");

                    b.HasKey("SkillID");

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("AppRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("AppUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("AppEmpLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.ToTable("AppUserRoles");

                    b.HasData(
                        new
                        {
                            UserId = "69BD714F-9576-45BA-B5B7-F00649BE00DE",
                            RoleId = "8D04DCE2-969A-435D-BBA4-DF3F325983DC"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("AppEmpTokens");
                });

            modelBuilder.Entity("ESMS.Data.Entities.EmpCertification", b =>
                {
                    b.HasOne("ESMS.Data.Entities.Certification", "Certification")
                        .WithMany("EmpCertifications")
                        .HasForeignKey("CertificationID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ESMS.Data.Entities.Employee", "Employee")
                        .WithMany("EmpCertifications")
                        .HasForeignKey("EmpID");

                    b.Navigation("Certification");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("ESMS.Data.Entities.EmpPositionInProject", b =>
                {
                    b.HasOne("ESMS.Data.Entities.Employee", "Employee")
                        .WithMany("EmpPosInProjects")
                        .HasForeignKey("EmpID");

                    b.HasOne("ESMS.Data.Entities.Position", "Position")
                        .WithMany("EmpPosInProjects")
                        .HasForeignKey("PosID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ESMS.Data.Entities.Project", "Project")
                        .WithMany("EmpPosInProjects")
                        .HasForeignKey("ProjectID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Position");

                    b.Navigation("Project");
                });

            modelBuilder.Entity("ESMS.Data.Entities.EmpSkill", b =>
                {
                    b.HasOne("ESMS.Data.Entities.Employee", "Employee")
                        .WithMany("EmpSkills")
                        .HasForeignKey("EmpID");

                    b.HasOne("ESMS.Data.Entities.Skill", "Skill")
                        .WithMany("EmpSkills")
                        .HasForeignKey("SkillID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Skill");
                });

            modelBuilder.Entity("ESMS.Data.Entities.Project", b =>
                {
                    b.HasOne("ESMS.Data.Entities.Employee", "Employee")
                        .WithMany("Projects")
                        .HasForeignKey("ProjectManagerID");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("ESMS.Data.Entities.Certification", b =>
                {
                    b.Navigation("EmpCertifications");
                });

            modelBuilder.Entity("ESMS.Data.Entities.Employee", b =>
                {
                    b.Navigation("EmpCertifications");

                    b.Navigation("EmpPosInProjects");

                    b.Navigation("EmpSkills");

                    b.Navigation("Projects");
                });

            modelBuilder.Entity("ESMS.Data.Entities.Position", b =>
                {
                    b.Navigation("EmpPosInProjects");
                });

            modelBuilder.Entity("ESMS.Data.Entities.Project", b =>
                {
                    b.Navigation("EmpPosInProjects");
                });

            modelBuilder.Entity("ESMS.Data.Entities.Skill", b =>
                {
                    b.Navigation("EmpSkills");
                });
#pragma warning restore 612, 618
        }
    }
}
