namespace ArtGallery.Models
{
    public class Sale
    {
        public int SaleId { get; set; }
        public string SaleName { get; set; }
        public DateOnly EndDate { get; set; }
        public List<ArtWork>? ArtWorks { get; set; }
    }
}
