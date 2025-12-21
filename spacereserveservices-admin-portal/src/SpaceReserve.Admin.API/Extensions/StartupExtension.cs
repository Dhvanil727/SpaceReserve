using Microsoft.AspNetCore.Authentication.JwtBearer;
using SpaceReserve.Infrastructure.Extensions;
using FluentValidation.AspNetCore;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Mvc;
using SpaceReserve.Utility.Settings;
using Hangfire;

namespace SpaceReserve.Admin.API.Extensions
{
    public static class StartupExtension
    {
        [Obsolete]
        public static void AddStartupServices(this WebApplicationBuilder builder)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null)
            {
                throw new InvalidOperationException("Entry assembly could not be determined.");
            }
            var logRepository = LogManager.GetRepository(entryAssembly);
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            builder.Services.AddAutoMapper(typeof(AutoMapperConfigurations));
            builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.Authority = builder.Configuration["Keycloak:Authority"];
                o.Audience = builder.Configuration["Keycloak:Audience"];
                o.RequireHttpsMetadata = false;
            });
            builder.Services.AddAuthorization(options =>
            {
                // Define policies if needed
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            });

            builder.Services.AddCors(x => x.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                        }
                    });
            });
            builder.Services.AddHangfire(config =>
        {
            config.UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(builder.Configuration.GetConnectionString("DatabaseConnection"));
        });
            builder.Services.AddHangfireServer();

            builder.Services.AddControllers()
            .AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(Assembly.Load("SpaceReserve.Admin.AppService")));
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });


            builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddAppServices();
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddHttpClient();
        }
    }
}