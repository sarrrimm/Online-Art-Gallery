using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using ArtGallery.Data;
using ArtGallery.Models;
using ArtGallery.ViewModels;
using Microsoft.VisualBasic;
using NuGet.Protocol.Core.Types;
using Microsoft.AspNetCore.Identity;
using ArtGallery.Helpers;
using Microsoft.CodeAnalysis.Elfie.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace ArtGallery.Controllers
{
    [Authorize]
    public class ArtWorksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment hostingEnvironment;

        public ArtWorksController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IWebHostEnvironment hc)
        {
            _userManager = userManager;
            _context = context;
            hostingEnvironment = hc;
        }

        public async Task<IActionResult> All()
        {
            var userExist = _userManager.GetUserId(User);
            if (userExist == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var artWorks = _context.ArtWork.Where(x=>x.OwnerId != userExist);
            ViewBag.Categories = _context.Category.Include(x => x.ArtWorks).ToList();
            return View(artWorks);
        }
        // GET: ArtWorks
        [Authorize(Roles = "Artist")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            
            return View(_context.ArtWork.Where(x => x.OwnerId == userId).OrderByDescending(x=>x.UploadDate));
        }

        // GET: ArtWorks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artWork = await _context.ArtWork.FirstOrDefaultAsync(m => m.ArtId == id);
            if (artWork == null)
            {
                return NotFound();
            }
            var auction = await _context.Auction.FirstOrDefaultAsync(m => m.AuctionId == artWork.AuctionId);
            if (auction != null)
            {
                ViewBag.RemTime = auction.RemainingTime.ToFriendlyString();
            }
            return View(artWork);
        }

        // GET: ArtWorks/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Category, "CategoryName", "CategoryName");
            ViewBag.Auctions = new SelectList(_context.Auction.Where(x=>x.EndDate >= DateTime.Now), "AuctionId", "AuctionName");
            ViewBag.BothEmptyMsg = "";
            return View();
        }

        // POST: ArtWorks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ArtWorkViewModel artWork)
        {
            string filename = "";
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(artWork.Category) && string.IsNullOrEmpty(artWork.OtherCategory))
                {
                    ViewBag.BothEmptyMsg = "Category field is required";
                    ViewBag.Categories = new SelectList(_context.Category, "CategoryName", "CategoryName", artWork.Category);
                    ViewBag.Auctions = new SelectList(_context.Auction.Where(x => x.EndDate >= DateTime.Now), "AuctionId", "AuctionName", artWork.AuctionId);
                    return View(artWork);
                }
                if (artWork.SellType == "Auction" && artWork.AuctionId == null)
                {
                    ViewBag.AuctionEmptyMsg = "Auction must be selected";
                    ViewBag.Categories = new SelectList(_context.Category, "CategoryName", "CategoryName", artWork.Category);
                    ViewBag.Auctions = new SelectList(_context.Auction.Where(x => x.EndDate >= DateTime.Now), "AuctionId", "AuctionName", artWork.AuctionId);
                    ViewBag.Auctions = new SelectList(_context.Auction.Where(x => x.EndDate >= DateTime.Now), "AuctionId", "AuctionName", artWork.AuctionId);
                    return View(artWork);
                }
                if (artWork.Image != null)
                {
                    //string folderPath = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    string folderPath = Path.Combine(hostingEnvironment.ContentRootPath, "images");
                    Console.WriteLine(folderPath);
                    filename = Guid.NewGuid().ToString() + "_" + artWork.Image.FileName;
                    string filepath = Path.Combine(folderPath, filename);
                    artWork.Image.CopyTo(new FileStream(filepath, FileMode.Create));
                }
                var userId = _userManager.GetUserId(User);
                if (!string.IsNullOrEmpty(artWork.OtherCategory))
                {
                    artWork.Category = artWork.OtherCategory;
                }
                ArtWork art = new ArtWork()
                {
                    Title = artWork.Title,
                    Description = artWork.Description,
                    imagePath = filename,
                    Category = artWork.Category,
                    Price = artWork.Price,
                    SoldPrice = 0,
                    AuctionId = artWork.AuctionId,
                    SellType = artWork.SellType,
                    Size = artWork.Size,
                    Sold = false,
                    Author = artWork.Author,
                    OwnerId = userId,
                    ReleaseDate = artWork.ReleaseDate,
                    UploadDate = DateTime.Now,
                };
                if (!string.IsNullOrEmpty(artWork.OtherCategory))
                {
                    Category category = new Category()
                    {
                        CategoryName = artWork.OtherCategory
                    };
                    category.ArtWorks?.Add(art);
                    _context.Category.Add(category);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var cat = _context.Category.FirstOrDefault(x=>x.CategoryName == artWork.Category);
                    cat.ArtWorks?.Add(art);
                }
                await _context.SaveChangesAsync();
                if (artWork.SellType == "Auction")
                {
                    var auction = await _context.Auction.FirstOrDefaultAsync(m => m.AuctionId == artWork.AuctionId);
                    auction.ArtWorks?.Add(art);
                    await _context.SaveChangesAsync();
                }
                _context.Add(art);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.Category, "CategoryName", "CategoryName");
            ViewBag.Auctions = new SelectList(_context.Auction.Where(x => x.EndDate >= DateTime.Now), "AuctionId", "AuctionName");
            return View(artWork);
        }
        //AddToAuction
        public async Task<IActionResult> AddToAuction(int? id)
        {
            var artWork = await _context.ArtWork.FindAsync(id);
            if (artWork == null)
            {
                return NotFound();
            }
            if (artWork.SellType == "Fixed Price")
            {
                return RedirectToAction(nameof(Index));
            }
            var artWorkView = new ArtWorkViewModel()
            {
                Title = artWork.Title,
                Description = artWork.Description,
                AuctionId = artWork.AuctionId,
                Category = artWork.Category,
                Size = artWork.Size,
                Price = artWork.Price,
                SellType = artWork.SellType,
                Author = artWork.Author,
                ReleaseDate = artWork.ReleaseDate,
            };
            ViewBag.ImagePath = artWork.imagePath;
            ViewBag.Auctions = new SelectList(_context.Auction.Where(x => x.EndDate >= DateTime.Now), "AuctionId", "AuctionName");
            return View(artWorkView);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToAuction(int? id, ArtWorkViewModel artWork)
        {
            var art = await _context.ArtWork.FindAsync(id);
            if (artWork.AuctionId == null)
            {
                ViewBag.ErrMsg = "Auction field is required";
                ViewBag.Auctions = new SelectList(_context.Auction.Where(x => x.EndDate >= DateTime.Now), "AuctionId", "AuctionName");
                ViewBag.ImagePath = art?.imagePath;
                return View(artWork);
            }
            if (art.Bids != null)
            {
                art.Bids.Clear();
                art.MaxBid = 0.00;
            }
            var sale = await _context.Sale.Include(x => x.ArtWorks).SingleAsync();
            if (sale != null && sale.ArtWorks.Contains(art))
            {
                sale.ArtWorks.Remove(art);
                _context.Sale.Update(sale);
                await _context.SaveChangesAsync();
            }
            art.AuctionId = artWork?.AuctionId;
            art.SellType = "Auction";
            art.Discount = null;
            art.DiscountedPrice = null;
            _context.ArtWork.Update(art);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ChangeAuction(int? id)
        {
            var artWork = await _context.ArtWork.FindAsync(id);
            if (artWork == null)
            {
                return NotFound();
            }
            if (artWork.SellType == "FixedPrice")
            {
                return RedirectToAction(nameof(Index));
            }
            var artWorkView = new ArtWorkViewModel()
            {
                Title = artWork.Title,
                Description = artWork.Description,
                AuctionId = artWork.AuctionId,
                Category = artWork.Category,
                Size = artWork.Size,
                Price = artWork.Price,
                SellType = artWork.SellType,
                Author = artWork.Author,
                ReleaseDate = artWork.ReleaseDate,
            };
            ViewBag.ImagePath = artWork.imagePath;
            ViewBag.Auctions = new SelectList(_context.Auction.Where(x => x.EndDate >= DateTime.Now), "AuctionId", "AuctionName", artWork.AuctionId);
            return View(artWorkView);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeAuction(int? id, ArtWorkViewModel artWork)
        {
            var art = await _context.ArtWork.FindAsync(id);
            if (artWork.AuctionId == art.AuctionId)
            {
                ViewBag.ForgotMsg = "You forgot to change the auction";
                return View(art);
            }
            art.AuctionId = artWork?.AuctionId;
            art.Bids?.Clear();
            art.MaxBid = 0.00;
            _context.ArtWork.Update(art);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
            //return View(artWork);
        }

        public async Task<IActionResult> RemoveFromAuction(int? id)
        {
            
            var art = await _context.ArtWork.FindAsync(id);
            art.Bids?.Clear();
            art.MaxBid = 0.00;
            art.SellType = "FixedPrice";
            art.AuctionId = null;
            _context.ArtWork.Update(art);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RemoveFromAuction(int? id, ArtWorkViewModel artWork)
        //{
        //    var art = await _context.ArtWork.FindAsync(id);
        //    art.Bids?.Clear();
        //    art.MaxBid = 0.00;
        //    art.SellType = "FixedPrice";
        //    art.AuctionId = null;
        //    _context.ArtWork.Update(art);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        // GET: ArtWorks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artWork = await _context.ArtWork.FindAsync(id);
            if (artWork == null)
            {
                return NotFound();
            }
            var art = new ArtWorkViewModel()
            {
                Title = artWork.Title,
                Description = artWork.Description,
                AuctionId = artWork.AuctionId,
                Size = artWork.Size,
                Category = artWork.Category,
                Price = artWork.Price,
                SellType = artWork.SellType,
                Author = artWork.Author,
                ReleaseDate = artWork.ReleaseDate,
            };
            ViewBag.Categories = new SelectList(_context.Category, "CategoryName", "CategoryName", artWork.Category);
            ViewBag.CurrImg = artWork.imagePath;
            return View(art);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ArtWorkViewModel artWork)
        {
            if (!ModelState.IsValid)
            {
                var errs = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var err in errs)
                {
                    Console.WriteLine(err.ErrorMessage);
                }
                return View(artWork);
            }

            var art = await _context.ArtWork.FirstOrDefaultAsync(x => x.ArtId == id);
            if (art == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {

            string filename = art.imagePath;
            string folderPath = Path.Combine(hostingEnvironment.ContentRootPath, "Images");

            if (artWork.Image != null)
            {
                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(art.imagePath))
                {
                    var oldImagePath = Path.Combine(folderPath, art.imagePath);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        try
                        {
                            System.IO.File.SetAttributes(oldImagePath, FileAttributes.Normal);
                            System.IO.File.Delete(oldImagePath);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", $"Unable to delete the old image file. Please check the file permissions. Error: {ex.Message}");
                                ViewBag.CurrImg = art.imagePath;
                                ViewBag.Categories = new SelectList(_context.Category, "CategoryName", "CategoryName", artWork.Category);
                                return View(artWork);
                        }
                    }
                }

                // Save the new image
                filename = Guid.NewGuid().ToString() + "_" + artWork.Image.FileName;
                string filepath = Path.Combine(folderPath, filename);
                try
                {
                    using (var fileStream = new FileStream(filepath, FileMode.Create))
                    {
                        await artWork.Image.CopyToAsync(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Unable to save the new image file. Please check the file permissions. Error: {ex.Message}");
                        ViewBag.CurrImg = art.imagePath;
                        ViewBag.Categories = new SelectList(_context.Category, "CategoryName", "CategoryName", artWork.Category);
                        return View(artWork);
                }
            }

                if (string.IsNullOrEmpty(artWork.Category) && string.IsNullOrEmpty(artWork.OtherCategory))
                {
                    ViewBag.BothEmptyMsg = "Category field is required";
                    ViewBag.Categories = new SelectList(_context.Category, "CategoryName", "CategoryName", artWork.Category);
                    ViewBag.CurrImg = art.imagePath;
                    return View(artWork);
                }

                var Categories = await _context.Category.ToListAsync();
                var catExists = Categories.FirstOrDefault(x => x.CategoryName == artWork.OtherCategory);
                if (!string.IsNullOrEmpty(artWork.OtherCategory))
                {
                    artWork.Category = artWork.OtherCategory;
                }
                // Update art properties
                art.Title = artWork.Title;
                art.Description = artWork.Description;
                art.imagePath = filename;
                art.Category = artWork.Category;
                art.Price = artWork.Price;
                art.SellType = artWork.SellType;
                art.Size = artWork.Size;
                art.Author = artWork.Author;
                art.ReleaseDate = artWork.ReleaseDate;

                if (!string.IsNullOrEmpty(artWork.OtherCategory))
                {
                    Category category = new Category()
                    {
                        CategoryName = artWork.OtherCategory
                    };
                    category.ArtWorks?.Add(art);
                    _context.Category.Add(category);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var cat = _context.Category.FirstOrDefault(x => x.CategoryName == artWork.Category);
                    cat.ArtWorks?.Add(art);
                }
                _context.Update(art);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Populate view data in case of error
            ViewBag.Categories = new SelectList(_context.Category, "CategoryName", "CategoryName", artWork.Category);
            ViewBag.Auctions = new SelectList(_context.Auction.Where(x => x.EndDate >= DateTime.Now), "AuctionId", "AuctionName");
            ViewBag.CurrImg = art.imagePath;
            return View(artWork);
    
        }

        // GET: ArtWorks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artWork = await _context.ArtWork
                .FirstOrDefaultAsync(m => m.ArtId == id);
            if (artWork == null)
            {
                return NotFound();
            }

            return View(artWork);
        }

        // POST: ArtWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artWork = await _context.ArtWork.FindAsync(id);
            if (artWork != null)
            {
                _context.ArtWork.Remove(artWork);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtWorkExists(int id)
        {
            return _context.ArtWork.Any(e => e.ArtId == id);
        }
    }
}
