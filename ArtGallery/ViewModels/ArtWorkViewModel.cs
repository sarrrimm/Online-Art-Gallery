using ArtGallery.Models;

namespace ArtGallery.ViewModels
{
    public class ArtWorkViewModel
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image{ get; set; }
        public int? AuctionId { get; set; }
        public string? Category { get; set; }
        public string? OtherCategory { get; set; }
        public double Price { get; set; }
        public string SellType { get; set; } //Auction or Fixed Price
        public string Author { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public string? Size { get; set; }
    }
}
