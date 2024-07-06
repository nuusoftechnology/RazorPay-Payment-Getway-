namespace RazorPay.Models
{
    public class QRCode
    {
        public decimal Amount { get; set; } = decimal.Zero;
        public string Name { get; set; } = default!;
    }
}