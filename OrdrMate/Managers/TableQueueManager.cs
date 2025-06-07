using System.Diagnostics;
using OrdrMate.Models;

namespace OrdrMate.Managers;

public class TableQueueManager
{
    private readonly Dictionary<int, ReservationQueue> _tableQueues;
    public TableQueueManager(Branch branch)
    {
        _tableQueues = [];

        if (branch.Tables == null)
        {
            Console.WriteLine("No tables found for the branch.");
            return;
        }

        foreach (var table in branch.Tables)
        {

            _tableQueues[table.TableNumber] = new ReservationQueue(table.Seats);
            Console.WriteLine($"Initialized queue for table {table.TableNumber} with {table.Seats} seats.");
        }
    }

    public int ReserveTable(int seats, TableReservation reservation)
    {

        int minReserved = int.MaxValue;
        int bestTable = -1;

        foreach (var kvp in _tableQueues)
        {
            if (kvp.Value.Seats >= seats)
            {
                if (kvp.Value.Count < minReserved)
                {
                    minReserved = kvp.Value.Count;
                    bestTable = kvp.Key;
                }
            }
        }

        _tableQueues[bestTable].EnqueueReservation(reservation);

        return bestTable;
    }

    public TableReservation? DequeueReservation(int tableNumber)
    {
        if (_tableQueues.TryGetValue(tableNumber, out var queue))
        {
            return queue.DequeueReservation();
        }
        else
        {
            Console.WriteLine($"No reservation queue found for table {tableNumber}.");
            return null;
        }
    }

    public int GetOrderPosition(int tableNumber, string orderId)
    {
        if (_tableQueues.TryGetValue(tableNumber, out var queue))
        {
            return queue.GetOrderPosition(orderId);
        }
        else
        {
            Console.WriteLine($"No reservation queue found for table {tableNumber}.");
            return 0;
        }
    }

    public int GetMinimumWaitingTime(int seats)
    {
        int minWaitingTime = int.MaxValue;

        foreach (var queue in _tableQueues.Values)
        {
            if (queue.Seats >= seats && queue.Count < minWaitingTime)
            {
                minWaitingTime = queue.Count;
            }
        }

        return minWaitingTime == int.MaxValue ? 0 : minWaitingTime;
    }

    public TableReservation? PeekReservation(int tableNumber)
    {
        if (_tableQueues.TryGetValue(tableNumber, out var queue))
        {
            return queue.Peek();
        }
        else
        {
            Console.WriteLine($"No reservation queue found for table {tableNumber}.");
            return null;
        }
    }

}