using Microsoft.EntityFrameworkCore;
using OrdrMate.Data;
using OrdrMate.Models;

namespace OrdrMate.Repositories;

public class ManagerRepo(OrdrMateDbContext c) : IManagerRepo {
    private readonly OrdrMateDbContext _db = c;

    public async Task<IEnumerable<Manager>> GetAll(){
        return await _db.Manager.ToListAsync();
    }
    
    public async Task<Manager?> GetManagerById(string id)
    {
        return await _db.Manager.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Manager> CreateManager(Manager manager)
    {
        _db.Manager.Add(manager);
        await _db.SaveChangesAsync();
        return manager;
    }

    public async Task<Manager?> GetManagerByUsername(string username){
        return await _db.Manager.FirstOrDefaultAsync(m => m.Username == username);
    }
}