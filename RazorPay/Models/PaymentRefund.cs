namespace RazorPay.Models
{
    public class PaymentRefund
    {
        public string paymentId { get; set; } = default!;
        public decimal Amount { get; set; } = decimal.Zero;
        public string Notes1 { get; set; } = default!;
        public string Notes2 { get; set; } = default!;
        public string Receipt { get; set;} = default!;
        public string currency { get; set; } = "INR";
    }
}
