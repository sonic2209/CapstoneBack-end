using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ESMS.Data.EF
{
    public class ESMSDbContextFactory : IDesignTimeDbContextFactory<ESMSDbContext>
    {
        public ESMSDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("CapstoneProject");
            var optionBuilder = new DbContextOptionsBuilder<ESMSDbContext>();
            optionBuilder.UseSqlServer(connectionString);

            return new ESMSDbContext(optionBuilder.Options);
        }
    }
}