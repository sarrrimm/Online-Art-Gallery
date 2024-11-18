using ArtGallery.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ArtGallery.Services
{
    public class CartCountService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartCountService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return 0;
            }

            var cart = await _context.Cart.Include(x => x.cartItems).FirstOrDefaultAsync(x=>x.UserId == userId);
            var count = 0;
            if (cart.cartItems != null)
            {
                count = cart.cartItems.Count();
            }
            return count;
        }
    }
}
