using ESMS.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ESMS.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppConfig>().HasData(
                           new AppConfig() { Key = "HomeTitle", Value = "This is home page of ESMS system" },
                           new AppConfig() { Key = "HomeKeyword", Value = "This is keyword of ESMS system" },
                           new AppConfig() { Key = "HomeDescription", Value = "This is description of ESMS system" }
                           );
            // any guid
            var roleId = new string("8D04DCE2-969A-435D-BBA4-DF3F325983DC");
            var adminId = new string("69BD714F-9576-45BA-B5B7-F00649BE00DE");
            modelBuilder.Entity<Role>().HasData(new Role
            {
                Id = roleId,
                Name = "admin",
                NormalizedName = "admin",
                Description = "Administrator role"
            });

            var hasher = new PasswordHasher<Employee>();
            modelBuilder.Entity<Employee>().HasData(new Employee
            {
                Id = adminId,
                UserName = "admin",
                NormalizedUserName = "admin",
                IdentityNumber = "0123456789",
                Address = "580 Quang Trung P10",
                Email = "resker123@gmail.com",
                NormalizedEmail = "resker123@gmail.com",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Abcd1234$"),
                SecurityStamp = string.Empty,
                Name = "Pham Tuan",
                DateCreated = DateTime.Now
            });

            modelBuilder.Entity<ProjectField>().HasData(
                new ProjectType { ID = 1, Name = "Community and social services" },
                new ProjectType { ID = 2, Name = "Business, management and administration" },
                new ProjectType { ID = 3, Name = "Education" },
                new ProjectType { ID = 4, Name = "Health and medicine" },
                new ProjectType { ID = 5, Name = "Sales" });

            modelBuilder.Entity<ProjectType>().HasData(
                new ProjectType { ID = 1, Name = "Web application" },
                new ProjectType { ID = 2, Name = "Mobile application" },
                new ProjectType { ID = 3, Name = "Desktop application" });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = roleId,
                UserId = adminId
            });
        }
    }
}