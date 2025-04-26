using Microsoft.EntityFrameworkCore;
using OrdrMate.Models;

namespace OrdrMate.Data;

public class OrdrMateDbContext(DbContextOptions<OrdrMateDbContext> options) 
    : DbContext (options)
{
    public DbSet<Manager> Manager => Set<Manager>();
    public DbSet<Restaurant> Restaurant => Set<Restaurant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Manager>()
        .HasIndex(m=> m.Username).IsUnique();

        modelBuilder.Entity<Restaurant>()
        .HasIndex(r => r.Name).IsUnique();

    }


}