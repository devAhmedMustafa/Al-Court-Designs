using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdrMate.DTOs.Kitchen;
using OrdrMate.Services;

namespace OrdrMate.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KitchenController : ControllerBase
{

    private readonly KitchenService _kitchenService;
    private readonly IAuthorizationService _authorizationService;
    public KitchenController(KitchenService kitchenService, IAuthorizationService authorizationService)
    {
        _kitchenService = kitchenService;
        _authorizationService = authorizationService;
    }

    [HttpPost]
    public async Task<ActionResult<KitchenDto>> AddKitchen([FromBody] AddKitchenDto kitchenDto)
    {

        if (kitchenDto == null)
        {
            return BadRequest("Kitchen data is required.");
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, kitchenDto.RestaurantId, "CanManageRestaurant");

        if (!authorizationResult.Succeeded)
        {
            return Forbid("You do not have permission to manage this restaurant.");
        }

        try
        {
            var createdKitchen = await _kitchenService.AddKitchen(kitchenDto);
            if (createdKitchen == null)
            {
                return BadRequest("Failed to create kitchen.");
            }
            return CreatedAtAction(nameof(AddKitchen), new { id = createdKitchen.Id }, createdKitchen);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error creating kitchen: {ex.Message}");
            return StatusCode(500, "Internal server error while creating kitchen.");
        }
    }

    [HttpGet("{restaurantId}")]
    public async Task<ActionResult<IEnumerable<KitchenDto>>> GetKitchensByRestaurantId(string restaurantId)
    {
        if (string.IsNullOrEmpty(restaurantId))
        {
            return BadRequest("Restaurant ID is required.");
        }
        try
        {
            var kitchens = await _kitchenService.GetKitchensByRestaurantId(restaurantId);
            if (kitchens == null || !kitchens.Any())
            {
                return NotFound("No kitchens found for the specified restaurant.");
            }
            return Ok(kitchens);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error retrieving kitchens: {ex.Message}");
            return StatusCode(500, "Internal server error while retrieving kitchens.");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteKitchen(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Kitchen ID is required.");
        }
        try
        {
            var kitchen = await _kitchenService.GetKitchenById(id);
            if (kitchen == null)
            {
                return NotFound("Kitchen not found.");
            }
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, kitchen.RestaurantId, "CanManageRestaurant");

            if (!authorizationResult.Succeeded)
            {
                return Forbid("You do not have permission to delete this kitchen.");
            }
            var isDeleted = await _kitchenService.DeleteKitchen(id);
            if (!isDeleted)
            {
                return NotFound("Kitchen not found or could not be deleted.");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error deleting kitchen: {ex.Message}");
            return StatusCode(500, "Internal server error while deleting kitchen.");
        }
    }

    [HttpGet("power/{branchId}/{kitchenId}")]
    public async Task<ActionResult<KitchenPowerDto>> GetKitchenPower(string branchId, string kitchenId)
    {
        if (string.IsNullOrEmpty(branchId) || string.IsNullOrEmpty(kitchenId))
        {
            return BadRequest("Branch ID and Kitchen ID are required.");
        }
        try
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, branchId, "branchManager");
            if (!authorizationResult.Succeeded)
            {
                return Forbid("You do not have permission to access this branch.");
            }
            var kitchenPower = await _kitchenService.GetKitchenPower(branchId, kitchenId);
            if (kitchenPower == null)
            {
                return NotFound("Kitchen power not found for the specified branch and kitchen.");
            }
            return Ok(kitchenPower);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error retrieving kitchen power: {ex.Message}");
            return StatusCode(500, "Internal server error while retrieving kitchen power.");
        }
    }

    // [HttpPost("power")]
    // public async Task<ActionResult<KitchenPowerDto>> AddKitchenPower([FromBody] AddKitchenPowerDto kitchenPowerDto)
    // {
    //     if (kitchenPowerDto == null)
    //     {
    //         return BadRequest("Kitchen power data is required.");
    //     }
    //     try
    //     {
    //         var authorizationResult = await _authorizationService.AuthorizeAsync(User, kitchenPowerDto.BranchId, "branchManager");
    //         if (!authorizationResult.Succeeded)
    //         {
    //             return Forbid("You do not have permission to manage this branch.");
    //         }
    //         var createdKitchenPower = await _kitchenService.AddKitchenPower(kitchenPowerDto);
    //         if (createdKitchenPower == null)
    //         {
    //             return BadRequest("Failed to create kitchen power.");
    //         }
    //         return CreatedAtAction(nameof(AddKitchenPower), new
    //         {
    //             id = new
    //             {
    //                 createdKitchenPower.BranchId,
    //                 createdKitchenPower.KitchenId
    //             }
    //         }, createdKitchenPower);
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.Error.WriteLine($"Error creating kitchen power: {ex.Message}");
    //         return StatusCode(500, "Internal server error while creating kitchen power.");
    //     }
    // }

    [HttpPut("power/{branchId}/{kitchenId}")]
    public async Task<ActionResult<KitchenPowerDto>> UpdateKitchenPower(string branchId, string kitchenId, [FromBody] UpdateKitchenPowerDto dto)
    {
        try
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, branchId, "branchManager");
            if (!authorizationResult.Succeeded)
            {
                return Forbid("You do not have permission to manage this branch");
            }

            var updatedKitchenPower = await _kitchenService.UpdateKitchenPower(branchId, kitchenId, dto);
            return Ok(updatedKitchenPower);
        }
        catch (Exception e)
        {
            return BadRequest(new { err = e.Message });
        }
    }

}