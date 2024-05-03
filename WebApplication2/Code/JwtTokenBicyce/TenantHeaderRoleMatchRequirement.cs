using Microsoft.AspNetCore.Authorization;

//
public class TenantHeaderRoleMatchRequirement : IAuthorizationRequirement
{
    public string RoleName { get; }
    public string HeaderName { get; }

    public TenantHeaderRoleMatchRequirement(string role)
    {
        RoleName = role;
        HeaderName = role;
    }
}