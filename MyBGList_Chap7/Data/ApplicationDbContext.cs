using System.Diagnostics.Metrics;
using Microsoft.EntityFrameworkCore;
using MyBGList.Models;

namespace MyBGList_Chap6.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Packet>()
                .HasOne(p => p.Order)  // Packet has one Order
                .WithMany(p => p.Packets)  // Order has many Packets
                .HasForeignKey(p => p.OrderId)  // Foreign key
                .IsRequired()  // Not nullable
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete if Order is deleted

            //Composite key in junction table
            modelBuilder.Entity<OrderBakingGood>()
                .HasKey(oc => new { oc.OrderId, oc.BakingGoodId });

            modelBuilder.Entity<OrderBakingGood>()
                .HasOne(obg => obg.Order)
                .WithMany(o => o.OrderBakingGoods)
                .HasForeignKey(oc => oc.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderBakingGood>()
                .HasOne(obg => obg.BakingGood)
                .WithMany(bg => bg.OrderBakingGoods)
                .HasForeignKey(oc => oc.BakingGoodId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BakingGoodBatch>()
                 .HasKey(bg => new { bg.BatchId, bg.BakingGoodId });

            modelBuilder.Entity<BakingGoodBatch>()
                .HasOne(bgb => bgb.Batch)
                .WithMany(b => b.BakingGoodBatches)
                .HasForeignKey(bg => bg.BatchId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BakingGoodBatch>()
                .HasOne(bgb => bgb.BakingGood)
                .WithMany(bg => bg.BakingGoodBatches)
                .HasForeignKey(bg => bg.BakingGoodId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BatchStock>()
                .HasKey(bd => new { bd.BatchId, bd.StockId });

            modelBuilder.Entity<BatchStock>()
                .HasOne(bs => bs.Batch)
                .WithMany(b => b.BatchStocks)
                .HasForeignKey(bs => bs.BatchId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BatchStock>()
                .HasOne(bs => bs.Stock)
                .WithMany(s => s.BatchStocks)
                .HasForeignKey(bs => bs.StockId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<Packet> Packets => Set<Packet>();
        public DbSet<BakingGood> BakingGoods => Set<BakingGood>();
        public DbSet<Batch> Batches => Set<Batch>();
        public DbSet<Stock> Stocks => Set<Stock>();
        public DbSet<OrderBakingGood> OrderBakingGoods => Set<OrderBakingGood>();
        public DbSet<BakingGoodBatch> BakingGoodBatches => Set<BakingGoodBatch>();
        public DbSet<BatchStock> BatchStocks => Set<BatchStock>();
    }
}
