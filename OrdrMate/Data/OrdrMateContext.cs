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
    public DbSet<Table> Table => Set<Table>();
    public DbSet<Kitchen> Kitchen => Set<Kitchen>();
    public DbSet<KitchenPower> KitchenPower => Set<KitchenPower>();

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

        modelBuilder.Entity<Item>()
            .HasOne(i => i.Kitchen)
            .WithMany()
            .HasForeignKey(i => i.KitchenId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

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
            .WithMany(r => r.BranchRequests)
            .HasForeignKey(br => br.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);


        // Table

        modelBuilder.Entity<Table>().HasKey(t => new { t.TableNumber, t.BranchId });
        modelBuilder.Entity<Table>().HasIndex(t => new { t.TableNumber, t.BranchId }).IsUnique();
        modelBuilder.Entity<Table>()
            .HasOne(t => t.Branch)
            .WithMany(b => b.Tables)
            .HasForeignKey(t => t.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Kitchen
        
        modelBuilder.Entity<Kitchen>().HasKey(k => k.Id);
        modelBuilder.Entity<Kitchen>().HasIndex(k => new { k.Name, k.RestaurantId }).IsUnique();
        modelBuilder.Entity<Kitchen>()
            .HasOne(k => k.Restaurant)
            .WithMany(r => r.Kitchens)
            .HasForeignKey(k => k.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // KitchenPower

        modelBuilder.Entity<KitchenPower>().HasKey(kp => new { kp.BranchId, kp.KitchenId });
        modelBuilder.Entity<KitchenPower>()
            .HasIndex(kp => new { kp.BranchId, kp.KitchenId })
            .IsUnique();
        modelBuilder.Entity<KitchenPower>()
            .HasOne(kp => kp.Branch)
            .WithMany()
            .HasForeignKey(kp => kp.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

    }


}