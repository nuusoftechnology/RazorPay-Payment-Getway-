namespace RazorPay.Models
{
    public class PaymentLinkResponse
    {
        public string id { get; set; } = default!;
        public string accept_partial { get; set; } = default!;
        public decimal amount { get; set; }
        public decimal amount_paid { get; set; }
        public string callback_method { get; set; } = default!;
        public string callback_url { get; set; } = default!;
        public int cancelled_at { get; set; } = 0;
        public int created_at { get; set; } = 0;
        public string currency { get; set; } = default!;
        public Customer customer { get; set; } = default!;
        public string description { get; set; } = default!;
        public int expire_by { get; set; } = 0;
        public int expired_at { get; set; } = 0;
        public int first_min_partial_amount { get; set; } = 0;
        public Notes notes { get; set; } = default!;
        public Notify notify { get; set; } = default!;
        public string reference_id { get; set; } = default!;
        public bool reminder_enable { get; set; }
        public string short_url { get; set; } = default!;
        public string status { get; set; } = default!;
        public int updated_at { get; set; } = 0;
        public bool upi_link { get; set; } = false;
        public bool whatsapp_link { get; set; } = false;
    }
    public class Customer
    {
        public string contact { get; set; } = default!;
        public string email { get; set; } = default!;
        public string name { get; set; } = default!;
    }
    public class Notes
    {
        public string policy_name { get; set; } = default!;
    }
    public class Notify
    {
        public bool email { get; set; }
        public bool sms { get; set; }
        public bool whatsapp { get; set; }
    }
}