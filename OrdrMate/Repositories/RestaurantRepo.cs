using Microsoft.EntityFrameworkCore;
using OrdrMate.Data;
using OrdrMate.Models;

namespace OrdrMate.Repositories;

public class RestaurantRepo(OrdrMateDbContext c) : IRestaurantRepo {

    private readonly OrdrMateDbContext _db = c;
    public async Task<Restaurant> CreateRestaurant(Restaurant restaurant){
        _db.Restaurant.Add(restaurant);
        await _db.SaveChangesAsync();
        return restaurant;
    }

    public async Task<Restaurant?> GetRestaurantById(string id){
        return await _db.Restaurant.FirstOrDefaultAsync(r=> r.Id == id);
    }
}