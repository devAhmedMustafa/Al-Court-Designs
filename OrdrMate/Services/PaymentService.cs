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

    public async Task<bool> UpdatePaymentStatus(string orderId, PaymentStatus status)
    {
        var payment = await _paymentRepo.GetPaymentByOrderId(orderId);
        if (payment == null)
        {
            throw new KeyNotFoundException($"Payment with id {orderId} not found.");
        }

        payment.Status = status;
        if (status == PaymentStatus.Completed)
        {
            payment.PaidAt = DateTime.UtcNow;
        }

        var updatedPayment = await _paymentRepo.UpdatePayment(payment);
        return updatedPayment != null;
    }
}