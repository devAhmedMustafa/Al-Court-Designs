namespace OrdrMate.Services;

using OrdrMate.DTOs.Order;
using OrdrMate.DTOs.Table;
using OrdrMate.Managers;
using OrdrMate.Models;
using OrdrMate.Repositories;

public class TableService
{
    private readonly ITableRepo _tableRepo;
    private readonly TableManager _tableManager;
    public TableService(ITableRepo tableRepo, TableManager tableManager)
    {
        _tableRepo = tableRepo;
        _tableManager = tableManager;
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
        var table = new Table
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

    public async Task<TableReservationDto> ReserveTable(OrderDto order, int seats)
    {
        var reservation = new TableReservation
        {
            BranchId = order.BranchId,
            CustomerId = order.CustomerId,
            OrderId = order.OrderId,
            ReservationTime = DateTime.UtcNow,
        };

        var tableNumber = _tableManager.ReserveTable(seats, reservation);

        reservation.TableNumber = tableNumber;

        var createdReservation = await _tableRepo.CreateTableReservation(reservation);

        return new TableReservationDto
        {
            TableNumber = createdReservation!.TableNumber,
        };
    }

    public async Task<int> GetFreeTableCount(string branchId)
    {
        var reservations = await _tableRepo.GetTableReservationsByBranchId(branchId);
        var reservedTables = reservations
            .Where(r => r.ReservationStatus == "Queued")
            .Select(r => r.TableNumber)
            .Distinct()
            .ToHashSet();

        var allTables = await _tableRepo.GetAllTablesOfBranch(branchId);
        return allTables.Count(t => !reservedTables.Contains(t.TableNumber));
    }

    public async Task<IEnumerable<TableReservationDto>> GetCustomerTableReservation(string customerId)
    {
        var reservations = await _tableRepo.GetTableReservationsByCustomerId(customerId);
        return reservations.Select(r => new TableReservationDto
        {
            TableNumber = r.TableNumber,
            Order = r.Order
        });
    }
    
    public async Task<TableReservationDto?> GetTableReservationByOrderId(string orderId)
    {
        var reservation = await _tableRepo.GetTableReservationByOrderId(orderId);
        if (reservation == null) return null;

        return new TableReservationDto
        {
            TableNumber = reservation.TableNumber,
        };
    }
}