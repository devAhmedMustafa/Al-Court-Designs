using OrdrMate.Models;

namespace OrdrMate.Repositories;

public interface IItemRepo
{
    Task<Item?> AddItem(Item item);
    Task<Item?> GetItem(string id);
    Task<IEnumerable<Item>> GetItems();
}