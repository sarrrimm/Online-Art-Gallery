using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ArtGallery.Data;
using ArtGallery.Models;
using SQLitePCL;

namespace ArtGallery.Services
{
    public class CartEmptyService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(10);

        public CartEmptyService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CreateCartAsync();
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CreateCartAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var users = await context.Users.ToListAsync();
                var carts = await context.Cart.Include(x=>x.cartItems).ToListAsync();


                foreach (var cart in carts)
                {
                    foreach (var item in cart.cartItems)
                    {
                        var art = await context.ArtWork.FirstOrDefaultAsync(x=>x.ArtId == item.ArtId);
                        if (art.Sold || art.SellType == "Auction")
                        {
                            context.CartItems.Remove(item);
                        }
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
