using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OrdrMate.DTOs;
using OrdrMate.Services;

namespace OrdrMate.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class RestaurantController(RestaurantService r) : ControllerBase
{
    private readonly RestaurantService _service = r;

    [HttpPost]
    public async Task<ActionResult<RestaurantController>> CreateRestaurant([FromBody] CreateRestaurantDto dto)
    {
        try
        {
            var result = await _service.CreateRestaurant(dto);
            return CreatedAtAction(nameof(CreateRestaurant), new { id = result.Id }, result);
        }
        catch (Exception e)
        {
            if (e.Message.Contains("already exists"))
                return Conflict(new { err = e.Message });

            return BadRequest(new { err = e.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<RestaurantDTO>> GetRestaurantByManagerId()
    {
        try
        {
            var managerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(managerId))
            {
                return Unauthorized(new { err = "Unauthorized" });
            }

            var restaurant = await _service.GetRestaurantByManagerId(managerId);
            if (restaurant == null)
            {
                return NotFound(new { err = "No restaurant found for this manager" });
            }

            return Ok(restaurant);
        }
        catch (Exception e)
        {
            return BadRequest(new { err = e.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RestaurantDTO>> GetRestaurantById(string id)
    {
        try
        {
            var restaurant = await _service.GetRestaurantById(id);
            if (restaurant == null)
            {
                return NotFound(new { err = "No restaurant found with this ID" });
            }

            return Ok(restaurant);
        }
        catch (Exception e)
        {
            return BadRequest(new { err = e.Message });
        }
    }

    [HttpGet("list/all")]
    public async Task<ActionResult<List<RestaurantDTO>>> GetAllRestaurants()
    {
        try
        {
            var restaurants = await _service.GetAllRestaurants();
            return Ok(restaurants);
        }
        catch (Exception e)
        {
            return BadRequest(new { err = e.Message });
        }
    }

    [HttpGet("categories/{restaurantId}")]
    public async Task<ActionResult<List<CategoryDto>>> GetRestaurantCategories(string restaurantId)
    {
        try
        {
            var categories = await _service.GetRestaurantCategories(restaurantId);
            if (categories == null || !categories.Any())
            {
                return NotFound(new { err = "No categories found for this restaurant" });
            }
            return Ok(categories);
        }
        catch (Exception e)
        {
            return BadRequest(new { err = e.Message });
        }
    }
}