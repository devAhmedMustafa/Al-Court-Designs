using OrdrMate.Models;

namespace OrdrMate.Repositories;

public interface IItemRepo
{
    Task<Item?> AddItem(Item item);
    Task<Item?> GetItem(string id);
    Task<IEnumerable<Item>> GetItems();
    Task<IEnumerable<Item>> GetItemsByRestaurantId(string restaurantId);
    Task<Item?> UpdateItem(string id, Item item);
    Task<bool> DeleteItem(string id);
}