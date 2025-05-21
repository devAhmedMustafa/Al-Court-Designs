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
    public DbSet<Branch> Branch => Set<Branch>();
    public DbSet<BranchRequest> BranchRequest => Set<BranchRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Manager

        modelBuilder.Entity<Manager>().HasKey(m => m.Id);
        modelBuilder.Entity<Manager>().HasIndex(m => m.Username).IsUnique();

        // Restaurant

        modelBuilder.Entity<Restaurant>().HasKey(r => r.Id);
        modelBuilder.Entity<Restaurant>().HasIndex(r => r.Name).IsUnique();
        modelBuilder.Entity<Restaurant>()
            .HasOne(r => r.Manager)
            .WithMany()
            .HasForeignKey(r => r.ManagerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Category

        modelBuilder.Entity<Category>().HasIndex(c => new { c.Name, c.RestaurantId }).IsUnique();
        modelBuilder.Entity<Category>().HasKey(c => new { c.Name, c.RestaurantId });
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Restaurant)
            .WithMany(r => r.Categories)
            .HasForeignKey(c => c.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Item

        modelBuilder.Entity<Item>().HasKey(i => i.Id);
        modelBuilder.Entity<Item>().HasIndex(i => new { i.Name, i.CategoryName, i.RestaurantId }).IsUnique();
        modelBuilder.Entity<Item>()
            .HasOne(i => i.Category)
            .WithMany(c => c.Items)
            .HasForeignKey(i => new { i.CategoryName, i.RestaurantId });

        modelBuilder.Entity<Item>()
            .HasOne(i => i.Restaurant)
            .WithMany(r => r.Items)
            .HasForeignKey(i => i.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Branch
        
        modelBuilder.Entity<Branch>().HasKey(b => b.Id);
        modelBuilder.Entity<Branch>().HasIndex(b => b.Phone).IsUnique();
        modelBuilder.Entity<Branch>().HasIndex(b => new { b.Lantitude, b.Longitude, b.RestaurantId }).IsUnique();
        modelBuilder.Entity<Branch>()
            .HasOne(b => b.Restaurant)
            .WithMany(r => r.Branches)
            .HasForeignKey(b => b.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Branch>()
            .HasOne(b => b.BranchManager)
            .WithMany()
            .HasForeignKey(b => b.BranchManagerId)
            .OnDelete(DeleteBehavior.Cascade);

        // BranchRequest

        modelBuilder.Entity<BranchRequest>().HasKey(br => br.Id);
        modelBuilder.Entity<BranchRequest>().HasIndex(br => new { br.Lantitude, br.Longitude, br.RestaurantId }).IsUnique();
        modelBuilder.Entity<BranchRequest>()
            .HasOne(br => br.Restaurant)
            .WithMany()
            .HasForeignKey(br => br.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

    }


}