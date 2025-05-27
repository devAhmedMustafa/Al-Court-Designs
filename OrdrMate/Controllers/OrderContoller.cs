using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdrMate.DTOs.Order;
using OrdrMate.Services;

namespace OrdrMate.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{

    private readonly OrderService _orderService;
    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<ActionResult> PlaceOrder([FromBody] PlaceOrderDto placeOrderDto)
    {
        try
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Forbid("User ID not found in claims.");


            placeOrderDto.CustomerId = userId;

            var order = await _orderService.PlaceOrder(placeOrderDto);
            if (order == null)
                return BadRequest("Failed to place order. Please check your order details and try again.");

            return CreatedAtAction(nameof(PlaceOrder), new { orderId = order.OrderId }, order);

        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while processing your request: {ex.Message}");
        }
        
    }
}