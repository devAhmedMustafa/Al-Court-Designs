using OrdrMate.Data;
using OrdrMate.Models;

namespace OrdrMate.Repositories;

public class ItemRepo(OrdrMateDbContext db) : IItemRepo
{
    private readonly OrdrMateDbContext _db = db;
    
    async Task<Item?> IItemRepo.AddItem(Item item)
    {
        
    }
}