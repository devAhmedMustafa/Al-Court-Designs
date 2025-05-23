using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdrMate.Services;

namespace OrdrMate.Controllers;
using OrdrMate.DTOs;

[ApiController]
[Route("api/[controller]")]
public class TableController : ControllerBase
{
    private readonly TableService _tableService;
    private readonly IAuthorizationService _authorizationService;

    public TableController(TableService tableService, IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
        _tableService = tableService;
    }

    [HttpGet("{branchId}")]
    public async Task<IActionResult> GetAllTablesOfBranch(string branchId)
    {
        var authorization = await _authorizationService.AuthorizeAsync(User, branchId, "BranchManager");
        if (!authorization.Succeeded)
        {
            return Forbid();
        }

        var tables = await _tableService.GetAllTablesOfBranch(branchId);
        return Ok(tables);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTable([FromBody] AddTableDto tableDto)
    {
        var authorization = await _authorizationService.AuthorizeAsync(User, tableDto.BranchId, "BranchManager");
        if (!authorization.Succeeded)
        {
            return Forbid();
        }

        var createdTable = await _tableService.CreateTable(tableDto);
        return CreatedAtAction(nameof(GetAllTablesOfBranch), new { branchId = tableDto.BranchId }, createdTable);
    }

    [HttpDelete("{branchId}/{tableNum}")]
    public async Task<IActionResult> DeleteTable(string branchId, int tableNum)
    {
        var authorization = await _authorizationService.AuthorizeAsync(User, branchId, "BranchManager");
        if (!authorization.Succeeded)
        {
            return Forbid();
        }

        var result = await _tableService.DeleteTable(branchId, tableNum);
        if (result)
        {
            return NoContent();
        }
        return NotFound();
    }

}