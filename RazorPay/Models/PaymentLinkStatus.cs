namespace RazorPay.Models
{
    public class PaymentLinkStatus
    {
        public List<PaymentLinks> payment_links { get; set; } = new();
    }
}
