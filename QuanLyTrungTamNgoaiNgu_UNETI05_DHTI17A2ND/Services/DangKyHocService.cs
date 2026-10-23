// Họ và tên: Nguyễn Hoàng Huy
// Mã sinh viên: [...]
// Nội dung thực hiện: Service xử lý logic đếm đăng ký và kiểm tra điều kiện đăng ký lớp học

using System.Linq;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Services
{
    public interface IDangKyHocService
    {
        int DemDaXacNhan(int maLop);
        (bool HopLe, string ThongBao) KiemTraDieuKienDangKy(int maHocVien, int maLop);
    }

    public class DangKyHocService : IDangKyHocService
    {
        private readonly AppDbContext _context;

        public DangKyHocService(AppDbContext context)
        {
            _context = context;
        }

        // 1. Đếm số DangKyHoc của lớp có TrangThai = "Đã xác nhận" (Sinh viên 4 dùng lại)
        public int DemDaXacNhan(int maLop)
        {
            return _context.DangKyHocs
                .Count(dk => dk.MaLop == maLop && dk.TrangThai == "Đã xác nhận");
        }

        // 2. Kiểm tra các điều kiện đăng ký
        public (bool HopLe, string ThongBao) KiemTraDieuKienDangKy(int maHocVien, int maLop)
        {
            // Kiểm tra 1: Học viên tồn tại
            var hocVien = _context.HocViens.FirstOrDefault(hv => hv.MaHocVien == maHocVien);
            if (hocVien == null)
            {
                return (false, "Thông tin học viên không tồn tại.");
            }

            // Kiểm tra 2: Lớp học tồn tại
            var lopHoc = _context.LopHocs.FirstOrDefault(l => l.MaLop == maLop);
            if (lopHoc == null)
            {
                return (false, "Lớp học không tồn tại.");
            }

            // Kiểm tra 3: Lớp đang "Đang tuyển sinh"
            if (lopHoc.TrangThai != "Đang tuyển sinh")
            {
                return (false, "Lớp học hiện tại không trong thời gian tuyển sinh.");
            }

            // Kiểm tra 4: Học viên chưa có đăng ký còn hiệu lực (khác "Đã hủy") ở lớp này
            bool daDangKy = _context.DangKyHocs.Any(dk =>
                dk.MaHocVien == maHocVien &&
                dk.MaLop == maLop &&
                dk.TrangThai != "Đã hủy");

            if (daDangKy)
            {
                return (false, "Bạn đã đăng ký lớp học này trước đó.");
            }

            // Kiểm tra 5: DemDaXacNhan(maLop) < SoLuongToiDa
            int soDaXacNhan = DemDaXacNhan(maLop);
            if (soDaXacNhan >= lopHoc.SoLuongToiDa)
            {
                return (false, "Lớp học đã đủ số lượng học viên tối đa.");
            }

            return (true, "Hợp lệ.");
        }
    }
}