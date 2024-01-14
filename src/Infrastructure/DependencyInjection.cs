
using Application.Interfaces.Token;
using Infrastructure.DataContexts;
using Infrastructure.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //string connectionString = configuration.GetConnectionString(InfrastructureConstants.DB_CONTEXT_NAME)!;
            string connectionString = "Server=sql641.main-hosting.eu;Database=u854386805_patientmanagmt;Port=3306;Uid=u854386805_pmanagmtuser;Pwd=GoInnoetech2022!";
            var serverVersion = ServerVersion.AutoDetect(connectionString);
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(
                    connectionString, serverVersion,
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            });

            services.AddCors(options =>
                       options.AddPolicy("GeneralPolicy",
                                policy =>
                                {
                                    policy.WithOrigins("https://localhost:3000", "https://134traduction.com", "http://localhost:3000")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                                }));
          

            services.AddMvc().AddSessionStateTempDataProvider();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddHttpClient();

            services.AddScoped<ITokenServices, TokenServices>();

            //services.AddIdentity<UserEntity, UserRoleEntity>(options =>
            //{
            //    options.Password.RequireDigit = true;
            //    options.Password.RequiredLength = 10;
            //    options.Password.RequireNonAlphanumeric = true;
            //    options.Password.RequireUppercase = true;
            //    options.Password.RequireLowercase = true;
            //    options.Password.RequiredUniqueChars = 2;
            //})
            //    .AddEntityFrameworkStores<ApplicationDbContext>()
            //    .AddDefaultTokenProviders();



            //var jwtSettings = new JwtSettings();
            //configuration.Bind(nameof(JwtSettings), jwtSettings);
            //services.AddSingleton(jwtSettings);

            services.AddAuthentication(
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(

                    options =>
                    {
                        options.SaveToken = true;
                        options.RequireHttpsMetadata = false; //TODO: Change to true in Production
                        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            NameClaimType = "name",
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = "http://localhost",
                            ValidAudience = "http://localhost",
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("fdsfksdfksdgkfsdfsfgssgskgsglw005694sfmsg0750wesd040460232")),
                            ClockSkew = TimeSpan.Zero

                        };
                    }
                );

            services.AddSwaggerGen(setup =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
                setup.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {jwtSecurityScheme, Array.Empty<string>() }
                });
            });


            return services;
        }
    }
}
