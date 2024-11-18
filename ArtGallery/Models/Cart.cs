using Microsoft.Identity.Client;

namespace ArtGallery.Models
{
    public class Cart
    {
        public int CartId {  get; set; }
        public string UserId { get; set; }
        public List<CartItem>? cartItems { get; set; }
    }
}
