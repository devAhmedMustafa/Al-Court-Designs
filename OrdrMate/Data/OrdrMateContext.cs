using Microsoft.EntityFrameworkCore;
using OrdrMate.Models;

namespace OrdrMate.Data;

public class OrdrMateDbContext(DbContextOptions<OrdrMateDbContext> options) 
    : DbContext (options)
{
    public DbSet<Manager> Manager => Set<Manager>();
    public DbSet<Restaurant> Restaurant => Set<Restaurant>();
    public DbSet<Item> Item => Set<Item>();
    public DbSet<Category> Category => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Manager>()
        .HasIndex(m=> m.Username).IsUnique();

        modelBuilder.Entity<Restaurant>()
        .HasIndex(r => r.Name).IsUnique();
        
        // Make sure that the name and restaurantid of the category are combined unique
        modelBuilder.Entity<Category>()
            .HasIndex(c => new { c.Name, c.RestaurantId }).IsUnique();

        modelBuilder.Entity<Category>().HasNoKey();

    }


}