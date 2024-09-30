using Newtonsoft.Json;
using Razorpay.Api;
using RazorPay.IServices;
using RazorPay.Models;
using RestSharp;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RazorPay.Services
{
    public class PaymentService : IPaymentService
    {
        private RazorpayClient client;
        protected HttpClient _client;
        public PaymentService()
        {
            client = new RazorpayClient("--key--", "--secret--");
            //var handler = new HttpClientHandler { Credentials = credentials };
            _client = new HttpClient();
        }
        //https://www.postman.com/razorpaydev/workspace/razorpay-public-workspace/collection/12492020-952c7295-118c-400f-8f2c-5266ef6f689a
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

        public async Task<PaymentLinkResponse> CreateStandardPaymentLink(decimal Amount, string MobileNo, string Name, string Email)
        {
            var expTime = TimeProvider.System.GetUtcNow().AddDays(1).ToUnixTimeSeconds();
            var baseURL = new Uri("https://api.razorpay.com/");
            //Test Mode
            string clientId = "rzp_test_ZL1a7FyyQVVw3h";
            string clientSecret = "stuoN7OM8kNOYtZzIc4s4Ukb";
            var uri = "v1/payment_links";
            Dictionary<string, object> paymentLinkRequest = new Dictionary<string, object>();
            paymentLinkRequest.Add("amount", (Amount * 100));
            paymentLinkRequest.Add("currency", "INR");
            paymentLinkRequest.Add("accept_partial", false);
            //paymentLinkRequest.Add("first_min_partial_amount", 100);
            paymentLinkRequest.Add("expire_by", expTime);
            //paymentLinkRequest.Add("reference_id", "GauravKumar");
            //paymentLinkRequest.Add("description", "Payment for policy no #23456");
            Dictionary<string, string> customer = new Dictionary<string, string>();
            customer.Add("contact", $"+91{MobileNo}");
            customer.Add("name", $"{Name}");
            customer.Add("email", $"{Email}");
            paymentLinkRequest.Add("customer", customer);
            Dictionary<string, object> notify = new Dictionary<string, object>();
            notify.Add("sms", true);
            notify.Add("email", true);
            notify.Add("whatsapp", true);
            paymentLinkRequest.Add("reminder_enable", true);
            //Dictionary<string, object> notes = new Dictionary<string, object>();
            //notes.Add("policy_name", "Jeevan Bima");
            //paymentLinkRequest.Add("notes", notes);
            paymentLinkRequest.Add("callback_url", "https://nuusoftechnology.com/");
            paymentLinkRequest.Add("callback_method", "get");
            var jsonData = System.Text.Json.JsonSerializer.Serialize(paymentLinkRequest);
            var basicAuthenticationValue =
    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
            // Configure required JSON serialization options
            var options = new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                IgnoreReadOnlyProperties = true,
                NumberHandling = JsonNumberHandling.WriteAsString,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };


            var options1 = new RestClientOptions(baseURL)
            {
                Timeout = TimeSpan.FromMinutes(10)
            };
            var client = new RestSharp.RestClient(options1);
            var request = new RestRequest(uri, RestSharp.Method.Post);
            request.AddHeader("Authorization", $"Basic {basicAuthenticationValue}");
            request.AddHeader("Content-Type", "application/json");
            request.AddStringBody(jsonData, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);
            PaymentLinkResponse linkResponse = JsonConvert.DeserializeObject<PaymentLinkResponse>(response.Content);
            //Console.WriteLine(response.Content);
            return linkResponse;
        }

        public Task<string> CreateUPIPaymentLink()
        {
            throw new NotImplementedException();
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
        private CredentialCache GetCredential(string uri, string userName, string Password)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            CredentialCache credentialCache = new CredentialCache();
            credentialCache.Add(new System.Uri(uri), "Basic", new NetworkCredential(userName: userName, password: Password));
            return credentialCache;
        }

        public async Task<QRCodeResponse> CreateQRCode(decimal Amount, string Name)
        {
            var expTime = TimeProvider.System.GetUtcNow().AddDays(1).ToUnixTimeSeconds();
            var baseURL = new Uri("https://api.razorpay.com/v1/");
            //Test Mode
            string clientId = "rzp_test_ZL1a7FyyQVVw3h";
            string clientSecret = "stuoN7OM8kNOYtZzIc4s4Ukb";
            var uri = "payments/qr_codes";
            var basicAuthenticationValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
            Dictionary<string, object> qrRequest = new Dictionary<string, object>();
            qrRequest.Add("type", "upi_qr");
            qrRequest.Add("name", $"{Name}");
            qrRequest.Add("usage", "single_use");
            qrRequest.Add("fixed_amount", true);
            qrRequest.Add("payment_amount", (Amount * 100));
            //qrRequest.Add("description", "This is test description request. For getting QR Code Test Test Test Test Test Test Test Test Test Test Test Test ");
            //qrRequest.Add("customer_id", "cust_HKsR5se84c5LTO");
            qrRequest.Add("close_by", expTime);
            Dictionary<string, object> notes = new Dictionary<string, object>();
            notes.Add("purpose", "Test UPI QR code notes");
            qrRequest.Add("notes", notes);
            var jsonData = JsonConvert.SerializeObject(qrRequest);
            var options1 = new RestClientOptions(baseURL)
            {
                Timeout = TimeSpan.FromMinutes(10)
            };
            var client = new RestSharp.RestClient(options1);
            var request = new RestRequest(uri, RestSharp.Method.Post);
            request.AddHeader("Authorization", $"Basic {basicAuthenticationValue}");
            request.AddHeader("Content-Type", "application/json");
            request.AddStringBody(jsonData, DataFormat.Json);
            RestResponse response = await client.ExecuteAsync(request);
            QRCodeResponse codeResponse = JsonConvert.DeserializeObject<QRCodeResponse>(response.Content);
            return codeResponse;
        }
    }
}