namespace RazorPay.Models
{
    public class PaymentRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
        public decimal Amount { get; set; }
    }
}
