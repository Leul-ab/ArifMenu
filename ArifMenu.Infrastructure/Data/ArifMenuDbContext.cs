using ArifMenu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArifMenu.Infrastructure.Data
{
    public class ArifMenuDbContext : DbContext
    {
        public ArifMenuDbContext(DbContextOptions<ArifMenuDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Merchant> Merchants { get; set; }

        public DbSet<MenuCategory> MenuCategories { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MerchantQrLink> MerchantQrLinks { get; set; }
        public DbSet<QrScanLog> QrScanLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Merchant)
                .WithOne(m => m.User)
                .HasForeignKey<Merchant>(m => m.UserId);


            modelBuilder.Entity<MenuCategory>()
       .HasOne(mc => mc.Merchant)
       .WithMany(m => m.MenuCategories)
       .HasForeignKey(mc => mc.MerchantId);

            modelBuilder.Entity<Menu>()
                .HasOne(m => m.Merchant)
                .WithMany(m => m.Menus)
                .HasForeignKey(m => m.MerchantId);

            modelBuilder.Entity<Menu>()
                .HasOne(m => m.Category)
                .WithMany(c => c.Menus)
                .HasForeignKey(m => m.CategoryId);

            modelBuilder.Entity<MerchantQrLink>()
               .HasIndex(x => x.QrSlug)
               .IsUnique();

            modelBuilder.Entity<MerchantQrLink>()
                .HasOne(x => x.Merchant)
                .WithMany()
                .HasForeignKey(x => x.MerchantId);



            modelBuilder.Entity<QrScanLog>()
              .HasOne(x => x.Merchant)
              .WithMany()
              .HasForeignKey(x => x.MerchantId);

        }
        
    }
}
