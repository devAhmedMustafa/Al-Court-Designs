using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdrMate.DTOs;

namespace OrdrMate.Controllers;

public class ItemController : ControllerBase
{
    private readonly ItemService _service;
    private readonly IAuthorizationService _authorizationService;

    private ItemController(ItemService service, IAuthorizationService authorizationService)
    {
        _service = service;
        _authorizationService = authorizationService;
    }

    [HttpPost]
    public async Task<ActionResult<ItemDto>> CreateItem([FromBody] AddItemDto dto)
    {
        try
        {
            var authorizationResult = await _authorizationService.AuthorizeAsync(User, dto.RestaurantId, "CanManageRestaurant");

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var result = await _service.AddItem(dto);
            return CreatedAtAction(nameof(CreateItem), new { id = result?.Id }, result);
        }
        catch (Exception e)
        {
            if (e.Message.Contains("already exists"))
                return Conflict(new { err = e.Message });

            return BadRequest(new { err = e.Message });
        }
    }
}