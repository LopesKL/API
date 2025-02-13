using API.Domain.Users.Auth;
using API.Domain.Users.Auth.JWT;
using API.Infra.SqlServer.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AddSqlServerDbContext
    {
        public static IServiceCollection AddSqlServerContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApiServerContext>(options =>
                 options.UseLazyLoadingProxies(false) // Disabling lazy loading here
                       .EnableSensitiveDataLogging()
                       .UseSqlServer(configuration.GetConnectionString("SqlServer"),
                       opt => opt.MigrationsAssembly("WebAPI"))
            );

            services.AddHostedService<TimedHostedService>();

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true;
            })
                .AddEntityFrameworkStores<ApiServerContext>()
                .AddDefaultTokenProviders();

            var signingConfigurations = new SigningConfigurations(configuration.GetSection("Authentication"));
            services.AddSingleton(signingConfigurations);

            var tokenConfigurations = new TokenConfigurations() { Days = 90 };
            new ConfigureFromConfigurationOptions<TokenConfigurations>(configuration.GetSection("Authentication")).Configure(tokenConfigurations);
            services.AddSingleton(tokenConfigurations);

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearerOptions =>
            {
                var paramsValidation = bearerOptions.TokenValidationParameters;
                paramsValidation.IssuerSigningKey = signingConfigurations.Key;
                paramsValidation.ValidAudience = tokenConfigurations.Audience;
                paramsValidation.ValidIssuer = tokenConfigurations.Issuer;
                paramsValidation.ValidateIssuerSigningKey = true;
                paramsValidation.ValidateLifetime = true;
                paramsValidation.ClockSkew = TimeSpan.Zero;
                paramsValidation.SaveSigninToken = true;
                bearerOptions.SaveToken = true;
            });

            services.AddAuthorization();

            return services;
        }
    }
}
