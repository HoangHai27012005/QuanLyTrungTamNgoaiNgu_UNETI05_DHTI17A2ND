// Họ và tên: Nguyễn Hoàng Huy
// Mã sinh viên: 23203100054
// Nội dung thực hiện: Service nghiệp vụ đăng ký lớp (đếm đã xác nhận, kiểm tra điều kiện đăng ký)

using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Services;

public class DangKyHocService
{
    private readonly AppDbContext _db;

    public DangKyHocService(AppDbContext db)
    {
        _db = db;
    }

    // Đếm số đăng ký "Đã xác nhận" của một lớp (SV4 sẽ gọi hàm này)
    public int DemDaXacNhan(int maLop)
    {
        return _db.DangKyHocs.Count(d => d.MaLop == maLop
                                      && d.TrangThai == TrangThaiDangKyHoc.DaXacNhan);
    }

    // Kiểm tra lần lượt từng điều kiện, mỗi lỗi một thông báo riêng
    public (bool HopLe, string ThongBao) KiemTraDieuKienDangKy(int maHocVien, int maLop)
    {
        // 1. Học viên phải tồn tại
        bool coHocVien = _db.HocViens.Any(h => h.MaHocVien == maHocVien);
        if (!coHocVien)
            return (false, "Không tìm thấy thông tin học viên.");

        // 2. Lớp phải tồn tại
        var lop = _db.LopHocs.AsNoTracking().FirstOrDefault(l => l.MaLop == maLop);
        if (lop == null)
            return (false, "Lớp học không tồn tại.");

        // 3. Lớp phải đang tuyển sinh
        if (lop.TrangThai != TrangThaiLopHoc.DangTuyenSinh)
            return (false, "Lớp này hiện không nhận đăng ký (trạng thái: " + lop.TrangThai + ").");

        // 4. Chưa có đăng ký còn hiệu lực (khác "Đã hủy") ở lớp này
        bool daDangKy = _db.DangKyHocs.Any(d => d.MaHocVien == maHocVien
                                             && d.MaLop == maLop
                                             && d.TrangThai != TrangThaiDangKyHoc.DaHuy);
        if (daDangKy)
            return (false, "Bạn đã đăng ký lớp này rồi.");

        // 5. Lớp còn chỗ
        if (DemDaXacNhan(maLop) >= lop.SoLuongToiDa)
            return (false, "Lớp đã đủ học viên.");

        return (true, "");
    }
}