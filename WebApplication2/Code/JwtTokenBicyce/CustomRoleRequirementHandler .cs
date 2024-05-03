using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication2.Code.JwtTokenBicyce
{
    public class TenantHeaderRoleMatchRequirementHandler : AuthorizationHandler<TenantHeaderRoleMatchRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantHeaderRoleMatchRequirementHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantHeaderRoleMatchRequirement requirement)
        {
            var headerValue = _httpContextAccessor.HttpContext.Request.Headers[requirement.HeaderName];
            var roleValue = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == requirement.RoleName);

            // Check if the header value matches the required role
            if (!headerValue.IsNullOrEmpty() && roleValue != null && headerValue.ToString() == roleValue.Value)
            {
                context.Succeed(requirement);
            }else
            if (headerValue.IsNullOrEmpty() && !(roleValue != null))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
