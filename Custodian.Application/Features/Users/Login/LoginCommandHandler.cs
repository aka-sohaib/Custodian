using Custodian.Application.Common.Interfaces;
using Custodian.Domain.Interfaces;
using Custodian.Application.Common.Exceptions;
using MediatR;
using Custodian.Domain.Entities;

namespace Custodian.Application.Features.Users.Login;

public class LoginCommandHandler: IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider    _jwtProvider;

    public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider    = jwtProvider;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        //---- Hash Password & Verify ----
        var user = await _userRepository.GetByEmailAsync(request.Email);
        // change back the message to this: Invalid Email or Password.
        if (user == null) 
        {
            throw new UnauthorizedException("cant find user");
        }

        var isVerified = _passwordHasher.Verify(user.PasswordHash, request.Password);
        if (!isVerified)
        {
            throw new UnauthorizedException("password doesnt match");
        }

        //---- extracting user role ----
        string userRole = user switch
        {
            InternalUser internalUser => internalUser.InternalUserRole.ToString(),
            VendorUser vendorUser => vendorUser.VendorUserRole.ToString(),
            _ => "DefaultRole"
        };

        //---- generate Json Web Token ----
        var token = _jwtProvider.GenerateToken(user.Id, user.Email, userRole);

        return new LoginResponse(token);
    }
}
