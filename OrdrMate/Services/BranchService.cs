namespace OrdrMate.Services;

using OrdrMate.DTOs;
using OrdrMate.Models;
using OrdrMate.Repositories;
using OrdrMate.Utils;

public class BranchService
{
    private readonly IBranchRepo _branchRepo;
    private readonly ManagerService _managerService;
    private readonly RestaurantService _restaurantService;

    public BranchService(IBranchRepo branchRepo, ManagerService managerService, RestaurantService restaurantService)
    {
        _branchRepo = branchRepo;
        _managerService = managerService;
        _restaurantService = restaurantService;
    }

    public async Task<BranchDto> GetBranchById(string id)
    {
        var branch = await _branchRepo.GetBranchById(id);
        if (branch == null)
        {
            throw new KeyNotFoundException($"Branch with id {id} not found.");
        }
        return new BranchDto
        {
            BranchId = branch.Id,
            Lantitude = branch.Lantitude,
            Longitude = branch.Longitude,
            BranchAddress = branch.Address,
            RestaurantId = branch.RestaurantId,
        };
    }

    public async Task<IEnumerable<BranchDto>> GetAllBranches()
    {
        var branches = await _branchRepo.GetAllBranches();
        return branches.Select(b => new BranchDto
        {
            BranchId = b.Id,
            Lantitude = b.Lantitude,
            Longitude = b.Longitude,
            BranchAddress = b.Address,
            RestaurantId = b.RestaurantId,
        });
    }

    public async Task<BranchApprovalDto> CreateBranch(BranchDto branchDto)
    {

        var restaurant = await _restaurantService.GetRestaurantById(branchDto.RestaurantId);

        if (restaurant == null)
        {
            throw new KeyNotFoundException($"Restaurant with id {branchDto.RestaurantId} not found.");
        }

        var username = RandomGenerator.GenerateRandomString(9, restaurant.Name);
        var password = RandomGenerator.GenerateRandomPassword(8);

        var createdManager = await _managerService.CreateManager(new CreateManagerDTO
        {
            Username = username,
            Password = password,
        });

        if (createdManager == null)
        {
            throw new Exception("Failed to create manager.");
        }
    
        var branch = new Branch
        {
            Id = Guid.NewGuid().ToString(),
            Lantitude = branchDto.Lantitude,
            Longitude = branchDto.Longitude,
            Address = branchDto.BranchAddress,
            Phone = branchDto.BranchPhoneNumber,
            RestaurantId = branchDto.RestaurantId,
            BranchManagerId = createdManager.Id,
        };

        var createdBranch = await _branchRepo.CreateBranch(branch);
        return new BranchApprovalDto
        {
            BranchId = createdBranch.Id,
            BranchAddress = createdBranch.Address,
            RestaurantId = createdBranch.RestaurantId,
            BranchPhoneNumber = createdBranch.Phone,
            BranchManagerId = createdBranch.BranchManagerId,
            BranchManagerUsername = username,
            BranchManagerPassword = password,
        };
    }

}