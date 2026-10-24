// Họ và tên: Nguyễn Hoàng Huy
// Mã sinh viên: 23203100054
// Nội dung thực hiện: Controller đăng ký lớp học và xem lịch sử đăng ký của học viên

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Filters;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Helpers;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Services;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Controllers;

public class DangKyHocController : Controller
{
    private readonly AppDbContext _db;
    private readonly DangKyHocService _dichVu;

    public DangKyHocController(AppDbContext db, DangKyHocService dichVu)
    {
        _db = db;
        _dichVu = dichVu;
    }

    // POST: DangKyHoc/DangKy - chỉ học viên, chỉ nhận maLop
    [HttpPost]
    [ValidateAntiForgeryToken]
    [YeuCauHocVien]
    public async Task<IActionResult> DangKy(int maLop)
    {
        // Mã tài khoản luôn lấy từ Session, không nhận từ form
        var maTaiKhoan = PhienDangNhap.LayMaTaiKhoan(HttpContext.Session);
        var hocVien = await _db.HocViens.AsNoTracking()
            .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);

        if (hocVien == null)
        {
            TempData["Loi"] = "Không tìm thấy hồ sơ học viên của tài khoản này.";
            return RedirectToAction("Index", "LopHoc");
        }

        // Kiểm tra nghiệp vụ trong service
        var ketQua = _dichVu.KiemTraDieuKienDangKy(hocVien.MaHocVien, maLop);
        if (!ketQua.HopLe)
        {
            TempData["Loi"] = ketQua.ThongBao;
            return RedirectToAction("Details", "LopHoc", new { id = maLop });
        }

        // Học phí lấy từ lớp (server), không tin giá trị từ client
        var lop = await _db.LopHocs.AsNoTracking().FirstAsync(l => l.MaLop == maLop);

        var dangKy = new DangKyHoc
        {
            MaHocVien = hocVien.MaHocVien,
            MaLop = maLop,
            NgayDangKy = DateTime.Now,              // ngày do server quyết định
            HocPhi = lop.HocPhi,
            TrangThai = TrangThaiDangKyHoc.ChoXacNhan
        };

        try
        {
            _db.DangKyHocs.Add(dangKy);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            TempData["Loi"] = "Không thể lưu đăng ký, vui lòng thử lại.";
            return RedirectToAction("Details", "LopHoc", new { id = maLop });
        }

        TempData["ThanhCong"] = "Đăng ký lớp \"" + lop.TenLop + "\" thành công, vui lòng chờ xác nhận.";
        return RedirectToAction(nameof(LichSu));   // Post-Redirect-Get
    }

    // GET: DangKyHoc/LichSu - chỉ xem đăng ký của chính mình
    [YeuCauHocVien]
    public async Task<IActionResult> LichSu()
    {
        var maTaiKhoan = PhienDangNhap.LayMaTaiKhoan(HttpContext.Session);

        var danhSach = await _db.DangKyHocs
            .AsNoTracking()
            .Where(d => d.HocVien!.MaTaiKhoan == maTaiKhoan)
            .OrderByDescending(d => d.NgayDangKy)
            .ThenByDescending(d => d.MaDangKy)
            .Select(d => new LichSuDangKyViewModel
            {
                MaDangKy = d.MaDangKy,
                MaLop = d.MaLop,
                TenLop = d.LopHoc!.TenLop,
                TenKhoaHoc = d.LopHoc.KhoaHoc!.TenKhoaHoc,
                NgayDangKy = d.NgayDangKy,
                HocPhi = d.HocPhi,
                TrangThai = d.TrangThai
            })
            .ToListAsync();

        return View(danhSach);
    }
}