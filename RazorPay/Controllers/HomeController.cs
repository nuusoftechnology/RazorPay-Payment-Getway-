using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using RazorPay.IServices;
using RazorPay.Models;
using System.Diagnostics;

namespace RazorPay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IPaymentService _paymentService;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContext, IPaymentService payment)
        {
            _logger = logger; _contextAccessor = httpContext; _paymentService = payment;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetAllPayment()
        {
            var payment = await _paymentService.FetchAllPayments();
            return View(payment);
        }
        [HttpPost]
        public async Task<IActionResult> ProcessRequestOrder(PaymentRequest payment)
        {
            MerchantOrder order = await _paymentService.ProcessMerchantOrder(payment);
            return View("_Payment", order);
        }
        [HttpPost]
        public async Task<IActionResult> CompleteOrderProcess()
        {
            string PaymentStatus = await _paymentService.CompleteOrderProcess(_contextAccessor);
            if (PaymentStatus.Equals("captured"))
                return RedirectToAction(nameof(status), new { status = "Success" });
            else
                return RedirectToAction(nameof(status), new { status = "failed" });
        }
        public IActionResult status(string Status)
        {
            return View("_OrderStatus", Status);
        }
        public async Task<IActionResult> FetchAllRefund()
        {
            var refund = await _paymentService.FetchAllRefundOrder();
            return View(refund);
        }
        [HttpGet]
        public IActionResult CreateANormalRefund(string id, decimal amount)
        {
            PaymentRefund paymentRefund = new PaymentRefund() { paymentId = id, Amount = amount};
            return View(paymentRefund);
        }
        public async Task<IActionResult> CapturePayment(string id, decimal amount)
        {
            var payment = await _paymentService.CapturePayment(id, amount);
            return View("_OrderStatus", payment.Attributes["status"].ToString());
        }
        [HttpPost]
        public async Task<IActionResult> CreateANormalRefund(PaymentRefund payment)
        {
            var refund = await _paymentService.CreateANormalRefund(payment);
            return View("_OrderStatus", refund.Attributes["status"].ToString());
        }
        [HttpGet]
        public IActionResult CreateAnInstantRefund(string id, decimal amount)
        {
            PaymentRefund paymentRefund = new PaymentRefund() { paymentId = id, Amount = amount };
            return View(paymentRefund);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAnInstantRefund(PaymentRefund payment)
        {
            var refund = await _paymentService.CreateAnInstantRefund(payment);
            return View("_OrderStatus", refund.Attributes["status"].ToString());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
