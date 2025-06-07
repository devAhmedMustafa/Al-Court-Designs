namespace OrdrMate.Repositories;

using Microsoft.EntityFrameworkCore;
using OrdrMate.Data;
using OrdrMate.Models;

public class BranchRepo : IBranchRepo
{
    private readonly OrdrMateDbContext _context;

    public BranchRepo(OrdrMateDbContext context)
    {
        _context = context;
    }

    public async Task<Branch> GetBranchById(string id)
    {
        return await _context.Branch.Include(b => b.Restaurant)
        .FirstOrDefaultAsync(b => b.Id == id)
        ?? throw new KeyNotFoundException($"Branch with id {id} not found.");
    }

    public async Task<Branch> GetDetailedBranchById(string id)
    {
        return await _context.Branch
        .Include(b => b.KitchenPowers)!.ThenInclude(kp => kp.Kitchen)
        .Include(b => b.Orders!.OrderBy(o => o.OrderDate)).ThenInclude(o => o.OrderItems)!.ThenInclude(oi => oi.Item).ThenInclude(i => i.Kitchen)
        .Include(b => b.Tables)
        .AsSplitQuery()
        .AsNoTracking()
        .FirstOrDefaultAsync(b => b.Id == id)
        ?? throw new KeyNotFoundException($"Branch with id {id} not found.");
    }

    public async Task<IEnumerable<Branch>> GetAllBranches()
    {
        return await _context.Branch
        .Include(b => b.Restaurant)
        .Include(b => b.Tables)
        .Include(b => b.Orders!.OrderBy(o => o.OrderDate)).ThenInclude(o => o.OrderItems)!.ThenInclude(oi => oi.Item).ThenInclude(i => i.Kitchen)
        .Include(b => b.KitchenPowers)!.ThenInclude(kp => kp.Kitchen)
        .AsSplitQuery()
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<IEnumerable<Branch>> GetRestaurantBranches(string restaurantId)
    {
        return await _context.Branch
            .Where(b => b.RestaurantId == restaurantId)
            .ToListAsync();
    }

    public async Task<Branch> GetBranchByManagerId(string managerId)
    {
        return await _context.Branch
            .FirstOrDefaultAsync(b => b.BranchManagerId == managerId)
            ?? throw new KeyNotFoundException($"Branch with manager id {managerId} not found.");
    }

    public async Task<Branch> CreateBranch(Branch branch)
    {
        await _context.Branch.AddAsync(branch);
        await _context.SaveChangesAsync();

        return branch;
    }

    public async Task<bool> DeleteBranch(string id)
    {
        var branch = await GetBranchById(id);
        if (branch == null)
        {
            return false;
        }

        _context.Branch.Remove(branch);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateBranch(Branch branch)
    {
        _context.Branch.Update(branch);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BranchExists(string id)
    {
        return await _context.Branch.AnyAsync(b => b.Id == id);
    }

    public async Task<bool> HasAccess(string branchId, string managerId)
    {
        var branch = await _context.Branch
            .FirstOrDefaultAsync(b => b.Id == branchId);

        if (branch == null)
        {
            return false;
        }

        if (branch.BranchManagerId == managerId) return true;

        var restaurant = await _context.Restaurant
            .Include(r => r.Branches)
            .FirstOrDefaultAsync(r => r.Id == branch.RestaurantId);

        return restaurant?.ManagerId == managerId;
    }

    public async Task<int> GetFreeTables(string branchId)
    {
        var tables = await _context.Table.CountAsync(t => t.BranchId == branchId);
        return tables;
    }

    public async Task<int> GetOrdersInQueue(string branchId)
    {
        var orders = await _context.Order
            .Where(o => o.BranchId == branchId && o.Status == Enums.OrderStatus.Queued)
            .CountAsync();
        return orders;
    }

}