namespace OrdrMate.Repositories;

using OrdrMate.Models;

public interface ITableRepo
{
    Task<IEnumerable<Table>> GetAllTablesOfBranch(string branchId);
    Task<Table> CreateTable(Table table);
    Task<Table> UpdateTable(string branchId, int tableNum, Table table);
    Task<bool> DeleteTable(string branchId, int tableNum);
}