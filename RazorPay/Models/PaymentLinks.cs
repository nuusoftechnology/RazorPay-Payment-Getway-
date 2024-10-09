using Razorpay.Api;

namespace RazorPay.Models
{
    public class PaymentLinks
    {
        public string accept_partial { get; set; } = default!;
        public decimal amount { get; set; }
        public decimal amount_paid { get; set; }
        public string callback_url { get; set; } = default!;
        public int cancelled_at { get; set; } = 0;
        public int created_at { get; set; } = 0;
        public string currency { get; set; } = default!;
        //public Customer customer { get; set; } = default!;
        public string description { get; set; } = default!;
        public int expire_by { get; set; } = 0;
        public int expired_at { get; set; } = 0;
        public int first_min_partial_amount { get; set; } = 0;
        //public Notes? notes { get; set; } = new();
        public Notify notify { get; set; } = new();
        public string order_id { get; set; } = default!;
        //public Payment? payments { get; set; } = new();
        public string reference_id { get; set; } = default!;
        public bool reminder_enable { get; set; }
        public string short_url { get; set; } = default!;
        public string status { get; set; } = default!;
        public int updated_at { get; set; } = 0;
        public bool upi_link { get; set; } = false;
        public bool whatsapp_link { get; set; } = false;
    }
}
