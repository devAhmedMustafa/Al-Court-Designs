using Microsoft.AspNetCore.Mvc;
using OrdrMate.DTOs;
using OrdrMate.Services;

namespace OrdrMate.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ManagerController(ManagerService s) : ControllerBase {

    private readonly ManagerService _service = s;

    [HttpGet]
    public async Task<IActionResult> GetManagers(){
        var managers = await _service.GetAllManagers();
        return Ok(managers);
    }

    [HttpPost]
    public async Task<ActionResult> Signup([FromBody] CreateManagerDTO data){

        if (!ModelState.IsValid){
            return BadRequest(ModelState);
        }

        var createdManager = await _service.CreateManager(data);
        return Ok(createdManager);
    }

}