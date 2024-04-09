using Razorpay.Api;
using RazorPay.IServices;
using RazorPay.Models;

namespace RazorPay.Services
{
    public class PaymentService : IPaymentService
    {
        private RazorpayClient client;
        public PaymentService()
        {
            //client = new RazorpayClient("rzp_live_N6AwHPPkur5r3P", "7Q3pINUv4n19cQf0M7Csctkt");
            client = new RazorpayClient("rzp_test_ZL1a7FyyQVVw3h", "stuoN7OM8kNOYtZzIc4s4Ukb");
        }
        public async Task<string> CompleteOrderProcess(IHttpContextAccessor httpContextAccessor)
        {
            string paymentId = httpContextAccessor.HttpContext.Request.Form["rzp_paymentid"].ToString();
            // This is orderId
            string orderId = httpContextAccessor.HttpContext.Request.Form["rzp_orderid"];
            //RazorpayClient client = new RazorpayClient("rzp_test_ZL1a7FyyQVVw3h", "stuoN7OM8kNOYtZzIc4s4Ukb");

            Payment payment = client.Payment.Fetch(paymentId);
            // This code is for capture the payment 
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", payment.Attributes["amount"]);
            Payment paymentCaptured = payment.Capture(options);
            //string amt = paymentCaptured.Attributes["amount"];
            //// Check payment made successfully
            return await Task.FromResult(paymentCaptured.Attributes["status"]);
        }

        public async Task<Refund> CreateAnInstantRefund(PaymentRefund paymentRefund)
        {
            //String paymentId = "pay_Z6t7VFTb9xHeOs";
            try
            {
                Dictionary<string, object> refundRequest = new Dictionary<string, object>();
                refundRequest.Add("amount", paymentRefund.Amount * 100);
                refundRequest.Add("speed", "optimum");
                refundRequest.Add("receipt", paymentRefund.Receipt);
                Refund refund = client.Payment.Fetch(paymentRefund.paymentId).Refund(refundRequest);
                return await Task.FromResult(refund);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<Refund> CreateANormalRefund(PaymentRefund paymentRefund)
        {
            //string paymentId = "pay_Z6t7VFTb9xHeOs";
            try
            {
                Dictionary<string, object> refundRequest = new Dictionary<string, object>();
                refundRequest.Add("amount", paymentRefund.Amount * 100);
                refundRequest.Add("speed", "normal");
                Dictionary<string, object> notes = new Dictionary<string, object>();
                notes.Add("notes_key_1", paymentRefund.Notes1);
                notes.Add("notes_key_2", paymentRefund.Notes2);
                refundRequest.Add("notes", notes);
                refundRequest.Add("receipt", paymentRefund.Receipt); //A unique identifier provided by you for your internal reference.
                Refund refund = client.Payment.Fetch(paymentRefund.paymentId).Refund(refundRequest);
                return await Task.FromResult(refund);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<Payment>> FetchAllPayments()
        {
            Dictionary<string, object> paymentRequest = new Dictionary<string, object>();
            paymentRequest.Add("count", "10");
            List<Payment> payment = client.Payment.All(paymentRequest);
            return await Task.FromResult(payment);
        }

        public async Task<List<Refund>> FetchAllRefundOrder()
        {
            //RazorpayClient client = new RazorpayClient("rzp_test_ZL1a7FyyQVVw3h", "stuoN7OM8kNOYtZzIc4s4Ukb");
            Dictionary<string, object> paramRequest = new Dictionary<string, object>();
            //paramRequest.Add("from", "1");
            //paramRequest.Add("to", "1");
            //paramRequest.Add("count", "1");// default: 10
            //paramRequest.Add("skip", "0");
            List<Refund> refund = client.Refund.All(paramRequest);
            return await Task.FromResult(refund);
        }

        public async Task<Payment> FetchAPayment(string PaymentId)
        {
            Payment payment = client.Payment.Fetch(PaymentId);
            return await Task.FromResult(payment);
        }

        public async Task<List<Refund>> FetchMultipleRefunds(string paymentId)
        {
            //String paymentId = "pay_Z6t7VFTb9xHeOs";
            Dictionary<string, object> paramRequest = new Dictionary<string, object>();
            //paramRequest.Add("from", "1");
            //paramRequest.Add("to", "1");
            //paramRequest.Add("count", "1");// default: 10
            //paramRequest.Add("skip", "0");
            List<Refund> refund = client.Payment.Fetch(paymentId).AllRefunds(paramRequest);
            return await Task.FromResult(refund);
        }

        public async Task<Refund> FetchParticularRefund(string refundId)
        {
            //string refundId = "rfnd_Z6t7VFTb9xHeOs";

            Refund refund = client.Refund.Fetch(refundId);
            return await Task.FromResult(refund);
        }

        public async Task<MerchantOrder> ProcessMerchantOrder(PaymentRequest payment)
        {
            // Generate random receipt number for order
            Random randomObj = new Random();
            string transactionId = randomObj.Next(001, 999).ToString();
            string receiptId = $"{Guid.NewGuid()}-{transactionId}";
            //RazorpayClient client = new RazorpayClient("rzp_test_ZL1a7FyyQVVw3h", "stuoN7OM8kNOYtZzIc4s4Ukb");
            Dictionary<string, object> options = new Dictionary<string, object>();
            options.Add("amount", payment.Amount * 100);  // Amount will in paise
            options.Add("receipt", receiptId);
            options.Add("currency", "INR");
            options.Add("payment_capture", "1"); // 1 - automatic  , 0 - manual
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

        //public async Task<Refund> UpdateTheRefund(string refundId)
        //{
        //    //string refundId = "rfnd_Z6t7VFTb9xHeOs";

        //    Dictionary<string, object> refundRequest = new Dictionary<string, object>();
        //    Dictionary<string, object> notes = new Dictionary<string, object>();
        //    notes.Add("notes_key_1", "Tea, Earl Grey, Hot update");
        //    notes.Add("notes_key_2", "Tea, Earl Grey… decaf.");
        //    refundRequest.Add("notes", notes);

        //    Refund refund = client.Refund.Fetch(refundId).Edit(refundRequest);
        //    return await Task.FromResult(refund);
        //}
    }
}
