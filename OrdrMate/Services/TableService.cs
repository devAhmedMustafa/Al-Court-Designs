namespace OrdrMate.Services;

using OrdrMate.DTOs.Table;
using OrdrMate.Repositories;

public class TableService
{
    private readonly ITableRepo _tableRepo;
    public TableService(ITableRepo tableRepo)
    {
        _tableRepo = tableRepo;
    }

    public async Task<IEnumerable<TableDto>> GetAllTablesOfBranch(string branchId)
    {
        var tables = await _tableRepo.GetAllTablesOfBranch(branchId);
        return tables.Select(t => new TableDto
        {
            TableNumber = t.TableNumber,
            Seats = t.Seats,
            BranchId = t.BranchId
        });
    }

    public async Task<TableDto> CreateTable(AddTableDto tableDto)
    {
        var table = new Models.Table
        {
            TableNumber = tableDto.TableNumber,
            Seats = tableDto.Seats,
            BranchId = tableDto.BranchId
        };

        var createdTable = await _tableRepo.CreateTable(table);
        return new TableDto
        {
            TableNumber = createdTable.TableNumber,
            Seats = createdTable.Seats,
            BranchId = createdTable.BranchId
        };
    }

    public async Task<bool> DeleteTable(string branchId, int tableNum)
    {
        return await _tableRepo.DeleteTable(branchId, tableNum);
    }
}