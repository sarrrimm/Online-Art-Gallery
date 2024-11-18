using Humanizer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtGallery.Models
{
    public class Auction
    {
        [Key]
        public int AuctionId { get; set; }
        public string AuctionName { get; set; }
        public string AuctionLocation { get; set; }
        public List<ArtWork>? ArtWorks { get; set; }
        public DateOnly StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        [NotMapped]
        public TimeSpan RemainingTime
        {
            get
            {
                var remaining = EndDate.Subtract(DateTime.Now);
                return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero; // Ensure no negative values
            }
        }

        [NotMapped]
        public string FormattedRemainingTime
        {
            get
            {
                return RemainingTime.ToString(@"hh\:mm\:ss");
            }
        }
    }
}
