using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Custodian.Infrastructure.Repositories;
using Custodian.Application.Common.Interfaces;
using Custodian.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Custodian.Infrastructure.Notifications;
using Custodian.Infrastructure.Scanners;
using Resend;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Options;
namespace Custodian.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CustodianDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // ---- Register Repositories ----
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IInternalUserRepository, InternalUserRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVendorUserRepository, VendorUserRepository>();
        services.AddScoped<IInvitationRepository, InvitationRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IOrganizationConnectionRepository, OrganizationConnectionRepository>();
        services.AddTransient<IEmailSender, ResendEmailSender>();

        // ---- Register Scanners ----
        services.AddScoped<IInvoiceScanner, AzureInvoiceScanner>();

        // ---- Register Security Services ----
        services.AddScoped<IPasswordHasher, Argon2PasswordHasher>();
        services.AddScoped<IJwtProvider, JwtProvider>();
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

        // ---- Register resend ----
        var resendApiKey = configuration["Resend:ApiKey"] ?? string.Empty;
        services.AddResend(options => options.ApiToken = resendApiKey);

        return services;
    }

    //---- Add Jwt Authentication Service ----
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        //---- Create Jwt object from Jwt Configurations ----
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("JwtSettings section is missing or invalid.");

        //---- Add Authtication & setting jwtBearer as default ----
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        })
        //---- add configurations on how to authenticate ----
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
            };
        });

        return services;
    }
}
