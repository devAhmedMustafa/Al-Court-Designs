using OrderMate.Enums;
using OrdrMate.DTOs.Order;
using OrdrMate.Models;
using OrdrMate.Utils;
using OrdrMate.Repositories;

namespace OrdrMate.Services;

public class OrderService
{
    private readonly PaymentService _paymentService;
    private readonly IOrderRepo _orderRepo;

    public OrderService(PaymentService paymentService, IOrderRepo orderRepo)
    {
        _paymentService = paymentService;
        _orderRepo = orderRepo;
    }

    public async Task<OrderDto> PlaceOrder(PlaceOrderDto placeOrderDto)
    {
        var order = new Order
        {
            BranchId = placeOrderDto.BranchId,
            CustomerId = placeOrderDto.CustomerId,
            TotalAmount = CalculateOrderTotal.GetTotalPrice(placeOrderDto.Items),
        };

        order = await _orderRepo.CreateOrder(order);

        var dto = new OrderDto
        {
            OrderId = order.Id,
            RestaurantName = order.Branch?.Restaurant.Name ?? "Unknown Restaurant",
            Customer = order.Customer?.Username ?? "Unknown Customer",
            OrderType = placeOrderDto.OrderType.ToString(),
            PaymentMethod = placeOrderDto.PaymentMethod,
            OrderDate = DateTime.UtcNow,
            OrderStatus = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            BranchId = order.BranchId,
            OrderItems = placeOrderDto.Items
        };

        foreach (var orderitem in placeOrderDto.Items)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ItemId = orderitem.ItemId,
                Quantity = orderitem.Quantity,
                Price = orderitem.Price,
            };

            await _orderRepo.CreateOrderItem(orderItem);
        }

        switch (placeOrderDto.OrderType)
        {
            case OrderType.Takeaway:
                dto.OrderNumber = await PlaceTakeawayOrder(order);
                dto.IsPaid = false;
                break;
            default:
                throw new NotImplementedException($"Order type {placeOrderDto.OrderType} is not implemented yet.");
        }

        await ProcessPayment(order, placeOrderDto);

        return dto;
    }

    private async Task<int> PlaceTakeawayOrder(Order order)
    {
        var orderNum = DailyNumberGenerator.GetNextNumber();

        var takeaway = new Takeaway
        {
            OrderId = order.Id,
            OrderNumber = orderNum,
        };
        
        await _orderRepo.CreateTakeawayOrder(takeaway);

        return orderNum;
    }

    private async Task<PaymentDto> ProcessPayment(Order order, PlaceOrderDto placeOrderDto)
    {

        return placeOrderDto.PaymentMethod.ToLower() switch
        {
            "cash" => await _paymentService.AddCashPayment(order.Id, order.TotalAmount),
            _ => throw new NotImplementedException($"Payment method {placeOrderDto.PaymentMethod} is not implemented yet."),
        };
    }

}