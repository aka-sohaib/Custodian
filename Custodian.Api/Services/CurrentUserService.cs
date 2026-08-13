using Custodian.Application.Common.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Custodian.Api.Services
{
    public class CurrentUserService: ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var userIdString = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                                ?? user?.FindFirstValue(JwtRegisteredClaimNames.Sub)
                                ?? user?.FindFirstValue("sub");

                return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
            }
        }
        public string Role
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                return user?.FindFirstValue(ClaimTypes.Role)
                    ?? user?.FindFirstValue("role")
                    ?? string.Empty;
            }
        }
    }
}
