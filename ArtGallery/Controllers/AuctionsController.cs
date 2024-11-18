using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArtGallery.Data;
using ArtGallery.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ArtGallery.Controllers
{
    [Authorize]
    public class AuctionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuctionsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Auctions
        public async Task<IActionResult> Index()
        {
            return View(await _context.Auction.OrderByDescending(x=>x.EndDate).ToListAsync());
        }

        public async Task<IActionResult> BidCreate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artWork = await _context.ArtWork.Include(a => a.Bids).FirstOrDefaultAsync(a => a.ArtId == id);
            if (artWork == null)
            {
                return NotFound();
            }
            if (artWork.SellType == "FixedPrice")
            {
                return RedirectToAction(nameof(Index));
            }
            double? max = artWork.Bids?.Any() == true ? artWork.Bids.Max(b => b.BidAmount) : 0;

            ViewBag.MaxBid = max;
            ViewBag.ArtWork = artWork;
            ViewBag.Auction = _context.Auction.FirstOrDefault(x => x.AuctionId == artWork.AuctionId);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            ViewBag.UserFullName = $"{user.FirstName} {user.LastName}";

            var userId = _userManager.GetUserId(User);
            Bids bid = new Bids()
            {
                ArtWorkId = artWork.ArtId,
                BidderId = userId,
                BidAmount = 0,
            };
            return View(bid);
        }


        [HttpPost]
        public async Task<IActionResult> BidCreate(int? id, Bids bid)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var artWork = await _context.ArtWork.Include(a => a.Bids).FirstOrDefaultAsync(x => x.ArtId == id);
                if (artWork == null)
                {
                    return NotFound();
                }

                double? max = artWork.Bids?.Any() == true ? artWork.Bids.Max(b => b.BidAmount) : 0;
                if (bid.BidAmount <= max)
                {
                    ViewBag.MaxBid = max;
                    ViewBag.ArtWork = artWork;
                    ViewBag.Auction = _context.Auction.FirstOrDefault(x => x.AuctionId == artWork.AuctionId);
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        return Unauthorized();
                    }
                    ViewBag.UserFullName = $"{user.FirstName} {user.LastName}";
                    ViewBag.GreaterMsg = "The bidding amount should be greater than the maximum bid.";
                    return View(bid);
                }

                Bids newBid = new Bids()
                {
                    ArtWorkId = artWork.ArtId,
                    BidderId = bid.BidderId,
                    BidAmount = bid.BidAmount,
                };
                
                _context.Bids.Add(newBid);
                await _context.SaveChangesAsync();

                artWork.MaxBid = newBid.BidAmount;
                artWork.Bids.Add(newBid);
                _context.ArtWork.Update(artWork);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(bid);
        }



        // GET: Auctions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auction
                .FirstOrDefaultAsync(m => m.AuctionId == id);
            if (auction == null)
            {
                return NotFound();
            }

            return View(auction);
        }

        // GET: Auctions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuctionId,AuctionName,AuctionLocation,StartDate,EndDate")] Auction auction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(auction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(auction);
        }

        // GET: Auctions/Edit/5
        [Authorize (Roles ="Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auction.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuctionId,AuctionName,AuctionLocation,StartDate,EndDate")] Auction auction)
        {
            if (id != auction.AuctionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuctionExists(auction.AuctionId))
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
            return View(auction);
        }

        // GET: Auctions/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auction = await _context.Auction
                .FirstOrDefaultAsync(m => m.AuctionId == id);
            if (auction == null)
            {
                return NotFound();
            }

            return View(auction);
        }

        // POST: Auctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auction = await _context.Auction.FindAsync(id);
            if (auction != null)
            {
                _context.Auction.Remove(auction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuctionExists(int id)
        {
            return _context.Auction.Any(e => e.AuctionId == id);
        }
    }
}
