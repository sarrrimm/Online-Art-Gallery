using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ArtGallery.Models;
using ArtGallery.ViewModels;

namespace ArtGallery.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        public DbSet<ArtGallery.Models.Category> Category { get; set; } = default!;
        public DbSet<ArtGallery.Models.ArtWork> ArtWork { get; set; } = default!;
        public DbSet<ArtGallery.Models.Order> Order { get; set; } = default!;
        public DbSet<ArtGallery.Models.Auction> Auction { get; set; } = default!;
        public DbSet<ArtGallery.Models.Bids> Bids { get; set; } = default!;
        public DbSet<ArtGallery.Models.Cart> Cart { get; set; } = default!;
        public DbSet<ArtGallery.Models.CartItem> CartItems { get; set; } = default!;
        public DbSet<ArtGallery.Models.Sale> Sale { get; set; } = default!;

    }
}
