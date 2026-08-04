using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Custodian.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Custodian.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CustodianDbContext>(options =>options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAuditLogRepository,        AuditLogRepository>();
        services.AddScoped<ICategoryRepository,        CategoryRepository>();
        services.AddScoped<IInternalUserRepository,    InternalUserRepository>();
        services.AddScoped<IInvoiceRepository,         InvoiceRepository>();
        services.AddScoped<IUserRepository,            UserRepository>();
        services.AddScoped<IVendorRepository,          VendorRepository>();
        services.AddScoped<IVendorUserRepository,      VendorUserRepository>();
        services.AddScoped<IInvitationRepository,      InvitationRepository>();
        services.AddScoped<ICompanyRepository,         CompanyRepository>();
        services.AddScoped<ICompanyVendorRepository, CompanyVendorRepository>();

        return services;
    }
}
