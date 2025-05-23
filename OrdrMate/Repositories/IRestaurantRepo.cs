using OrdrMate.Models;

namespace OrdrMate.Repositories;

public interface IRestaurantRepo
{
    Task<Restaurant> CreateRestaurant(Restaurant restaurant);
    Task<Restaurant?> GetRestaurantById(string id);
    Task<bool> HasAccessToRestaurant(string managerId, string restaurantId);

    Task<Restaurant?> GetRestaurantByManagerId(string managerId);
    Task<IEnumerable<Restaurant>> GetAllRestaurants();
}