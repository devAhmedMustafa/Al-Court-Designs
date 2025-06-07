using System.Diagnostics;
using OrdrMate.Events;
using OrdrMate.Models;
using OrdrMate.Repositories;

namespace OrdrMate.Managers;

public class TableManager
{

    private readonly static Dictionary<string, TableQueueManager> _branchQueues = [];
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IBranchRepo _branchRepo;
    private static bool _initialized = false;

    public TableManager(
        IBranchRepo branchRepo,
        IServiceScopeFactory scopeFactory
    )
    {
        _scopeFactory = scopeFactory;
        _branchRepo = branchRepo;

        Console.WriteLine($"TableManager {_initialized} initialized");

        if (_initialized) return;

        Init();

        BranchEvents.BranchCreated += AddBranchQueue;

        _initialized = true;
    }

    private void Init()
    {
        var restaurants = _branchRepo.GetAllBranches().Result;
        foreach (var restaurant in restaurants)
        {
            AddBranchQueue(restaurant);
        }
    }

    public void AddBranchQueue(Branch branch)
    {
        if (!_branchQueues.ContainsKey(branch.Id))
        {
            _branchQueues[branch.Id] = new TableQueueManager(branch);
        }
    }

    public async Task<int> ReserveTable(int seats, TableReservation reservation)
    {
        if (_branchQueues.TryGetValue(reservation.BranchId, out var queueManager))
        {
            var tableNumber = queueManager.ReserveTable(seats, reservation);

            var peekReservation = queueManager.PeekReservation(tableNumber);

            if (peekReservation == null)
            {
                Console.WriteLine($"No reservation found for table {tableNumber} in branch {reservation.BranchId}.");
                return -1;
            }

            var tableRepo = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITableRepo>();

            var order = await tableRepo.GetTableOrderByReservationId(peekReservation.ReservationId);
            if (order == null || order.OrderItems == null)
            {
                Console.WriteLine($"No order found for reservation {peekReservation.ReservationId} in branch {peekReservation.BranchId}.");
                return -1;
            }

            await tableRepo.UpdateTableReservationStatus(peekReservation.ReservationId, "Seated");
            OrderEvents.OnOrderPlaced(reservation.BranchId, [.. order.OrderItems]);

            return tableNumber;
        }
        else
        {
            throw new Exception($"No queue manager found for branch {reservation.BranchId}");
        }
    }

    public async Task DequeueReservation(string branchId, int tableNumber)
    {
        if (_branchQueues.TryGetValue(branchId, out var queueManager))
        {
            var reservation = queueManager.DequeueReservation(tableNumber);
            if (reservation == null)
            {
                Console.WriteLine($"No reservation found for table {tableNumber} in branch {branchId}.");
                return;
            }

            var tableRepo = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITableRepo>();

            var order = await tableRepo.GetTableOrderByReservationId(reservation.ReservationId);
            if (order == null || order.OrderItems == null)
            {
                Console.WriteLine($"No order found for reservation {reservation.ReservationId} in branch {branchId}.");
                return;
            }

            await tableRepo.UpdateTableReservationStatus(reservation.ReservationId, "Left");
        }
        else
        {
            Console.WriteLine($"No reservation queue found for branch {branchId} and table {tableNumber}.");
        }
    }

    public async Task<int> GetOrderPosition(string reservationId)
    {
        var tableRepo = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ITableRepo>();
        var reservation = await tableRepo.GetTableReservationByOrderId(reservationId);
        if (reservation == null)
        {
            Debug.WriteLine($"No reservation found for order {reservationId}.");
            return -1;
        }

        if (_branchQueues.TryGetValue(reservation.BranchId, out var queueManager))
        {
            return queueManager.GetOrderPosition(reservation.TableNumber, reservation.OrderId) + 1;
        }
        else
        {
            Debug.WriteLine($"No reservation queue found for branch {reservation.BranchId}.");
            return -1;
        }
    }
    
    public Task<int> GetMinimumWaitingTime(string branchId, int seats)
    {
        if (_branchQueues.TryGetValue(branchId, out var queueManager))
        {
            return Task.FromResult(queueManager.GetMinimumWaitingTime(seats));
        }
        else
        {
            Debug.WriteLine($"No reservation queue found for branch {branchId}.");
            return Task.FromResult(-1);
        }
    }

}