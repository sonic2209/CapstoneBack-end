using ESMS.BackendAPI.Constants;
using ESMS.BackendAPI.Services.Certifications;
using ESMS.BackendAPI.Services.Emails;
using ESMS.BackendAPI.Services.Employees;
using ESMS.BackendAPI.Services.Languages;
using ESMS.BackendAPI.Services.Notifications;
using ESMS.BackendAPI.Services.Positions;
using ESMS.BackendAPI.Services.Projects;
using ESMS.BackendAPI.Services.Skills;
using ESMS.BackendAPI.ViewModels.Common;
using ESMS.BackendAPI.ViewModels.Employees;
using ESMS.BackendAPI.ViewModels.Notifications;
using ESMS.Data.EF;
using ESMS.Data.Entities;
using FirebaseAdmin;
using FluentValidation.AspNetCore;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ESMS.BackendAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //password configure
            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            });

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddDbContext<ESMSDbContext>(option =>
                option.UseSqlServer(Configuration.GetConnectionString(SystemConstants.MainConnectionString)));

            //Use Identity
            services.AddIdentity<Employee, Role>()
                .AddEntityFrameworkStores<ESMSDbContext>()
                .AddDefaultTokenProviders();

            //Declare DI
            services.AddTransient<IProjectService, ProjectService>();
            services.AddTransient<IPositionService, PositionService>();
            services.AddTransient<IEmployeeService, EmployeeService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ICertificationService, CertificationService>();
            services.AddTransient<ISkillService, SkillService>();
            services.AddTransient<ILanguageService, LanguageService>();
            services.AddTransient<UserManager<Employee>, UserManager<Employee>>();
            services.AddTransient<SignInManager<Employee>, SignInManager<Employee>>();
            services.AddTransient<RoleManager<Role>, RoleManager<Role>>();
            services.AddTransient<INotificationService, NotificationService>();

            services.AddControllersWithViews();

            //Fluent Validator
            services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<EmpCreateRequestValidator>());

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swagger Capstone Project", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                  {
                    {
                      new OpenApiSecurityScheme
                      {
                        Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                          },
                          Scheme = "oauth2",
                          Name = "Bearer",
                          In = ParameterLocation.Header,
                        },
                        new List<string>()
                      }
                    });
            });

            string issuer = Configuration.GetValue<string>("Tokens:Issuer");
            string signingKey = Configuration.GetValue<string>("Tokens:Key");
            byte[] signingKeyBytes = System.Text.Encoding.UTF8.GetBytes(signingKey);

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidAudience = issuer,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = System.TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes)
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //var userService = app.ApplicationServices.GetService<IEmployeeService>();
            var scope = app.ApplicationServices.CreateScope();
            var notiService = scope.ServiceProvider.GetService<INotificationService>();
            var employeeService = scope.ServiceProvider.GetService<IEmployeeService>();
            var projectService = scope.ServiceProvider.GetService<IProjectService>();
            var emailService = scope.ServiceProvider.GetService<IEmailService>();
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "capstone-esms-firebase-adminsdk-3z2td-a35a510cb1.json")),
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/User/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseRouting();

            app.UseCors("MyPolicy");

            app.UseAuthorization();

            app.UseSwagger(c =>
            {
                c.PreSerializeFilters.Add((swagger, httpReq) =>
                {
                    swagger.Servers = new List<OpenApiServer>();
                });
            });

            app.UseSwaggerUI(c =>
            {
                c.DocumentTitle = "ESMSBackEndApi";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger CapstoneProject V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            new Thread(async () =>
               {
                   while (true)
                   {
                       
                       Thread.CurrentThread.IsBackground = true;
                       /* run your code here */
                       //   Console.WriteLine("Hello, world");
                       var check = await projectService.CheckProject();
                       //emailService.Send("dinhbinh599@gmail.com", "gigalesky@gmail.com", "123");

                       var listDeletedProject = await projectService.CheckNoEmpProject();
                       if (listDeletedProject.Count() > 0)
                        {
                           foreach (var deletedProject in listDeletedProject)
                           {
                               string topic = deletedProject.ProjectManagerID;
                               NotificationContent noti = new NotificationContent()
                               {
                                   title = "A project of yours has been deleted",
                                   body = "We've deleted project named \"" + deletedProject.ProjectName + "\" due to the lack of employees in it",
                                   topic = deletedProject.ProjectManagerID,
                                   dateCreate = DateTime.Now
                               };
                               notiService.SendMessage(noti);
                           }
                       }
                       //await employeeService.RemoveExpiredCertificate();
                       //var result = userService.GetById("064535f6-61c5-4968-93a3-fc22172640a3");
                       /* */
                       Thread.Sleep(TimeSpan.FromMinutes(30));
                   }
               }).Start();
        }
    }
}