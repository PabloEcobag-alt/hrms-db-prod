using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace api_hrm.Authorization;

public sealed class AppPermissionRequirement(
    string systemCode,
    string moduleName,
    string action) : IAuthorizationRequirement
{
    public string SystemCode { get; } = systemCode;
    public string ModuleName { get; } = moduleName;
    public string Action { get; } = action;
}

public sealed class AppPermissionAuthorizationHandler
    : AuthorizationHandler<AppPermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AppPermissionRequirement requirement)
    {
        if (IsSuperUser(context.User) || HasPermission(context.User, requirement))
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }

    private static bool IsSuperUser(ClaimsPrincipal user) =>
        bool.TryParse(user.FindFirst("isSuperUser")?.Value, out var value) && value;

    private static bool HasPermission(ClaimsPrincipal user, AppPermissionRequirement requirement)
    {
        var permissions = user.FindFirst("permissions")?.Value;
        if (string.IsNullOrWhiteSpace(permissions)) return false;

        try
        {
            using var document = JsonDocument.Parse(permissions);
            return document.RootElement.TryGetProperty(requirement.SystemCode, out var app)
                && app.TryGetProperty(requirement.ModuleName, out var module)
                && module.TryGetProperty(requirement.Action, out var allowed)
                && allowed.ValueKind == JsonValueKind.True;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}