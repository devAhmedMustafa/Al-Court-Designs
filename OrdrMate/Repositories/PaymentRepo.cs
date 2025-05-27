namespace OrdrMate.Repositories;

using OrdrMate.Data;
using OrdrMate.Models;

public class PaymentRepo : IPaymentRepo
{
    private readonly OrdrMateDbContext _db;
    public PaymentRepo(OrdrMateDbContext context)
    {
        _db = context;
    }
    public async Task<Payment> CreatePayment(Payment payment)
    {
        _db.Payment.Add(payment);
        await _db.SaveChangesAsync();
        return payment;
    }
}