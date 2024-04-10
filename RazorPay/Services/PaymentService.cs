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
            client = new RazorpayClient("--key--", "--secret--");
            //client = new RazorpayClient("--key--", "--secret--");
        }

        public async Task<Payment> CapturePayment(string PaymentId, decimal amount)
        {
            Dictionary<string, object> paymentRequest = new Dictionary<string, object>();
            paymentRequest.Add("amount", amount);
            paymentRequest.Add("currency", "INR");

            Payment payment = client.Payment.Fetch(PaymentId).Capture(paymentRequest);
            return await Task.FromResult(payment);
        }

        public async Task<string> CompleteOrderProcess(IHttpContextAccessor httpContextAccessor)
        {
            string paymentId = httpContextAccessor.HttpContext.Request.Form["rzp_paymentid"].ToString();
            // This is orderId
            string orderId = httpContextAccessor.HttpContext.Request.Form["rzp_orderid"];
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
            try
            {
                Dictionary<string, object> refundRequest = new Dictionary<string, object>();
                refundRequest.Add("amount", paymentRefund.Amount);
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
            try
            {
                Dictionary<string, object> refundRequest = new Dictionary<string, object>();
                refundRequest.Add("amount", paymentRefund.Amount);
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
            paymentRequest.Add("count", "100");
            List<Payment> payment = client.Payment.All(paymentRequest);
            return await Task.FromResult(payment);
        }

        public async Task<List<Refund>> FetchAllRefundOrder()
        {
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
            Refund refund = client.Refund.Fetch(refundId);
            return await Task.FromResult(refund);
        }

        public async Task<MerchantOrder> ProcessMerchantOrder(PaymentRequest payment)
        {
            // Generate random receipt number for order
            Random randomObj = new Random();
            string transactionId = randomObj.Next(001, 999).ToString();
            string receiptId = $"{Guid.NewGuid()}-{transactionId}";
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
                RazorpayKey = "--key--",
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
