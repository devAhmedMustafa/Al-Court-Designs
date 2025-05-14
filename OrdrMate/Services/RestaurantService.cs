using OrdrMate.DTOs;
using OrdrMate.Models;
using OrdrMate.Repositories;

namespace OrdrMate.Services;

public class RestaurantService(IRestaurantRepo r, IManagerRepo m) {
    private readonly IRestaurantRepo _repo = r;
    private readonly IManagerRepo _managerRepo = m;

    public async Task<RestaurantDTO> CreateRestaurant(CreateRestaurantDto dto){
        try {

            var manager = await _managerRepo.GetManagerByUsername(dto.ManagerUsername);

            if (manager == null){
                throw new Exception("No manager with "+dto.ManagerUsername+" username");
            }

            var restaurant = new Restaurant{
                Name=dto.Name,
                Email=dto.Email,
                Phone=dto.Phone,
                ManagerId=manager.Id
            };

            var createdRestaurant = await _repo.CreateRestaurant(restaurant);

            var responseDto = new RestaurantDTO{
                Id=createdRestaurant.Id,
                Name=createdRestaurant.Name,
            };

            if (createdRestaurant.Email != null) responseDto.Email = createdRestaurant.Email;
            if (createdRestaurant.Phone != null) responseDto.Phone = createdRestaurant.Phone;

            return responseDto;

        } catch(Exception e){
            throw;
        }
    }
}