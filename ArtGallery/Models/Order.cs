namespace ArtGallery.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public List<CartItem>? Items { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public double? TotalAmount { get; set; }
        public string Address { get; set; }
    }
}
