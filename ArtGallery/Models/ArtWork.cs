using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace ArtGallery.Models
{
    public class ArtWork
    {
        [Key]
        public int ArtId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? Size { get; set; }
        public string imagePath { get; set; }
        public string? Category { get; set; }
        public double Price { get; set; }
        public double? Discount { get; set; }
        public double? DiscountedPrice { get; set; }
        public double? SoldPrice { get; set; }
        public string? SoldTo { get; set; }
        public bool Sold { get; set; } = false;
        public int? AuctionId { get; set; }
        public List<Bids>? Bids { get; set; }
        public double? MaxBid { get; set; } = 0.00;
        public string SellType { get; set; } //Auction or Fixed Price
        public string Author { get; set; }
        public string? OwnerId { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public DateTime UploadDate { get; set; }

        public ArtWork (){
            MaxBid = 0.00;       
        }
    }
}
