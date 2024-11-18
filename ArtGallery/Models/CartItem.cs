namespace ArtGallery.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ArtId { get; set; }
        public string Title { get; set; }
        public double? Price { get; set; }
        public string Author { get; set; }
        public string ImagePath { get; set; }
        
    }
}
