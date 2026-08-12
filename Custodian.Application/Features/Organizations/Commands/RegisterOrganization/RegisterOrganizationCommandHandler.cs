using Custodian.Application.Common.Exceptions;
using Custodian.Application.Common.Interfaces;
using Custodian.Domain.Entities;
using Custodian.Domain.Enums;
using Custodian.Domain.Interfaces;
using MediatR;

namespace Custodian.Application.Features.Organizations.Commands.RegisterOrganization;

public class RegisterOrganizationCommandHandler : IRequestHandler<RegisterOrganizationCommand, Guid>
{
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IInternalUserRepository _internalUserRepository;
    private readonly IVendorUserRepository _vendorUserRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterOrganizationCommandHandler(
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IInternalUserRepository internalUserRepository,
        IVendorUserRepository vendorUserRepository,
        IPasswordHasher passwordHasher)
    {
        _organizationRepository = organizationRepository;
        _userRepository         = userRepository;
        _internalUserRepository = internalUserRepository;
        _vendorUserRepository   = vendorUserRepository;
        _passwordHasher         = passwordHasher;
    }

    public async Task<Guid> Handle(RegisterOrganizationCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate Organization Email Uniqueness
        var isOrgEmailUnique = await _organizationRepository.IsEmailUniqueAsync(request.OrganizationEmail, null);
        if (!isOrgEmailUnique)
        {
            throw new ConflictException($"This organization email '{request.OrganizationEmail}' is already registered.");
        }

        // 2. Validate Organization Phone Uniqueness
        var isOrgPhoneUnique = await _organizationRepository.IsPhoneUniqueAsync(request.OrganizationPhone, null);
        if (!isOrgPhoneUnique)
        {
            throw new ConflictException($"This organization phone '{request.OrganizationPhone}' is already registered.");
        }

        // 3. Validate User Email Uniqueness across all users
        var isUserEmailUnique = await _userRepository.IsEmailUniqueAsync(request.AdminEmail, null);
        if (!isUserEmailUnique)
        {
            throw new ConflictException($"This user email '{request.AdminEmail}' is already registered.");
        }

        // 4. Create Organization Entity
        var organization = Organization.Create(
            request.OrganizationName,
            request.OrganizationPhone,
            request.OrganizationEmail,
            request.IsCompany,
            request.IsVendor
        );

        // 5. Hash Password & Create Admin User Entity
        var passwordHash = _passwordHasher.Hash(request.AdminPassword);

        await _organizationRepository.AddAsync(organization);

        if (request.IsCompany)
        {
            var internalAdmin = InternalUser.CreateInternalUser(
                request.AdminName,
                request.AdminEmail,
                passwordHash,
                InternalUserRole.Admin,
                organization.Id
            );
            await _internalUserRepository.AddAsync(internalAdmin);
        }
        else
        {
            var vendorAdmin = VendorUser.CreateVendorUser(
                request.AdminName,
                request.AdminEmail,
                passwordHash,
                organization.Id,
                VendorUserRole.Admin
            );
            await _vendorUserRepository.AddAsync(vendorAdmin);
        }

        return organization.Id;
    }
}
