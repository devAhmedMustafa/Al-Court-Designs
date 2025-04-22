using Microsoft.EntityFrameworkCore;
using OrdrMate.Models;

namespace OrdrMate.Data;

public class OrdrMateContext(DbContextOptions<OrdrMateContext> options) 
    : DbContext (options)
{
    DbSet<Manager> Managers => Set<Manager>();
}