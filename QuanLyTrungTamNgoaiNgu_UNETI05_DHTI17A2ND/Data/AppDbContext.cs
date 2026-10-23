// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: DbContext, cấu hình quan hệ bằng Fluent API
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<KhoaHoc> KhoaHocs { get; set; }
        public DbSet<LopHoc> LopHocs { get; set; }
        public DbSet<HocVien> HocViens { get; set; }
        public DbSet<DangKyHoc> DangKyHocs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Tên đăng nhập không được trùng (unique index ở mức CSDL)
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique();

            // TaiKhoan 1-1 HocVien (khóa ngoại nằm ở HocVien.MaTaiKhoan)
            modelBuilder.Entity<HocVien>()
                .HasOne(h => h.TaiKhoan)
                .WithOne(t => t.HocVien)
                .HasForeignKey<HocVien>(h => h.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            // KhoaHoc 1-n LopHoc
            modelBuilder.Entity<LopHoc>()
                .HasOne(l => l.KhoaHoc)
                .WithMany(k => k.DanhSachLop)
                .HasForeignKey(l => l.MaKhoaHoc)
                .OnDelete(DeleteBehavior.Restrict);

            // LopHoc 1-n DangKyHoc
            modelBuilder.Entity<DangKyHoc>()
                .HasOne(d => d.LopHoc)
                .WithMany(l => l.DanhSachDangKy)
                .HasForeignKey(d => d.MaLop)
                .OnDelete(DeleteBehavior.Restrict);

            // HocVien 1-n DangKyHoc
            modelBuilder.Entity<DangKyHoc>()
                .HasOne(d => d.HocVien)
                .WithMany(h => h.DanhSachDangKy)
                .HasForeignKey(d => d.MaHocVien)
                .OnDelete(DeleteBehavior.Restrict);

            // Index giúp kiểm tra "đã đăng ký lớp này chưa" nhanh hơn
            modelBuilder.Entity<DangKyHoc>()
                .HasIndex(d => new { d.MaHocVien, d.MaLop });
        }
    }
}