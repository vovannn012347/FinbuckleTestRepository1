
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebApplication2;

public class CustomTenantControlMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IMultiTenantContextAccessor<AppTenantInfo> _tenantContextAccessor;
    
    public CustomTenantControlMiddleware(RequestDelegate next, 
        IMultiTenantContextAccessor<AppTenantInfo> tenantContextAccessor)
    {
        _next = next;
        _tenantContextAccessor = tenantContextAccessor;
    }

    public async Task Invoke(HttpContext context)
    {
        var principal = context.User;

        //if (principal?.Identity?.IsAuthenticated == true)
        //{
            //var currentTenant = _tenantContextAccessor.MultiTenantContext.TenantInfo;
            //var claim = principal.Claims.FirstOrDefault(c => c.Type == Constants.TenantClaimType);

            //if (currentTenant != null && claim != null && currentTenant.Id != claim.Value)
            //{
            //    await context.SignOutAsync();
            //}
        //}

        await _next(context);
    }
}