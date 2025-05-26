using Microsoft.AspNetCore.Mvc;
using OrdrMate.DTOs.User;
using OrdrMate.Services;

namespace OrdrMate.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController(CustomerService service) : ControllerBase
{

    private readonly CustomerService _service = service ?? throw new ArgumentNullException(nameof(service));

    [HttpPost]
    public async Task<ActionResult<GoogleLoginCredentialsDto>> GoogleLogin([FromBody] GoogleLoginRequestDto dto)
    {
        if (dto == null || string.IsNullOrEmpty(dto.IdToken))
        {
            return BadRequest("Google login credentials are required.");
        }

        try
        {
            var response = await _service.AuthenticateCustomer(dto);

            if (response == null)
                return Unauthorized(new { err = "Invalid Google ID token or user not found." });

            return Ok(response);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Credentials"))
                return Unauthorized(new { err = ex.Message });
            else if (ex.Message.Contains("token"))
                return BadRequest(new { err = "Invalid Google ID token." });
            else
                return StatusCode(500, new { err = "Internal server error while processing Google login." });
        }
         
    }
}