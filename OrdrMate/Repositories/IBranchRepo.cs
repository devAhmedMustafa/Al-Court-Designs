namespace OrdrMate.Repositories;

using OrdrMate.Models;

public interface IBranchRepo
{
    Task<Branch> GetBranchById(string id);
    Task<IEnumerable<Branch>> GetAllBranches();
    Task<Branch> CreateBranch(Branch branch);
    Task<bool> DeleteBranch(string id);
    Task<bool> UpdateBranch(Branch branch);
    Task<bool> BranchExists(string id);
}