using ArtGallery.Models;
namespace ArtGallery.ViewModels
{
    public class HomePageViewModel
    {
        public List<string> CarouselImages { get; set; }
        public List<ArtWork>? FeaturedArt { get; set; }
    }
}
