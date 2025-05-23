using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace OrdrMate.Middlewares;
using OrdrMate.Repositories;

public class BranchManagerHandler : AuthorizationHandler<BranchManagerRequirement, string>
{
    private readonly IBranchRepo _repo;
    public BranchManagerHandler(IBranchRepo repo)
    {
        _repo = repo;
    }
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BranchManagerRequirement requirement, string branchId)
    {

        var managerId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (managerId == null || branchId == null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (_repo.HasAccess(branchId, managerId).Result)
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