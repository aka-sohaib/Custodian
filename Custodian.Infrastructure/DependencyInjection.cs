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
}
