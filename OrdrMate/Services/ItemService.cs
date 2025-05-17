namespace OrdrMate.DTOs;

using OrdrMate.DTOs;
using OrdrMate.Models;
using OrdrMate.Repositories;

public class ItemService(IItemRepo itemRepo)
{
    private readonly IItemRepo _itemRepo = itemRepo;

    public async Task<ItemDto?> AddItem(AddItemDto item)
    {
        try
        {
            var newItem = new Item
            {
                Name = item.Name,
                Description = item.Description,
                ImageUrl = item.ImageUrl,
                Price = item.Price,
                CategoryName = item.Category,
                RestaurantId = item.RestaurantId
            };

            var addedItem = await _itemRepo.AddItem(newItem);

            if (addedItem == null)
            {
                throw new Exception("Failed to add item");
            }

            return new ItemDto
                {
                    Id = addedItem.Id,
                    Name = addedItem.Name,
                    Description = addedItem.Description,
                    ImageUrl = addedItem.ImageUrl,
                    Price = addedItem.Price,
                    Category = addedItem.CategoryName
                };
            
        } catch (Exception ex)
        {
            throw new Exception($"Error adding item: {ex.Message}");
        }
    }

    public async Task<Item?> GetItem(string id)
    {
        return await _itemRepo.GetItem(id);
    }

    public async Task<IEnumerable<Item>> GetItems()
    {
        return await _itemRepo.GetItems();
    }
}