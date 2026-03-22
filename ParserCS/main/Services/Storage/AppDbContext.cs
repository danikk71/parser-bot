using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using main.Models;
using Microsoft.EntityFrameworkCore;

namespace main.Services.Storage
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<PriceHistory> PriceHistories { get; set; } = null!;
        public DbSet<Favourite> Favourites { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PriceHistory>()
                .HasOne(ph => ph.Product)
                .WithMany(p => p.PriceHistories)
                .HasForeignKey(ph => ph.ProductId);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductURL)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasDiscriminator<ProductType>("Type")
                .HasValue<GPU>(ProductType.GPU)
                .HasValue<CPU>(ProductType.CPU)
                .HasValue<RAM>(ProductType.RAM)
                .HasValue<SSD>(ProductType.SSD)
                .HasValue<HDD>(ProductType.HDD)
                .HasValue<Motherboard>(ProductType.Motherboard);

            base.OnModelCreating(modelBuilder);
        }
    }
}   
