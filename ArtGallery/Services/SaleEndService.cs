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
    public class SaleEndService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(2);

        public SaleEndService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await EndSale();
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task EndSale()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var users = await context.Users.ToListAsync();
                var sale = await context.Sale.Include(x=>x.ArtWorks).SingleAsync();
                if (sale !=null && sale.ArtWorks != null && sale.EndDate <= DateOnly.FromDateTime(DateTime.Now))
                {
                    var artInSale = sale.ArtWorks.ToList();
                    foreach (var art in artInSale)
                    {
                        sale.ArtWorks?.Remove(art);
                        art.Discount = null;
                        art.DiscountedPrice = null;
                        context.ArtWork.Update(art);
                    }
                }
                context.Sale.Update(sale);
                await context.SaveChangesAsync();
            }
        }
    }
}
