using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArtGallery.Data;
using ArtGallery.Models;
using ArtGallery.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace ArtGallery.Controllers
{
    public class SaleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly Random _random;
        private readonly UserManager<ApplicationUser> _userManager;

        public SaleController(UserManager<ApplicationUser> userManager,ApplicationDbContext context)
        {
            _context = context;
            _random = new Random();
            _userManager = userManager;
        }

        // GET: Sale
        public async Task<IActionResult> Index()
        {
            var sale = await _context.Sale.Include(x=>x.ArtWorks).SingleAsync();
            var arts = sale.ArtWorks?.ToList();
            
            SalesViewModel salesView = new SalesViewModel()
            {
                Sale = sale,
                ArtWorks = arts,
            };
            return View(salesView);
        }
        public async Task<IActionResult> SaleItems()
        {

            var sale = await _context.Sale.SingleAsync();
            var artWorks = _context.ArtWork.Where(x => x.DiscountedPrice != null);
            SalesViewModel salesView = new SalesViewModel()
            {
                Sale = sale,
                ArtWorks = artWorks.ToList(),
            };
            ViewBag.UserId = _userManager.GetUserId(User);
            ViewBag.Categories = await _context.Category.ToListAsync();
            return View(salesView);
        }
        public async Task<IActionResult> AllItems()
        {
            var sale = await _context.Sale.SingleAsync();
            var artWorks = _context.ArtWork.Where(x => x.SellType == "FixedPrice" && x.Sold != true && x.DiscountedPrice == null);
            SalesViewModel salesView = new SalesViewModel()
            {
                Sale = sale,
                ArtWorks = artWorks.ToList(),
            };
            return View(salesView);
        }
        public async Task<IActionResult> AddToSale(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var sale = await _context.Sale.Include(x=>x.ArtWorks).SingleAsync();
            var art = _context.ArtWork.FirstOrDefault(x=>x.ArtId == id);
            if (art == null)
            {
                return NotFound();
            }

            double discountPercentage = GetRandomDiscountPercentage();
            art.Discount = discountPercentage;
            art.DiscountedPrice = art.Price - (art.Price * discountPercentage / 100); 
            _context.ArtWork.Update(art);
            await _context.SaveChangesAsync();
            sale.ArtWorks?.Add(art);
            _context.Sale.Update(sale);
            await _context.SaveChangesAsync();
            return RedirectToAction("AllItems","Sale");
        }
        public async Task<IActionResult> RemoveFromSale(int? id)
        {
            var returnUrl = Request.Headers["Referer"].ToString();
            if (id == null)
            {
                return NotFound();
            }
            var sale = await _context.Sale.SingleAsync();
            var art = _context.ArtWork.FirstOrDefault(x => x.ArtId == id);
            if (art == null)
            {
                return NotFound();
            }
            sale?.ArtWorks?.Remove(art);
            art.Discount = null;
            art.DiscountedPrice = null;
            _context.ArtWork.Update(art);
            _context.Sale.Update(sale);
            await _context.SaveChangesAsync();
            return Redirect(returnUrl);
        }

        // GET: Sale/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sale
                .FirstOrDefaultAsync(m => m.SaleId == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // GET: Sale/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sale/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SaleId,SaleName,EndDate")] Sale sale)
        {
            if (ModelState.IsValid)
            {
                if (_context.Sale.Any())
                {
                    var existingSale = await _context.Sale.SingleAsync();
                    existingSale?.ArtWorks?.Clear();
                    _context.Sale.Remove(existingSale);
                    await _context.SaveChangesAsync();
                }
                _context.Add(sale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sale);
        }

        // GET: Sale/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sale.FindAsync(id);
            if (sale == null)
            {
                return NotFound();
            }
            return View(sale);
        }

        // POST: Sale/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SaleId,SaleName,EndDate")] Sale sale)
        {
            if (id != sale.SaleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaleExists(sale.SaleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sale);
        }

        // GET: Sale/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sale = await _context.Sale
                .FirstOrDefaultAsync(m => m.SaleId == id);
            if (sale == null)
            {
                return NotFound();
            }

            return View(sale);
        }

        // POST: Sale/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sale = await _context.Sale.FindAsync(id);
            if (sale != null)
            {
                _context.Sale.Remove(sale);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaleExists(int id)
        {
            return _context.Sale.Any(e => e.SaleId == id);
        }
        private double GetRandomDiscountPercentage()
        {
            return _random.Next(5, 31); // Generates a random number between 5 and 30 (inclusive)
        }
    }
}
