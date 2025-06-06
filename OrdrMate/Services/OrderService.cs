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
    private readonly PaymobService _paymobService;

    public OrderService(PaymentService paymentService, IOrderRepo orderRepo, PaymobService paymobService)
    {
        _paymentService = paymentService;
        _orderRepo = orderRepo;
        _paymobService = paymobService;
    }

    public async Task<OrderIntentDto> CreateOrderIntent(PlaceOrderDto orderIntent)
    {

        var totalAmount = orderIntent.Items.Sum(oi => oi.Price * oi.Quantity);

        var intent = new OrderIntent
        {
            CustomerId = orderIntent.CustomerId,
            BranchId = orderIntent.BranchId,
            Status = PaymentStatus.INITIATED,
            Amount = totalAmount,
            PaymentMethod = orderIntent.PaymentMethod,
            OrderType = orderIntent.OrderType,
            PaymentProvider = orderIntent.PaymentMethod == "Cash" ? "Cash" : "Paymob",
            OrderItems = [.. orderIntent.Items.Select(oi => new OrderItemDto
            {
                ItemId = oi.ItemId,
                Quantity = oi.Quantity,
                Price = oi.Price,
            })],
        };

        var redirectUrl = string.Empty;

        switch (intent.PaymentProvider.ToLower())
        {
            case "cash":
                var order = await ConfirmOrder(intent);
                intent.OrderId = order.Id;
                break;

            case "paymob":
                var intentResponse = await CreatePaymentSession(intent)
                    ?? throw new InvalidOperationException("Failed to create payment session with Paymob.");

                if (string.IsNullOrEmpty(intentResponse.RedirectUrl)) throw new InvalidOperationException("Redirect URL is empty from Paymob response.");

                redirectUrl = intentResponse.RedirectUrl;
                intent.Id = intentResponse.OrderId;
                break;
            default:
                throw new NotSupportedException($"Payment provider {intent.PaymentProvider} is not supported.");
        }


        var savedIntent = await _orderRepo.CreateOrderIntent(intent);

        return new OrderIntentDto
        {
            OrderIntentId = savedIntent.Id,
            RedirectUrl = redirectUrl,
        };
    }

    public async Task<IntentResponse> CreatePaymentSession(OrderIntent orderIntent)
    {
        return await _paymobService.CreateOrderIntent(orderIntent.Amount, orderIntent.PaymentMethod);
    }

    public async Task<OrderDto> ProceedTransaction(string orderIntentId, string transactionId)
    {
        var orderIntent = await _orderRepo.GetOrderIntentById(orderIntentId) ?? throw new KeyNotFoundException($"Order intent with id {orderIntentId} not found.");
        if (orderIntent.Status != PaymentStatus.INITIATED) throw new InvalidOperationException($"Order intent with id {orderIntentId} is not in a valid state for processing.");

        var order = await ConfirmOrder(orderIntent, true) ?? throw new InvalidOperationException($"Failed to confirm order for order intent with id {orderIntentId}.");
        orderIntent.OrderId = order.Id;

        var payment = await ProcessPayment(orderIntent, transactionId) ?? throw new InvalidOperationException($"Failed to process payment for order intent with id {orderIntentId}.");
        orderIntent.Status = PaymentStatus.Completed;

        return new OrderDto
        {
            OrderId = order.Id,
            BranchId = order.BranchId,
            RestaurantName = order.Branch?.Restaurant?.Name ?? "Unknown Restaurant",
            Customer = order.Customer?.Username ?? "Unknown Customer",
            OrderType = orderIntent.OrderType.ToString(),
            PaymentMethod = orderIntent.PaymentMethod,
            OrderDate = order.OrderDate,
            OrderStatus = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
        };
    }

    private async Task<Order> ConfirmOrder(OrderIntent orderIntent, bool isPaid = false)
    {
        var existingOrder = await _orderRepo.GetOrderById(orderIntent.OrderId!);
        if (existingOrder != null)
        {
            Console.WriteLine($"Order with ID {orderIntent.OrderId} already exists. Skipping order creation.");
            return existingOrder;
        }
        
        var order = new Order
        {
            BranchId = orderIntent.BranchId,
            CustomerId = orderIntent.CustomerId,
            OrderType = orderIntent.OrderType,
            TotalAmount = orderIntent.Amount,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Queued,
            IsPaid = isPaid,
        };

        order = await _orderRepo.CreateOrder(order);

        List<OrderItem> orderItems = [];

        if (orderIntent.OrderItems != null && orderIntent.OrderItems.Any())
        {
            foreach (var item in orderIntent.OrderItems)
            {
                var orderItem = new OrderItem
                {
                    ItemId = item.ItemId,
                    OrderId = order.Id,
                    Quantity = item.Quantity,
                    Price = item.Price,
                };

                orderItem = await _orderRepo.CreateOrderItem(orderItem);
                orderItems.Add(orderItem);
            }
        }

        OrderEvents.OnOrderPlaced(order.BranchId, orderItems);

        switch (orderIntent.OrderType)
        {
            case OrderType.Takeaway:
                await PlaceTakeawayOrder(order);
                break;
            default:
                throw new NotImplementedException($"Order type {orderIntent.OrderType} is not implemented yet.");
        }

        return order;
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

    private async Task<PaymentDto> ProcessPayment(OrderIntent orderIntent, string transactionId)
    {
        return await _paymentService.AddPayment(orderIntent, transactionId);
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

public class IntentResponse
{
    public required string OrderId { get; set; }
    public required string RedirectUrl { get; set; }
}