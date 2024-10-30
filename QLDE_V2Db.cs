using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLDE_V2.Class;


namespace QLDE_V2
{
    public class QLDE_V2Db : DbContext
    {
        public DbSet<Chuong> Chuongtb { get; set; }
        public DbSet<Bai> Baitb { get; set; }
        public DbSet<CauHoi> CauHoitb { get; set; }
        public DbSet<KhoDe> KhoDetb { get; set; }
        public DbSet<CauHoiDe> CauHoiDetb { get; set; }
        public DbSet<KieuCauHoi> KieuCauhoitb { get; set; }
        public DbSet<MucDoCauHoi> MucDoCauHoitb { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlite("Data Source = test67.sqlite");
        }

        /// <summary>
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Bai>()
                .HasOne(b => b.Chuong)
                .WithMany(c => c.BaiS)
                .HasForeignKey(b => b.ChuongIdChuong)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CauHoi>()
                .HasOne(ch => ch.MucDoCauHoi)
                .WithMany(b => b.CauHoiS)
                .HasForeignKey(ch => ch.MucDoCauHoiIdMDCH)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CauHoi>()
                .HasOne(ch => ch.KieuCauHoi)
                 .WithMany(b => b.CauHoiS)
                .HasForeignKey(ch => ch.KieuCauHoiIdKCH)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CauHoiDe>()
                .HasKey(dc => new { dc.IdDeCauHoi});

            modelBuilder.Entity<CauHoiDe>()
                .HasOne(dc => dc.KhoDe)
                .WithMany(md => md.CauHoiDeS)
                .HasForeignKey(dc => dc.DeIdDe)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CauHoiDe>()
                .HasOne(dc => dc.CauHoi)
                .WithMany(ch => ch.CauHoiDeS)
                .HasForeignKey(dc => dc.CauHoiIdCauHoi)
                .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
