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
        return await _context.Branch.FindAsync(id) ?? throw new KeyNotFoundException($"Branch with id {id} not found.");
    }

    public async Task<IEnumerable<Branch>> GetAllBranches()
    {
        return await _context.Branch.ToListAsync();
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


}