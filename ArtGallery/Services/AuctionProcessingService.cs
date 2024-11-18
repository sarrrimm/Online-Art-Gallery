using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ArtGallery.Data;

namespace ArtGallery.Services
{
    public class AuctionProcessingService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30); // Check every minute

        public AuctionProcessingService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessEndedAuctionsAsync();
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task ProcessEndedAuctionsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var endedAuctions = await context.Auction.Include(a => a.ArtWorks).ThenInclude(aw => aw.Bids)
                    .Where(a => a.EndDate <= DateTime.Now)
                    .ToListAsync();

                foreach (var auction in endedAuctions)
                {
                    foreach (var artWork in auction.ArtWorks)
                    {
                        if (artWork.Bids != null && artWork.Bids.Any())
                        {
                            var highestBid = artWork.Bids.OrderByDescending(b => b.BidAmount).First();
                            artWork.SoldTo = highestBid.BidderId;
                            artWork.SoldPrice = highestBid.BidAmount;
                            artWork.Sold = true;
                        }
                        else
                        {
                            artWork.Bids?.Clear();
                            artWork.SellType = "FixedPrice";
                            artWork.AuctionId = null;
                        }
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
