namespace OrdrMate.Repositories;

using OrdrMate.Models;
using OrdrMate.Data;
using Microsoft.EntityFrameworkCore;
using OrdrMate.Enums;

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
        var saved = _db.OrderItem.Add(orderItem);
        await _db.SaveChangesAsync();
        await _db.Entry(saved.Entity).Reference(oi => oi.Item).LoadAsync();
        return saved.Entity;
    }

    public async Task<Order> GetDetailedOrderById(string orderId)
    {
        return await _db.Order
            .Include(o => o.OrderItems!).ThenInclude(oi => oi.Item)
            .Include(o => o.Branch).ThenInclude(b => b!.Restaurant)
            .Include(o => o.Customer)
            .Include(o => o.Payment)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == orderId)
            ?? throw new KeyNotFoundException($"Order with id {orderId} not found.");
    }

    public async Task<Takeaway?> GetTakeawayById(string orderId)
    {
        return await _db.Takeaway.FirstOrDefaultAsync(t => t.OrderId == orderId);
    }

    public async Task<Indoor?> GetDineInById(string orderId)
    {
        return await _db.Indoor.FirstOrDefaultAsync(i => i.OrderId == orderId);
    }

    public async Task<IEnumerable<Takeaway>> GetTakeawaysByCustomerId(string customerId)
    {
        return await _db.Takeaway
            .Include(t => t.Order).ThenInclude(o => o.Branch).ThenInclude(b => b!.Restaurant)
            .Include(t => t.Order).ThenInclude(o => o.Customer)
            .Include(t => t.Order).ThenInclude(o => o.Payment)
            .Where(t => t.Order.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Indoor>> GetIndoorsByCustomerId(string customerId)
    {
        return await _db.Indoor
            .Include(i => i.Order).ThenInclude(o => o.Branch).ThenInclude(b => b!.Restaurant)
            .Include(i => i.Order).ThenInclude(o => o.Customer)
            .Include(i => i.Order).ThenInclude(o => o.Payment)
            .Where(i => i.Order.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<Order?> GetOrderById(string orderId)
    {
        return await _db.Order
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<Order?> SetOrderPaidStatus(string orderId, bool isPaid)
    {
        return await _db.Order
            .Where(o => o.Id == orderId)
            .ExecuteUpdateAsync(o => o.SetProperty(p => p.IsPaid, isPaid))
            .ContinueWith(_ => _db.Order.FirstOrDefaultAsync(o => o.Id == orderId))
            .Unwrap();
    }

    public async Task<Order?> SetOrderStatus(string orderId, OrderStatus status)
    {
        var order = await _db.Order
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            throw new KeyNotFoundException($"Order with id {orderId} not found.");
        }

        order.Status = status;
        _db.Order.Update(order);
        await _db.SaveChangesAsync();
        return order;
    }

    public async Task<IEnumerable<Order>> GetReadyOrdersByBranchId(string branchId)
    {
        return await _db.Order
            .Where(o => o.BranchId == branchId && o.Status == OrderStatus.Ready)
            .Include(o => o.Payment)
            .Include(o => o.Customer)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetUnpaidOrdersByBranchId(string branchId)
    {
        return await _db.Order
            .Where(o => o.BranchId == branchId && o.IsPaid == false)
            .Include(o => o.Payment)
            .Include(o => o.Customer)
            .ToListAsync();
    }
}