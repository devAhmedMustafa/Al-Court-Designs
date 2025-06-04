using OrdrMate.DTOs.Order;
using OrdrMate.Models;

namespace OrdrMate.Utils;

public static class CalculateOrderTotal
{
    public static decimal GetTotalPrice(OrderItemDto[] orderItems)
    {
        decimal total = 0;
        foreach (var orderItem in orderItems)
        {
            total += orderItem.Price * orderItem.Quantity;
        }
        return total;
    }
}