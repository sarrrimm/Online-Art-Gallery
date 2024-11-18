using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ArtGallery.Data;
using ArtGallery.Models;
using System.Security.AccessControl;
using Microsoft.AspNetCore.Identity;
using ArtGallery.ViewModels;
using System.Numerics;

namespace ArtGallery.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(UserManager<ApplicationUser> userManager,ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cart
     
        public async Task<IActionResult> Index(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            double? total = 0.00;
            var cart = _context.Cart.Include(x=>x.cartItems)
                .FirstOrDefault(m => m.UserId == id);
            if (cart == null)
            {
                return NotFound();
            }
            var userItems = _context.CartItems.Where(x => x.CartId == cart.CartId);
            foreach (var item in userItems)
            {
                total += item.Price;
            }
            ViewBag.CartId = cart.CartId;
            ViewBag.Total = total;
            return View(userItems);
        }

        public async Task<IActionResult> Create(int? id)
        {
            var returnUrl = Request.Headers["Referer"].ToString();
            var art = await _context.ArtWork.FindAsync(id);
            var userId = _userManager.GetUserId(User);
            var cart = await _context.Cart.Include(x=>x.cartItems).FirstOrDefaultAsync(m => m.UserId == userId);
            double? price = 0;
            if (art.DiscountedPrice != null)
            {
                price = art.DiscountedPrice;
                ViewBag.Discount = art.Discount;
            }
            else
            {
                price = art.Price;
            }
            if (!cart.cartItems.Any(x => x.ArtId == id))
            {
                CartItem item = new CartItem()
                {
                    CartId = cart.CartId,
                    ArtId = art.ArtId,
                    Title = art.Title,
                    Price = price,
                    Author = art.Author,
                    ImagePath = art.imagePath,
                };
                _context.CartItems.Add(item);
                await _context.SaveChangesAsync();
            }
            return Redirect(returnUrl);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var item = await _context.CartItems.FindAsync(id);
            if (item != null)
            {
                _context.CartItems.Remove(item);
            }

            await _context.SaveChangesAsync();
            return Redirect("/Cart/Index/"+userId);
        }
        public async Task<IActionResult> Empty(string id)
        {
            var userCart = _context.Cart.FirstOrDefault(x=> x.UserId == id);
            var CartId = userCart.CartId;
            var ItemsToRemove = _context.CartItems.Where(x=> x.CartId == CartId);
            foreach (var item in ItemsToRemove)
            {
                _context.CartItems.Remove(item);
            }
            await _context.SaveChangesAsync();
            return Redirect("/Cart/Index/" + id);
        }
        public async Task<IActionResult> Checkout(int id)
        {
            var cart = await _context.Cart.Include(x=>x.cartItems).FirstOrDefaultAsync(x=> x.CartId == id);
            var user = await _userManager.FindByIdAsync(cart.UserId);
            double? total = 0.00;
            var cartItems = cart.cartItems.ToList();
            if (cartItems == null)
            {
                return Redirect("/Cart/Index/" + user.Id);
            }
            if (cartItems != null)
            {
                foreach (var item in cartItems)
                {
                    total += item.Price;
                }
                CheckoutViewModel checkout = new CheckoutViewModel()
                {
                    UserId = cart.UserId,
                    Name = user.FirstName + user.LastName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = "",
                    TotalAmount = total,
                    GrandTotal = total + 20
                };
                ViewBag.Items = cartItems;
                return View(checkout);
            }
            else
            {
                return Redirect("/Cart/Index/" + user.Id);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(int id, CheckoutViewModel checkout)
        {
            var cart = await _context.Cart.Include(x=>x.cartItems).FirstOrDefaultAsync(x=>x.CartId == id);
            if (cart == null)
            {
                return View(checkout);
            }
            var cartItems = await _context.CartItems.ToListAsync();
            Order order = new Order()
            {
                Items = cartItems,
                OrderDate = DateTime.Now,
                CustomerId = cart.UserId,
                CustomerName = checkout.Name,
                TotalAmount = checkout.GrandTotal,
                Address = checkout.Address,
            };
            _context.Order.Add(order);
            foreach (var item in cartItems)
            {
                var art = _context.ArtWork.FirstOrDefault(x=>x.ArtId == item.ArtId);
                art.Sold = true;
                art.SoldTo = cart.UserId;
                art.SoldPrice = item.Price;
                _context.ArtWork.Update(art);
                _context.CartItems.Remove(item);
            }
            await _context.SaveChangesAsync();
            return Redirect("/Cart/Index/" + cart.UserId);
        }
        public async Task<IActionResult> BuyNow(int id)
        {
            var art = _context.ArtWork.FirstOrDefault(x=>x.ArtId == id);
            if (art == null) return NotFound();
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            double? price = 0;
            if (art.DiscountedPrice != null)
            {
                 price = art.DiscountedPrice;
                 ViewBag.Discount = art.Discount;
            }
            else
            {
                 price = art.Price;
            }
            CheckoutViewModel checkout = new CheckoutViewModel()
            {
                UserId = art.OwnerId,
                Name = user.FirstName + user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = "",
                TotalAmount = price,
                GrandTotal = price + 20
            };
            ViewBag.Title = art.Title; 
            ViewBag.ImagePath = art.imagePath;
            ViewBag.Author = art.Author;
            
            return View(checkout);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BuyNow(int id, CheckoutViewModel checkout)
        {
            var art = _context.ArtWork.FirstOrDefault(x=>x.ArtId == id);
            var userId = _userManager.GetUserId(User);
            var cart = _context.Cart.Include(x => x.cartItems).FirstOrDefault(x => x.UserId == userId);
            double? price = 0;
            if (cart == null)
            {
                return View(checkout);
            }
            if (art.DiscountedPrice != null)
            {
                price = art.DiscountedPrice;
                ViewBag.Discount = art.Discount;
            }
            else
            {
                price = art.Price;
            }
            CartItem cartItem = new CartItem()
            {
                CartId = cart.CartId,
                ArtId = art.ArtId,
                Title = art.Title,
                Price = price,
                Author = art.Author,
                ImagePath = art.imagePath,
            };
            Order order = new Order()
            {
                OrderDate = DateTime.Now,
                CustomerId = cart.UserId,
                CustomerName = checkout.Name,
                TotalAmount = checkout.GrandTotal,
                Address = checkout.Address,
            };
            order?.Items?.Add(cartItem);
            _context.Order.Add(order);
            art.Sold = true;
            art.SoldTo = cart.UserId;
            art.SoldPrice = price;  
            _context.ArtWork.Update(art);
            
            await _context.SaveChangesAsync();
            return Redirect("/Home/Index/");
        }
        private bool CartExists(int id)
        {
            return _context.Cart.Any(e => e.CartId == id);
        }
    }
}
