using OrdrMate.Models;

namespace OrdrMate.Managers;

public class ReservationQueue
{
    private readonly Queue<TableReservation> _queue;
    private int _seats;
    public int Seats => _seats;
    public ReservationQueue(int seats)
    {
        _queue = new Queue<TableReservation>();
        _seats = seats;
    }

    public void EnqueueReservation(TableReservation reservation)
    {
        _queue.Enqueue(reservation);
    }

    public TableReservation? DequeueReservation()
    {
        if (_queue.Count == 0)
            return null;

        return _queue.Dequeue();
    }

    public int Count => _queue.Count;
    public bool IsEmpty => _queue.Count == 0;
    public TableReservation Peek()
    {
        if (_queue.Count == 0)
            throw new InvalidOperationException("Queue is empty.");

        return _queue.Peek();
    }

    public int GetOrderPosition(string orderId)
    {
        int position = 0;
        foreach (var reservation in _queue)
        {
            if (reservation.OrderId == orderId)
            {
                return position;
            }
            position++;
        }
        return -1;
    }
}