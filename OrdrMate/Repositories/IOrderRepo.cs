namespace OrdrMate.Repositories;

using OrdrMate.Models;

public interface IOrderRepo
{
    Task<Order> CreateOrder(Order order);
    Task<Takeaway> CreateTakeawayOrder(Takeaway takeaway);
    Task<OrderItem> CreateOrderItem(OrderItem orderItem);
}