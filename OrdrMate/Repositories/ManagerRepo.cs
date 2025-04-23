using Microsoft.EntityFrameworkCore;
using OrdrMate.Data;
using OrdrMate.Models;

namespace OrdrMate.Repositories;

public class ManagerRepo(OrdrMateDbContext c) : IManagerRepo {
    private readonly OrdrMateDbContext _context = c;

    public async Task<IEnumerable<Manager>> GetAll(){
        return await _context.Manager.ToListAsync();
    }

    public async Task<Manager> CreateManager(Manager manager){
        _context.Manager.Add(manager);
        await _context.SaveChangesAsync();
        return manager;
    }
}