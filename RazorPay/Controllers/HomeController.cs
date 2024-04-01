using Microsoft.AspNetCore.Mvc;
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
            return View("_OrderStatus",Status);
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
