using Microsoft.EntityFrameworkCore;
using OrdrMate.DTOs;
using OrdrMate.Middlewares;
using OrdrMate.Models;
using OrdrMate.Repositories;

namespace OrdrMate.Services;

public class ManagerService(IManagerRepo r, IConfiguration c) {
    private readonly IManagerRepo _repo = r;
    private readonly IConfiguration _config = c;

    public async Task<IEnumerable<ManagerDTO>> GetAllManagers(){
        var managers = await _repo.GetAll();
        return managers.Select(m => new ManagerDTO{
            Id = m.Id,
            Username = m.Username,
            Role = m.Role
        });
    }

    public async Task<ManagerDTO> CreateManager(CreateManagerDTO dto){

        try {
            var manager = new Manager {
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            var createdManager = await _repo.CreateManager(manager);

            var managerDto = new ManagerDTO {
                Id = createdManager.Id,
                Username = createdManager.Username,
                Role = createdManager.Role
            };

            return managerDto;
        }
        catch(DbUpdateException ex){
            if (ex.InnerException!.Message.Contains("duplicate"))
                throw new Exception("Username already exists");
            throw;
        }
    }

    public async Task<string> AuthenticateManager(LoginDTO data) {
        var manager = await _repo.GetManagerByUsername(data.Username)
            ?? throw new Exception("Invalid Credentials");

        if (!BCrypt.Net.BCrypt.Verify(data.Password, manager.Password))
            throw new Exception("Invalid Credentials");

        var jwtService = new JWTService(_config);

        return jwtService.GenerateJWT(data.Username);
    }
}