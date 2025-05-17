using OrdrMate.Models;

namespace OrdrMate.Repositories;

public interface IRestaurantRepo
{
    Task<Restaurant> CreateRestaurant(Restaurant restaurant);
    Task<Restaurant?> GetRestaurantById(string id);
    Task<Boolean> HasAccessToRestaurant(string managerId, string restaurantId);
}