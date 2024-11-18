using System.ComponentModel.DataAnnotations;

namespace ArtGallery.Models
{
    public class Bids
    {
        [Key]
        public int BidId { get; set; }
        public int ArtWorkId { get; set; }
        public string BidderId { get; set; }
        public double BidAmount { get; set; }
    }
}
