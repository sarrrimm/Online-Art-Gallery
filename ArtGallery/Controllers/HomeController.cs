using ArtGallery.Data;
using ArtGallery.Models;
using System.Linq;
using ArtGallery.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ArtGallery.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var carouselImages = new List<string>
                {
                    "car1.jpg",
                    "car2.jpg",
                    "car3.jpg"
                };

            var artSold = _context.ArtWork.Where(x => x.Sold != true);
            var arts = artSold.Where(x => x.imagePath != null);
            var featuredArt = arts.AsEnumerable().OrderBy(a => Guid.NewGuid()).Take(3).ToList();

            var viewModel = new HomePageViewModel
            {
                CarouselImages = carouselImages,
                FeaturedArt = featuredArt
            };

            return View(viewModel);
        }

        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
