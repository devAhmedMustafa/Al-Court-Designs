using OrdrMate.DTOs.Order;
using OrdrMate.Enums;
using OrdrMate.Models;
using OrdrMate.Repositories;

namespace OrdrMate.Services;

public class PaymentService
{
    private readonly IPaymentRepo _paymentRepo;
    public PaymentService(IPaymentRepo paymentRepo)
    {
        _paymentRepo = paymentRepo;
    }
    public async Task<PaymentDto> AddCashPayment(string orderId, decimal amount)
    {

        var payment = new Payment
        {
            OrderId = orderId,
            Amount = amount,
            PaymentMethod = "Cash",
            Provider = "Cash",
            Status = PaymentStatus.Pending,
        };

        var paymentDto = new PaymentDto
        {
            OrderId = orderId,
            PaidAt = DateTime.UtcNow,
            PaymentMethod = "Cash",
            Amount = 100.00m,
            Status = PaymentStatus.Pending.ToString(),
            Provider = "Cash"
        };

        await _paymentRepo.CreatePayment(payment);

        return paymentDto;
    }
}