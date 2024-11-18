namespace ArtGallery.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? CategoryDescription { get; set; }
        public List<ArtWork>? ArtWorks {get; set;} 
    }
}
