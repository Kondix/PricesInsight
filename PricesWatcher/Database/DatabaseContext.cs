using Microsoft.EntityFrameworkCore;
using PricesWatcher.Database.Domain;
using System;

namespace PricesWatcher.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<TripAdvisorHotelRating> TripAdvisorHotelRatings { get; set; }
        public DbSet<Price> Prices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=master;Trusted_Connection=True;", builder =>
            {
                builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hotel>().ToTable("Hotels");
            modelBuilder.Entity<Offer>().ToTable("Offers");
            modelBuilder.Entity<TripAdvisorHotelRating>().ToTable("TripAdvisorHotelRatings");
            modelBuilder.Entity<Price>().ToTable("Prices");
        }
    }
}
