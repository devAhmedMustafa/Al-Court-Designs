namespace OrdrMate.Repositories;

using OrdrMate.Models;

public interface IPaymentRepo
{
    public Task<Payment> CreatePayment(Payment payment);
}