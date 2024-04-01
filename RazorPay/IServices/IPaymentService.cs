using RazorPay.Models;

namespace RazorPay.IServices
{
    public interface IPaymentService
    {
        Task<MerchantOrder> ProcessMerchantOrder(PaymentRequest payment);
        Task<string> CompleteOrderProcess(IHttpContextAccessor httpContextAccessor);
    }
}
