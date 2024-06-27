using Newtonsoft.Json;
using Razorpay.Api;
using RazorPay.IServices;
using RazorPay.Models;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text;
using System;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using RestSharp;

namespace RazorPay.Services
{
    public class PaymentService : IPaymentService
    {
        private RazorpayClient client;
        protected HttpClient _client;
        public PaymentService()
        {
            //client = new RazorpayClient("--key--", "--secret--");
            client = new RazorpayClient("rzp_test_ZL1a7FyyQVVw3h", "stuoN7OM8kNOYtZzIc4s4Ukb");
            //var credentials = new NetworkCredential("rzp_test_ZL1a7FyyQVVw3h", "stuoN7OM8kNOYtZzIc4s4Ukb");
            //var handler = new HttpClientHandler { Credentials = credentials };
            _client = new HttpClient();
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

        public async Task<string> CreateStandardPaymentLink()
        {
            var baseURL = new Uri("https://api.razorpay.com/");
            string clientId = "rzp_test_ZL1a7FyyQVVw3h";
            string clientSecret = "stuoN7OM8kNOYtZzIc4s4Ukb";
            var uri = "v1/payment_links";
            Dictionary<string, object> paymentLinkRequest = new Dictionary<string, object>();
            paymentLinkRequest.Add("amount", 1000);
            paymentLinkRequest.Add("currency", "INR");
            paymentLinkRequest.Add("accept_partial", false);
            paymentLinkRequest.Add("first_min_partial_amount", 100);
            paymentLinkRequest.Add("expire_by", DateTime.UtcNow.AddMinutes(15).Ticks);
            paymentLinkRequest.Add("reference_id", "TSsd19896");
            paymentLinkRequest.Add("description", "Payment for policy no #23456");
            Dictionary<string, string> customer = new Dictionary<string, string>();
            customer.Add("contact", "+919999999999");
            customer.Add("name", "Gaurav Kumar");
            customer.Add("email", "gaurav.kumar@example.com");
            paymentLinkRequest.Add("customer", customer);
            Dictionary<string, object> notify = new Dictionary<string, object>();
            notify.Add("sms", true);
            notify.Add("email", true);
            paymentLinkRequest.Add("reminder_enable", true);
            Dictionary<string, object> notes = new Dictionary<string, object>();
            notes.Add("policy_name", "Jeevan Bima");
            paymentLinkRequest.Add("notes", notes);
            paymentLinkRequest.Add("callback_url", "https://example-callback-url.com/");
            paymentLinkRequest.Add("callback_method", "get");
            //var jsonData = JsonConvert.SerializeObject(paymentLinkRequest);
            var jsonData = System.Text.Json.JsonSerializer.Serialize(paymentLinkRequest);
            //GetCredential(uri, clientId, clientSecret);
            //_client.DefaultRequestHeaders.Add("accept", "application/json");
            var basicAuthenticationValue =
    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));

            //_client.DefaultRequestHeaders.Authorization =
            //    new AuthenticationHeaderValue("Basic", basicAuthenticationValue);

            //_client.DefaultRequestHeaders.Authorization
            //= new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
            //    Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}")));

            //string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{clientId}:{clientSecret}"));

            //var authenticationString = $"{clientId}:{clientSecret}";
            //var base64EncodedAuthenticationString = Convert.ToBase64String(Encoding.UTF8.GetBytes(authenticationString));
            //_client.DefaultRequestHeaders.Add("Authorization", "Basic " + basicAuthenticationValue);
            //byte[] basicBytes = Encoding.Unicode.GetBytes($"{clientId}:{clientSecret}");
            //string basicB64 = Convert.ToBase64String(basicBytes);

            //_client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", $"{clientId}", $"{clientSecret}"))));
            //_client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue("Basic", basicB64);
            //var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //var content = new StringContent(jsonData);
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
            //// Send POST request
            //var response = await _client.PostAsJsonAsync(uri, paymentLinkRequest, options);
            //var authenticationString = $"{clientId}:{clientSecret}";
            //var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));

            //var requestMessage = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, uri);
            //requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
            //requestMessage.Content = content;

            ////make the request
            //var response = await _client.SendAsync(requestMessage);
            //if (response.IsSuccessStatusCode)
            //{
            //    response.EnsureSuccessStatusCode();
            //    // Read and deserialize the response content
            //    var responseContent = await response.Content.ReadAsStringAsync();
            //    return responseContent;
            //}
            //else
            //    return string.Empty;
            //PaymentLink paymentlink = client.PaymentLink.Create(paymentLinkRequest);


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
            Console.WriteLine(response.Content);
            return response.Content;
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

        public async Task<string> CreateQRCode()
        {
            var baseURL = new Uri("https://api.razorpay.com/v1/");
            string clientId = "rzp_test_ZL1a7FyyQVVw3h";
            string clientSecret = "stuoN7OM8kNOYtZzIc4s4Ukb";
            var uri = "payments/qr_codes";
            var basicAuthenticationValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
            Dictionary<string, object> qrRequest = new Dictionary<string, object>();
            qrRequest.Add("type", "upi_qr");
            qrRequest.Add("name", "Store_1");
            qrRequest.Add("usage", "single_use");
            qrRequest.Add("fixed_amount", true);
            qrRequest.Add("payment_amount", 300);
            qrRequest.Add("description", "For Store 1");
            qrRequest.Add("customer_id", "cust_MHYe2dVX323WYD");
            qrRequest.Add("close_by", DateTime.UtcNow.AddMinutes(15).Ticks);
            Dictionary<string, object> notes = new Dictionary<string, object>();
            notes.Add("notes_key_1", "Tea, Earl Grey, Hot");
            notes.Add("notes_key_2", "Tea, Earl Grey… decaf.");
            qrRequest.Add("notes", notes);

            var jsonData = JsonConvert.SerializeObject(qrRequest);
            //            var body = @"{" + "\n" +
            //@"    ""type"": ""upi_qr""," + "\n" +
            //@"    ""name"": ""Store_1""," + "\n" +
            //@"    ""usage"": ""single_use""," + "\n" +
            //@"    ""fixed_amount"": true," + "\n" +
            //@"    ""payment_amount"": 300," + "\n" +
            //@"    ""description"": ""For Store 1""," + "\n" +
            //@"    ""customer_id"": ""cust_HKsR5se84c5LTO""," + "\n" +
            //@"    ""close_by"": 1981615838," + "\n" +
            //@"    ""notes"": {" + "\n" +
            //@"        ""purpose"": ""Test UPI QR code notes""" + "\n" +
            //@"    }" + "\n" +
            //@"}";
            var options1 = new RestClientOptions(baseURL)
            {
                Timeout = TimeSpan.FromMinutes(10)
            };
            var client = new RestSharp.RestClient(options1);
            var request = new RestRequest(uri, RestSharp.Method.Post);
            request.AddHeader("Authorization", $"Basic {basicAuthenticationValue}");
            //request.AddHeader("Content-Type", "application/json");
            //request.AddStringBody(body, DataFormat.Json);
            request.AddParameter("text/plain", jsonData, ParameterType.RequestBody);
            RestResponse response = await client.ExecuteAsync(request);
            Console.WriteLine(response.Content);
            return response.Content;
        }
    }
}