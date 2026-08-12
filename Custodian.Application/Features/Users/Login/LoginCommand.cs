using MediatR;

namespace Custodian.Application.Features.Users.Login;

public record LoginResponse(string Token);
public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;