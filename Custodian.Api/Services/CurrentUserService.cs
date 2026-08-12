using Custodian.Application.Common.Interfaces;
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
                var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
            }
        }
        public string Role
        {
            get
            {
                return _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
            }
        }
    }
}
