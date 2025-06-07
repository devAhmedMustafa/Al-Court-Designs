using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdrMate.Services;

namespace OrdrMate.Controllers;
using OrdrMate.DTOs.Table;
using OrdrMate.Managers;

[ApiController]
[Route("api/[controller]")]
public class TableController : ControllerBase
{
    private readonly TableService _tableService;
    private readonly IAuthorizationService _authorizationService;
    private readonly TableManager _tableManager;

    public TableController(TableService tableService, IAuthorizationService authorizationService, TableManager tableManager)
    {
        _tableManager = tableManager;
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

    [HttpGet("free/{branchId}")]
    public async Task<IActionResult> GetFreeTablesOfBranch(string branchId)
    {
        var authorization = await _authorizationService.AuthorizeAsync(User, branchId, "BranchManager");
        if (!authorization.Succeeded)
        {
            return Forbid();
        }

        var freeTables = await _tableService.GetFreeTableCount(branchId);
        return Ok(freeTables);
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

    [HttpGet("min_waiting_time/{branchId}/{seats}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMinWaitingTime(string branchId, int seats)
    {
        var minWaitingTime = await _tableManager.GetMinimumWaitingTime(branchId, seats);
        return Ok(new { minWaitingTime });
    }

    [HttpGet("estimated_waiting_time/{reservationId}")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetEstimatedWaitingTime(string reservationId)
    {
        var estimatedWaitingTime = await _tableManager.GetOrderPosition(reservationId);
        return Ok(new { estimatedWaitingTime });
    }

}