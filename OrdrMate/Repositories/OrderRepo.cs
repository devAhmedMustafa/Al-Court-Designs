namespace OrdrMate.Repositories;

using OrdrMate.Models;
using OrdrMate.Data;

public class OrderRepo : IOrderRepo
{
    private readonly OrdrMateDbContext _db;

    public OrderRepo(OrdrMateDbContext context)
    {
        _db = context;
    }

    public async Task<Order> CreateOrder(Order order)
    {
        var savedOrder = _db.Order.Add(order);
        await _db.SaveChangesAsync();
        return savedOrder.Entity;
    }

    public async Task<Takeaway> CreateTakeawayOrder(Takeaway takeaway)
    {
        _db.Takeaway.Add(takeaway);
        await _db.SaveChangesAsync();
        return takeaway;
    }

    public async Task<OrderItem> CreateOrderItem(OrderItem orderItem)
    {
        _db.OrderItem.Add(orderItem);
        await _db.SaveChangesAsync();
        return orderItem;
    }
}