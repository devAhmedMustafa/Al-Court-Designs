using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdrMate.DTOs;
using OrdrMate.Repositories;
using OrdrMate.Services;

namespace OrdrMate.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BranchController : ControllerBase
{
    private readonly IBranchRequestRepo _branchRequestRepo;
    private readonly IAuthorizationService _authorizationService;
    private readonly BranchService _branchService;

    public BranchController(IBranchRequestRepo branchRequestRepo, IAuthorizationService authorizationService, BranchService branchService)
    {
        _authorizationService = authorizationService;
        _branchRequestRepo = branchRequestRepo;
        _branchService = branchService;
    }

    [HttpGet]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetAllBranchRequests()
    {
        var branchRequests = await _branchRequestRepo.GetAllBranchRequests();

        var branchRequestsDto = branchRequests.Select(br => new BranchRequestDto
        {
            BranchRequestId = br.Id,
            RestaurantName = br.Restaurant.Name,
            BranchAddress = br.Address,
            BranchPhoneNumber = br.Phone,
            Lantitude = br.Lantitude,
            Longitude = br.Longitude

        }).ToList();
        return Ok(branchRequestsDto);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> GetBranchRequestById(string id)
    {
        var branchRequest = await _branchRequestRepo.GetBranchRequestById(id);
        if (branchRequest == null)
        {
            return NotFound($"Branch request with id {id} not found.");
        }
        var branchRequestDto = new BranchRequestDto
        {
            BranchRequestId = branchRequest.Id,
            RestaurantName = branchRequest.Restaurant.Name,
            BranchAddress = branchRequest.Address,
            BranchPhoneNumber = branchRequest.Phone,
            Lantitude = branchRequest.Lantitude,
            Longitude = branchRequest.Longitude
        };
        return Ok(branchRequestDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBranchRequest([FromBody] AddBranchRequestDto branchRequestDto)
    {

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, branchRequestDto.RestaurantId, "CanManageRestaurant");

        if (!authorizationResult.Succeeded)
        {
            return Forbid("You do not have permission to manage this restaurant.");
        }

        if (branchRequestDto == null)
        {
            return BadRequest("Branch request data is required.");
        }

        var branchRequest = new Models.BranchRequest
        {
            Id = Guid.NewGuid().ToString(),
            RestaurantId = branchRequestDto.RestaurantId,
            Address = branchRequestDto.BranchAddress,
            Phone = branchRequestDto.BranchPhoneNumber,
            Lantitude = branchRequestDto.Lantitude,
            Longitude = branchRequestDto.Longitude
        };

        var createdBranchRequest = await _branchRequestRepo.CreateBranchRequest(branchRequest);
        return CreatedAtAction(nameof(GetBranchRequestById), new { id = createdBranchRequest.Id }, new BranchRequestDto
        {
            BranchRequestId = createdBranchRequest.Id,
            RestaurantName = createdBranchRequest.Restaurant.Name,
            BranchAddress = createdBranchRequest.Address,
            BranchPhoneNumber = createdBranchRequest.Phone,
            Lantitude = createdBranchRequest.Lantitude,
            Longitude = createdBranchRequest.Longitude
        });
    }

    [HttpPost("{id}")]
    [Authorize(Policy = "Admin")]
    public async Task<IActionResult> ApproveBranchRequest(string id)
    {
        var branchRequest = await _branchRequestRepo.GetBranchRequestById(id);
        if (branchRequest == null)
        {
            return NotFound($"Branch request with id {id} not found.");
        }

        var branchCreated = await _branchService.CreateBranch(new BranchDto {
            Lantitude = branchRequest.Lantitude,
            Longitude = branchRequest.Longitude,
            BranchAddress = branchRequest.Address,
            BranchPhoneNumber = branchRequest.Phone,
            RestaurantId = branchRequest.RestaurantId
        });

        if (branchCreated == null)
        {
            return BadRequest("Failed to create branch.");
        }

        var isDeleted = await _branchRequestRepo.DeleteBranchRequest(id);
        if (!isDeleted)
        {
            return BadRequest("Failed to delete branch request.");
        }

        return CreatedAtAction(nameof(GetBranchRequestById), new { id = branchCreated.BranchId }, branchCreated);
    }

}