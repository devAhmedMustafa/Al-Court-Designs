namespace OrdrMate.Repositories;

using OrdrMate.Models;
using OrdrMate.Data;
using Microsoft.EntityFrameworkCore;

public class TableRepo : ITableRepo
{
    private readonly OrdrMateDbContext _context;

    public TableRepo(OrdrMateDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Table>> GetAllTablesOfBranch(string branchId)
    {
        return await _context.Table
            .Where(t => t.BranchId == branchId)
            .ToListAsync();
    }

    public async Task<Table> CreateTable(Table table)
    {

        var existingTable = await _context.Table
            .FirstOrDefaultAsync(t => t.TableNumber == table.TableNumber && t.BranchId == table.BranchId);

        if (existingTable != null)
        {
            throw new InvalidOperationException($"Table with number {table.TableNumber} already exists in branch {table.BranchId}.");
        }

        var branch = await _context.Branch
            .FirstOrDefaultAsync(b => b.Id == table.BranchId);

        if (branch == null)
        {
            throw new InvalidOperationException($"Branch with id {table.BranchId} does not exist.");
        }

        _context.Table.Add(table);
        await _context.SaveChangesAsync();
        return table;
    }

    public async Task<Table> UpdateTable(string branchId, int tableNum, Table table)
    {
        var existingTable = await _context.Table
            .FirstOrDefaultAsync(t => t.TableNumber == tableNum && t.BranchId == branchId);

        if (existingTable == null)
        {
            throw new InvalidOperationException($"Table with number {table.TableNumber} does not exist in branch {branchId}.");
        }

        existingTable.Seats = table.Seats;

        await _context.SaveChangesAsync();
        return existingTable;
    }
    
    public async Task<bool> DeleteTable(string branchId, int tableNum)
    {
        var table = await _context.Table
            .FirstOrDefaultAsync(t => t.TableNumber == tableNum && t.BranchId == branchId);

        if (table == null)
        {
            throw new InvalidOperationException($"Table with number {tableNum} does not exist in branch {branchId}.");
        }

        _context.Table.Remove(table);
        await _context.SaveChangesAsync();
        return true;
    }
}