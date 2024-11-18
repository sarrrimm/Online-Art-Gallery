using ArtGallery.Models;
using System.ComponentModel.DataAnnotations;

namespace ArtGallery.ViewModels
{
    public class CheckoutViewModel
    {
        [Key]
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public List<CartItem>? Items { get; set; }
        public double? TotalAmount { get; set; }
        public double? GrandTotal { get; set; }
        public double? ShippingAmount { get; set; }
    }
}
