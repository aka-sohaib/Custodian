using MediatR;

namespace Custodian.Application.Features.Organizations.Commands.RegisterOrganization;

public record RegisterOrganizationCommand(
    //---- Organization Details ----
    string OrganizationName,
    string OrganizationEmail,
    string OrganizationPhone,
    bool IsCompany,
    bool IsVendor,

    //---- Admin User Details ----
    string AdminName,
    string AdminEmail,
    string AdminPassword
) : IRequest<Guid>;
