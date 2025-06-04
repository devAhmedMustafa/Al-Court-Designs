namespace OrdrMate.Repositories;

using OrdrMate.Enums;
using OrdrMate.Models;

public interface IOrderRepo
{
    Task<Order> CreateOrder(Order order);
    Task<Takeaway> CreateTakeawayOrder(Takeaway takeaway);
    Task<OrderItem> CreateOrderItem(OrderItem orderItem);
    Task<Order?> GetOrderById(string orderId);
    Task<Order> GetDetailedOrderById(string orderId);
    Task<Takeaway?> GetTakeawayById(string orderId);
    Task<Indoor?> GetDineInById(string orderId);
    Task<IEnumerable<Takeaway>> GetTakeawaysByCustomerId(string customerId);
    Task<IEnumerable<Indoor>> GetIndoorsByCustomerId(string customerId);
    Task<Order?> SetOrderPaidStatus(string orderId, bool isPaid);
    Task<Order?> SetOrderStatus(string orderId, OrderStatus status);
    Task<IEnumerable<Order>> GetReadyOrdersByBranchId(string branchId);
    Task<IEnumerable<Order>> GetUnpaidOrdersByBranchId(string branchId);
}