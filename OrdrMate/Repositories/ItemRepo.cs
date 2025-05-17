using Microsoft.EntityFrameworkCore;
using OrdrMate.Data;
using OrdrMate.Models;
using OrdrMate.Repositories;

public class ItemRepo(OrdrMateDbContext context) : IItemRepo
{
    private readonly OrdrMateDbContext _context = context;

    public async Task<Item?> AddItem(Item item)
    {
        // Create a category if it doesn't exist
        var category = await _context.Category
            .FirstOrDefaultAsync(c => c.Name == item.CategoryName && c.RestaurantId == item.RestaurantId);

        if (category == null)
        {
            category = new Category
            {
                Name = item.CategoryName,
                RestaurantId = item.RestaurantId
            };
            await _context.Category.AddAsync(category);
        }
        
        await _context.Item.AddAsync(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<Item?> GetItem(string id)
    {
        return await _context.Item.FindAsync(id);
    }

    public async Task<IEnumerable<Item>> GetItems()
    {
        return await _context.Item.ToListAsync();
    }
}