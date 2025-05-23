using Microsoft.AspNetCore.Mvc;
using OrdrMate.DTOs;
using OrdrMate.Services;

namespace OrdrMate.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ManagerController(ManagerService s) : ControllerBase
{

    private readonly ManagerService _service = s;

    [HttpGet]
    public async Task<IActionResult> GetManagers()
    {
        var managers = await _service.GetAllManagers();
        return Ok(managers);
    }

    [HttpPost]
    public async Task<ActionResult<ManagerDTO>> RegisterManager([FromBody] CreateManagerDTO data)
    {

        try
        {
            var result = await _service.CreateManager(data);
            return CreatedAtAction(nameof(RegisterManager), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("already exists"))
                return Conflict(new { err = ex.Message });

            return BadRequest(new { err = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginSuccessDto>> LoginManager([FromBody] LoginDTO data)
    {
        try
        {
            var result = await _service.AuthenticateManager(data);
            return Ok(result);
        }
        catch (Exception ex)
        {
            if (ex.Message.Contains("Credentials"))
                return Unauthorized(new { err = ex.Message });

            return BadRequest(new { err = ex.Message });
        }
    }    

}