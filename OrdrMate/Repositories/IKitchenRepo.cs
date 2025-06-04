using OrdrMate.Models;

namespace OrdrMate.Repositories;

public interface IKitchenRepo
{
    public Task<Kitchen?> GetKitchenById(string kitchenId);
    public Task<List<Kitchen>> GetKitchensByRestaurantId(string restaurantId);
    public Task<Kitchen?> CreateKitchen(Kitchen kitchen);
    public Task<Kitchen?> UpdateKitchen(string id, Kitchen kitchen);
    public Task<bool> DeleteKitchen(string kitchenId);
    public Task<KitchenPower?> GetKitchenPowerByBranchIdAndKitchenId(string branchId, string kitchenId);
    public Task<KitchenPower?> CreateKitchenPower(KitchenPower kitchenPower);
    public Task<KitchenPower?> UpdateKitchenPower(string branchId, string kitchenId, KitchenPower kitchenPower);
}