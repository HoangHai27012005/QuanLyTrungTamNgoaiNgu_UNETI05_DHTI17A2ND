// Họ và tên: Nguyễn Hoàng Huy
// Mã sinh viên: 23203100054
// Nội dung thực hiện: Controller học viên (Admin xem/tìm kiếm, học viên tự cập nhật thông tin)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Filters;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Helpers;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Controllers;

public class HocVienController : Controller
{
    private readonly AppDbContext _db;

    public HocVienController(AppDbContext db)
    {
        _db = db;
    }

    // ===== PHẦN CỦA ADMIN =====

    // GET: HocVien/Index - tìm theo họ tên, email, số điện thoại + phân trang
    [YeuCauAdmin]
    public async Task<IActionResult> Index(string? tuKhoa, int trang = 1, int kichThuocTrang = 10)
    {
        if (kichThuocTrang < 1 || kichThuocTrang > 50) kichThuocTrang = 10;

        // Giữ IQueryable, chưa gửi SQL
        var truyVan = _db.HocViens.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            tuKhoa = tuKhoa.Trim();
            truyVan = truyVan.Where(h => h.HoTen.Contains(tuKhoa)
                                      || h.Email.Contains(tuKhoa)
                                      || h.SoDienThoai.Contains(tuKhoa));
        }

        int tongSoBanGhi = await truyVan.CountAsync();
        int tongSoTrang = Math.Max(1, (int)Math.Ceiling(tongSoBanGhi / (double)kichThuocTrang));
        trang = Math.Clamp(trang, 1, tongSoTrang);

        var danhSach = await truyVan
            .OrderBy(h => h.HoTen)
            .ThenBy(h => h.MaHocVien)
            .Skip((trang - 1) * kichThuocTrang)
            .Take(kichThuocTrang)
            .Select(h => new HocVienItemViewModel
            {
                MaHocVien = h.MaHocVien,
                HoTen = h.HoTen,
                Email = h.Email,
                SoDienThoai = h.SoDienThoai,
                // subquery Count trong cùng câu Select => không bị N+1
                SoDangKy = h.DanhSachDangKy.Count(d => d.TrangThai != "Đã hủy")
            })
            .ToListAsync();

        var vm = new HocVienIndexViewModel
        {
            TuKhoa = tuKhoa,
            Trang = trang,
            KichThuocTrang = kichThuocTrang,
            TongSoTrang = tongSoTrang,
            TongSoBanGhi = tongSoBanGhi,
            DanhSach = danhSach
        };
        return View(vm);
    }

    // GET: HocVien/Details/5 - thông tin học viên + các lớp đã đăng ký
    [YeuCauAdmin]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var vm = await _db.HocViens
            .AsNoTracking()
            .Where(h => h.MaHocVien == id)
            .Select(h => new HocVienChiTietViewModel
            {
                MaHocVien = h.MaHocVien,
                HoTen = h.HoTen,
                NgaySinh = h.NgaySinh,
                GioiTinh = h.GioiTinh,
                SoDienThoai = h.SoDienThoai,
                Email = h.Email,
                DiaChi = h.DiaChi,
                DanhSachDangKy = h.DanhSachDangKy
                    .OrderByDescending(d => d.NgayDangKy)
                    .Select(d => new LichSuDangKyViewModel
                    {
                        MaDangKy = d.MaDangKy,
                        MaLop = d.MaLop,
                        TenLop = d.LopHoc!.TenLop,
                        TenKhoaHoc = d.LopHoc.KhoaHoc!.TenKhoaHoc,
                        NgayDangKy = d.NgayDangKy,
                        HocPhi = d.HocPhi,
                        TrangThai = d.TrangThai
                    }).ToList()
            })
            .FirstOrDefaultAsync();

        if (vm == null) return NotFound();
        return View(vm);
    }

    // ===== PHẦN CỦA HỌC VIÊN =====

    // GET: HocVien/ThongTinCaNhan
    [YeuCauHocVien]
    public async Task<IActionResult> ThongTinCaNhan()
    {
        var maTaiKhoan = PhienDangNhap.LayMaTaiKhoan(HttpContext.Session);

        var vm = await _db.HocViens
            .AsNoTracking()
            .Where(h => h.MaTaiKhoan == maTaiKhoan)
            .Select(h => new HocVienThongTinViewModel
            {
                HoTen = h.HoTen,
                NgaySinh = h.NgaySinh,
                GioiTinh = h.GioiTinh,
                SoDienThoai = h.SoDienThoai,
                Email = h.Email,
                DiaChi = h.DiaChi
            })
            .FirstOrDefaultAsync();

        if (vm == null) return NotFound();
        return View(vm);
    }

    // POST: HocVien/CapNhat
    [HttpPost]
    [ValidateAntiForgeryToken]
    [YeuCauHocVien]
    public async Task<IActionResult> CapNhat(HocVienThongTinViewModel vm)
    {
        if (!ModelState.IsValid)
            return View("ThongTinCaNhan", vm);   // hiện lại form kèm lỗi

        var maTaiKhoan = PhienDangNhap.LayMaTaiKhoan(HttpContext.Session);

        // Lấy học viên của CHÍNH người đăng nhập (có tracking để sửa)
        var hocVien = await _db.HocViens
            .Include(h => h.TaiKhoan)
            .FirstOrDefaultAsync(h => h.MaTaiKhoan == maTaiKhoan);

        if (hocVien == null) return NotFound();

        hocVien.HoTen = vm.HoTen.Trim();
        hocVien.NgaySinh = vm.NgaySinh!.Value;
        hocVien.GioiTinh = vm.GioiTinh;
        hocVien.SoDienThoai = vm.SoDienThoai.Trim();
        hocVien.Email = vm.Email.Trim();
        hocVien.DiaChi = string.IsNullOrWhiteSpace(vm.DiaChi) ? null : vm.DiaChi.Trim();

        // Đồng bộ họ tên và email sang bảng TaiKhoan
        if (hocVien.TaiKhoan != null)
        {
            hocVien.TaiKhoan.HoTen = hocVien.HoTen;
            hocVien.TaiKhoan.Email = hocVien.Email;
        }

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            TempData["Loi"] = "Không thể lưu thay đổi, vui lòng thử lại.";
            return RedirectToAction(nameof(ThongTinCaNhan));
        }

        // Cập nhật lại họ tên trong Session để menu hiển thị tên mới
        HttpContext.Session.SetString("HoTen", hocVien.HoTen);

        TempData["ThanhCong"] = "Cập nhật thông tin cá nhân thành công.";
        return RedirectToAction(nameof(ThongTinCaNhan));
    }
}