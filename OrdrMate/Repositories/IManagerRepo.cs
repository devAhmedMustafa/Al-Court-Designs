namespace OrdrMate.Repositories;

using OrdrMate.Models;

public interface IManagerRepo {

    Task<IEnumerable<Manager>> GetAll();
    Task<Manager?> GetManagerByUsername(string username);
    Task<Manager> CreateManager(Manager manager);

}