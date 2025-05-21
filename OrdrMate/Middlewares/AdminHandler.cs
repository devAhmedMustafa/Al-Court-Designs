using Microsoft.AspNetCore.Authorization;
using OrdrMate.Middlewares;
using OrdrMate.Models;

namespace OrdrMate.Middlewares;

public class AdminHandler : AuthorizationHandler<AdminRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AdminRequirement requirement
    )
    {
        if (context.User.IsInRole(ManagerRole.Admin.ToString()))
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