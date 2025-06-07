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

    public async Task<TableReservation?> CreateTableReservation(TableReservation reservation)
    {
        var existingReservation = await _context.TableReservation
            .FirstOrDefaultAsync(r => r.TableNumber == reservation.TableNumber && r.BranchId == reservation.BranchId && r.ReservationTime == reservation.ReservationTime);

        if (existingReservation != null) throw new InvalidOperationException($"A reservation already exists for table {reservation.TableNumber} at branch {reservation.BranchId} at the specified time.");

        var branch = await _context.Branch
            .FirstOrDefaultAsync(b => b.Id == reservation.BranchId);

        if (branch == null) throw new InvalidOperationException($"Branch with id {reservation.BranchId} does not exist.");
        var customer = await _context.User
            .FirstOrDefaultAsync(u => u.Id == reservation.CustomerId);
        if (customer == null) throw new InvalidOperationException($"Customer with id {reservation.CustomerId} does not exist.");

        _context.TableReservation.Add(reservation);
        await _context.SaveChangesAsync();
        return reservation;
    }

    public async Task<IEnumerable<TableReservation>> GetTableReservationsByBranchId(string branchId)
    {
        return await _context.TableReservation
            .Include(r => r.Branch)
            .Include(r => r.Customer)
            .Include(r => r.Order).ThenInclude(o => o!.OrderItems)!.ThenInclude(oi => oi.Item)
            .Where(r => r.BranchId == branchId)
            .ToListAsync();
    }

    public async Task<TableReservation> UpdateTableReservationStatus(string reservationId, string status)
    {
        var existingReservation = await _context.TableReservation
            .FirstOrDefaultAsync(r => r.ReservationId == reservationId)
            ?? throw new InvalidOperationException($"Reservation with id {reservationId} does not exist.");

        existingReservation.ReservationStatus = status;

        await _context.SaveChangesAsync();
        return existingReservation;
    }

    public async Task<Order?> GetTableOrderByReservationId(string reservationId)
    {
        var reservation = await _context.TableReservation
            .Include(r => r.Order).ThenInclude(o => o!.OrderItems)!.ThenInclude(oi => oi.Item)
            .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

        if (reservation == null) return null;

        return reservation.Order;
    }

    public async Task<IEnumerable<TableReservation>> GetTableReservationsByCustomerId(string customerId)
    {
        return await _context.TableReservation
            .Include(r => r.Branch)
            .Include(r => r.Customer)
            .Include(r => r.Order)
            .Where(r => r.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<TableReservation?> GetTableReservationByOrderId(string orderId)
    {
        return await _context.TableReservation
            .Include(r => r.Branch)
            .Include(r => r.Customer)
            .Include(r => r.Order)
            .FirstOrDefaultAsync(r => r.OrderId == orderId);
    }
    
    public async Task<TableReservation?> GetTableReservationById(string reservationId)
    {
        return await _context.TableReservation
            .Include(r => r.Branch)
            .Include(r => r.Customer)
            .Include(r => r.Order)
            .FirstOrDefaultAsync(r => r.ReservationId == reservationId);
    }

}