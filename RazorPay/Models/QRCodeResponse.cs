namespace RazorPay.Models
{
    public class QRCodeResponse
    {
        public string id { get; set; } = default!;
        public string entity { get; set; } = default!;
        public int created_at { get; set; } = 0;
        public string name { get; set; } = default!;
        public string usage { get; set; } = default!;
        public string type { get; set; } = default!;
        public string image_url { get; set; } = default!;
        public decimal payment_amount { get; set; } = default!;
        public string status { get; set; } = default!;
        public bool fixed_amount { get; set; } = false;
        public int payments_amount_received { get; set; } = 0;
        public int payments_count_received { get; set; } = 0;
        public Note notes { get; set; } = default!;
        public int close_by { get; set; } = 0;
    }
    public class Note
    {
        public string purpose { get; set; } = default!;
    }
}