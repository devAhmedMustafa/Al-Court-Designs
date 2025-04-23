using OrdrMate.DTOs;
using OrdrMate.Models;
using OrdrMate.Repositories;

namespace OrdrMate.Services;

public class ManagerService(IManagerRepo r) {
    private readonly IManagerRepo _repo = r;

    public async Task<IEnumerable<ManagerDTO>> GetAllManagers(){
        var managers = await _repo.GetAll();
        return managers.Select(m => new ManagerDTO{
            Id = m.Id,
            Username = m.Username,
            Password = m.Password
        });
    }

    public async Task<Manager> CreateManager(CreateManagerDTO dto){
        var manager = new Manager {
            Username = dto.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        return await _repo.CreateManager(manager);
    }
}