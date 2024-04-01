using Razorpay.Api;
using RazorPay.IServices;
using RazorPay.Models;

namespace RazorPay.Services
{
    public class PaymentService : IPaymentService
    {
        public async Task<string> CompleteOrderProcess(IHttpContextAccessor httpContextAccessor)
        {
            string paymentId = httpContextAccessor.HttpContext.Request.Form["rzp_paymentid"].ToString();
            // This is orderId
            string orderId = httpContextAccessor.HttpContext.Request.Form["rzp_orderid"];
            RazorpayClient client = new RazorpayClient("rzp_test_ZL1a7FyyQVVw3h", "stuoN7OM8kNOYtZzIc4s4Ukb");

            Payment payment = client.Payment.Fetch(paymentId);
            // This code is for capture the payment 
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", payment.Attributes["amount"]);
            Payment paymentCaptured = payment.Capture(options);
            //string amt = paymentCaptured.Attributes["amount"];
            //// Check payment made successfully
            return await Task.FromResult(paymentCaptured.Attributes["status"]);
        }

        public async Task<MerchantOrder> ProcessMerchantOrder(PaymentRequest payment)
        {
            // Generate random receipt number for order
            Random randomObj = new Random();
            string transactionId = randomObj.Next(001, 999).ToString();
            string receiptId = $"{Guid.NewGuid()}-{transactionId}";
            RazorpayClient client = new RazorpayClient("rzp_test_ZL1a7FyyQVVw3h", "stuoN7OM8kNOYtZzIc4s4Ukb");
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", payment.Amount * 100);  // Amount will in paise
            options.Add("receipt", receiptId);
            options.Add("currency", "INR");
            options.Add("payment_capture", "0"); // 1 - automatic  , 0 - manual
                                                 //options.Add("notes", "-- You can put any notes here --");
            Order orderResponse = client.Order.Create(options);
            string orderId = orderResponse["id"].ToString();

            MerchantOrder merchantOrder = new MerchantOrder()
            {
                OrderId = orderResponse.Attributes["id"],
                RazorpayKey = "rzp_test_ZL1a7FyyQVVw3h",
                Amount = payment.Amount * 100,
                Currency = "INR",
                Name = payment.Name,
                Email = payment.Email,
                ContactNumber = payment.ContactNumber,
                Address = payment.Address,
                Description = "Testing description"
            };
            return await Task.FromResult(merchantOrder);
        }
    }
}
