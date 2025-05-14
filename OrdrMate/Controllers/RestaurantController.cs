using Microsoft.AspNetCore.Mvc;
using OrdrMate.DTOs;
using OrdrMate.Services;

namespace OrdrMate.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class RestaurantController(RestaurantService r) : ControllerBase {
    private readonly RestaurantService _service = r;

    [HttpPost]
    public async Task<ActionResult<RestaurantController>> CreateRestaurant([FromBody] CreateRestaurantDto dto){
        try {
            var result = await _service.CreateRestaurant(dto);
            return CreatedAtAction(nameof(CreateRestaurant), new{id=result.Id}, result);
        }
        catch(Exception e){
            if (e.Message.Contains("already exists"))
                return Conflict(new {err=e.Message});

            return BadRequest(new {err=e.Message});
        }
    }
}