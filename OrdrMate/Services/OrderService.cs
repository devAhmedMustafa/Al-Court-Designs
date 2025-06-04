using OrdrMate.Enums;
using OrdrMate.DTOs.Order;
using OrdrMate.Models;
using OrdrMate.Utils;
using OrdrMate.Repositories;
using OrdrMate.Events;

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
            RestaurantName = order.Branch?.Restaurant?.Name ?? "Unknown Restaurant",
            Customer = order.Customer?.Username ?? "Unknown Customer",
            OrderType = placeOrderDto.OrderType.ToString(),
            PaymentMethod = placeOrderDto.PaymentMethod,
            OrderDate = DateTime.UtcNow,
            OrderStatus = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            BranchId = order.BranchId,
        };

        List<OrderItem> orderItems = [];

        foreach (var orderitem in placeOrderDto.Items)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ItemId = orderitem.ItemId,
                Quantity = orderitem.Quantity,
                Price = orderitem.Price,
            };

            var savedOrderItem = await _orderRepo.CreateOrderItem(orderItem);

            if (savedOrderItem.Item == null)
            {
                Console.WriteLine($"Order item with ID {savedOrderItem.ItemId} not found. Skipping this item.");
                continue;
            }

            orderItems = [.. orderItems, savedOrderItem];
        }

        OrderEvents.OnOrderPlaced(order.BranchId, orderItems);

        switch (placeOrderDto.OrderType)
        {
            case OrderType.Takeaway:
                dto.OrderNumber = await PlaceTakeawayOrder(order);
                dto.IsPaid = false;
                order.OrderType = OrderType.Takeaway;
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
    public async Task<IEnumerable<OrderDto>> GetCustomerOrders(string customerId)
    {
        var takeaways = await _orderRepo.GetTakeawaysByCustomerId(customerId);
        var indoors = await _orderRepo.GetIndoorsByCustomerId(customerId);

        if (takeaways == null)
        {
            Console.WriteLine($"No takeaways orders found for customer with ID: {customerId}");
            takeaways = [];
        }

        if (indoors == null)
        {
            Console.WriteLine($"No dine-in orders found for customer with ID: {customerId}");
            indoors = [];
        }

        var takeawayDtos = takeaways.Select(t => new OrderDto
        {
            OrderId = t.Order.Id,
            RestaurantName = t.Order.Branch?.Restaurant?.Name ?? "Unknown Restaurant",
            Customer = t.Order.Customer?.Username ?? "Unknown Customer",
            OrderType = OrderType.Takeaway.ToString(),
            PaymentMethod = t.Order.Payment?.PaymentMethod ?? "Unknown",
            OrderDate = t.Order.OrderDate,
            OrderStatus = t.Order.Status.ToString(),
            TotalAmount = t.Order.TotalAmount,
            BranchId = t.Order.BranchId,
            OrderNumber = t.OrderNumber,
            IsPaid = t.Order.IsPaid,
        });

        var indoorDtos = indoors.Select(i => new OrderDto
        {
            OrderId = i.Order.Id,
            RestaurantName = i.Order.Branch?.Restaurant?.Name ?? "Unknown Restaurant",
            Customer = i.Order.Customer?.Username ?? "Unknown Customer",
            OrderType = OrderType.DineIn.ToString(),
            PaymentMethod = i.Order.Payment?.PaymentMethod ?? "Unknown",
            OrderDate = i.Order.OrderDate,
            OrderStatus = i.Order.Status.ToString(),
            TotalAmount = i.Order.TotalAmount,
            BranchId = i.Order.BranchId,
            TableNumber = i.TableNumber,
            IsPaid = i.Order.IsPaid
        });

        var orders = takeawayDtos.Concat(indoorDtos);

        return orders;

    }

    public async Task<OrderDto> GetOrderById(string orderId)
    {
        var order = await _orderRepo.GetOrderById(orderId);

        if (order == null) throw new KeyNotFoundException($"Order with id {orderId} not found.");

        return new OrderDto
        {
            OrderId = order.Id,
            RestaurantName = order.Branch?.Restaurant?.Name ?? "Unknown Restaurant",
            Customer = order.Customer?.Username ?? "Unknown Customer",
            OrderType = "",
            PaymentMethod = order.Payment?.PaymentMethod ?? "Unknown",
            OrderDate = order.OrderDate,
            OrderStatus = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            BranchId = order.BranchId,
            IsPaid = order.IsPaid,
        };
    }

    public async Task<OrderDto> GetOrderDetails(string orderId)
    {
        var order = await _orderRepo.GetDetailedOrderById(orderId);
        if (order == null) throw new KeyNotFoundException($"Order with id {orderId} not found.");

        var orderDto = new OrderDto
        {
            OrderId = order.Id,
            RestaurantName = order.Branch?.Restaurant?.Name ?? "Unknown Restaurant",
            Customer = order.Customer?.Username ?? "Unknown Customer",
            OrderType = "",
            PaymentMethod = order.Payment?.PaymentMethod ?? "Unknown",
            OrderDate = order.OrderDate,
            OrderStatus = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            BranchId = order.BranchId,
            IsPaid = order.IsPaid,
            OrderItems = order.OrderItems?.Select(oi => new OrderItemDto
            {
                ItemId = oi.ItemId,
                Item = new DTOs.Item.ItemDto
                {
                    Id = oi.Item?.Id ?? string.Empty,
                    Name = oi.Item?.Name ?? "Unknown Item",
                    Description = oi.Item?.Description ?? "No description available",
                    ImageUrl = oi.Item?.ImageUrl ?? string.Empty,
                    Price = oi.Item?.Price ?? 0,
                    Category = oi.Item?.CategoryName ?? "Uncategorized",
                    PreparationTime = oi.Item?.PreperationTime ?? 0,
                    KitchenName = oi.Item?.Kitchen?.Name ?? "Unknown Kitchen"
                },
                Quantity = oi.Quantity,
                Price = oi.Price,
            }).ToArray()

        };

        var takeaway = await _orderRepo.GetTakeawayById(orderId);

        if (takeaway != null)
        {
            orderDto.OrderType = OrderType.Takeaway.ToString();
            orderDto.OrderNumber = takeaway.OrderNumber;
            return orderDto;
        }

        var indoor = await _orderRepo.GetDineInById(orderId);
        if (indoor != null)
        {
            orderDto.OrderType = OrderType.DineIn.ToString();
            orderDto.TableNumber = indoor.TableNumber;
            return orderDto;
        }

        throw new KeyNotFoundException($"Order with id {orderId} not found.");

    }

    public async Task<bool> ManualPayOrder(string orderId)
    {
        var order = await _orderRepo.GetOrderById(orderId);
        if (order == null) throw new KeyNotFoundException($"Order with id {orderId} not found.");

        if (order.IsPaid) return true;

        order.IsPaid = true;
        await _paymentService.UpdatePaymentStatus(orderId, PaymentStatus.Completed);
        order = await _orderRepo.SetOrderPaidStatus(orderId, true);

        if (order == null) throw new KeyNotFoundException($"Order with id {orderId} not found after updating payment status.");

        return order.IsPaid;
    }

    public async Task<IEnumerable<OrderDto>> GetReadyOrders(string branchId)
    {
        var orders = await _orderRepo.GetReadyOrdersByBranchId(branchId);

        if (orders == null || !orders.Any())
        {
            Console.WriteLine($"No ready orders found for branch with ID: {branchId}");
            return [];
        }

        return orders.Select(o => new OrderDto
        {
            OrderId = o.Id,
            RestaurantName = o.Branch?.Restaurant?.Name ?? "Unknown Restaurant",
            Customer = o.Customer?.Username ?? "Unknown Customer",
            OrderType = o.OrderType.ToString(),
            PaymentMethod = o.Payment?.PaymentMethod ?? "Unknown",
            OrderDate = o.OrderDate,
            OrderStatus = o.Status.ToString(),
            TotalAmount = o.TotalAmount,
            BranchId = o.BranchId,
            IsPaid = o.IsPaid,
        });
    }

    public async Task<IEnumerable<OrderDto>> GetUnpaidOrders(string branchId)
    {
        var orders = await _orderRepo.GetUnpaidOrdersByBranchId(branchId);

        if (orders == null || !orders.Any())
        {
            Console.WriteLine($"No ready orders found for branch with ID: {branchId}");
            return [];
        }

        return orders.Select(o => new OrderDto
        {
            OrderId = o.Id,
            RestaurantName = o.Branch?.Restaurant?.Name ?? "Unknown Restaurant",
            Customer = o.Customer?.Username ?? "Unknown Customer",
            OrderType = o.OrderType.ToString(),
            PaymentMethod = o.Payment?.PaymentMethod ?? "Unknown",
            OrderDate = o.OrderDate,
            OrderStatus = o.Status.ToString(),
            TotalAmount = o.TotalAmount,
            BranchId = o.BranchId,
            IsPaid = o.IsPaid,
        });
    }

}