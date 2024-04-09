using Razorpay.Api;
using RazorPay.Models;

namespace RazorPay.IServices
{
    public interface IPaymentService
    {
        //Make payment
        Task<MerchantOrder> ProcessMerchantOrder(PaymentRequest payment);
        Task<string> CompleteOrderProcess(IHttpContextAccessor httpContextAccessor);
        //Refund Payment
        Task<List<Refund>> FetchAllRefundOrder();
        Task<Refund> FetchParticularRefund(string refundId);
        Task<List<Refund>> FetchMultipleRefunds(string paymentId);
        Task<Refund> CreateAnInstantRefund(PaymentRefund paymentRefund);
        Task<Refund> CreateANormalRefund(PaymentRefund paymentRefund);
        //Task<Refund> UpdateTheRefund(string refundId);
        //Payments
        Task<List<Payment>> FetchAllPayments();
        Task<Payment> FetchAPayment(string PaymentId);
        //settlements
        //Task<List<Settlement>> FetchAllSettlements();
        //Task<Settlement> FetchAsettlement(string settlementId);
        //Payment Link
        //Task<string> CreateStandardPaymentLink();
        //Task<string> CreateUPIPaymentLink();
    }
}