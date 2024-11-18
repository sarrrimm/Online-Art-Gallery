using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ArtGallery.Data;
using ArtGallery.Models;

namespace ArtGallery.Services
{
    public class CartCreationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);

        public CartCreationService(IServiceProvider serviceProvider)
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
                var carts = await context.Cart.ToListAsync();

                
                    foreach (var user in users)
                    {
                    if (carts.Any(x => x.UserId == user.Id))
                    {
                        Console.WriteLine("Cart Already Exists");
                    }
                    else
                    {
                        Cart cart = new Cart()
                        {
                            UserId = user.Id,
                        };
                        context.Cart.Add(cart);
                    }
                   }
                await context.SaveChangesAsync();
            }
        }
    }
}
