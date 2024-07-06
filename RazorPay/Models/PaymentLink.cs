using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace RazorPay.Models
{
    public class PaymentLink
    {
        [Required(ErrorMessage ="{0} is required")]
        public decimal Amount { get; set; } = decimal.Zero;
        public string MobileNo { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
    }
}