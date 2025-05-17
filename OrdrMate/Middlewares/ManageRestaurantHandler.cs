namespace OrdrMate.Middlewares;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using OrdrMate.Repositories;

public class ManageRestaurantHandler : AuthorizationHandler<ManageRestaurantRequirement, string>
{
    private readonly IRestaurantRepo _repo;
    public ManageRestaurantHandler(IRestaurantRepo repo)
    {
        _repo = repo;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ManageRestaurantRequirement requirement,
        string restaurantId
        )
    {
        var managerId = context.User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(managerId) || string.IsNullOrEmpty(restaurantId))
        {
            return;
        }

        bool hasAccess = await _repo.HasAccessToRestaurant(managerId, restaurantId);
        if (hasAccess)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}