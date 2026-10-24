// Họ và tên: Ngụy Thế Toàn
// Mã sinh viên: 2320310050
// Nội dung thực hiện: Admin quản lý đăng ký (danh sách, chi tiết, xác nhận, hủy, đổi trạng thái)

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Filters;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Services;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Controllers;

[YeuCauAdmin]
public class QuanLyDangKyController : Controller
{
    private readonly AppDbContext _db;
    private readonly DangKyHocService _dichVu;   // dùng DemDaXacNhan của SV3

    public QuanLyDangKyController(AppDbContext db, DangKyHocService dichVu)
    {
        _db = db;
        _dichVu = dichVu;
    }

    // Bảng chuyển trạng thái: khóa = trạng thái hiện tại, giá trị = các trạng thái được phép chuyển sang
    private static readonly Dictionary<string, string[]> BangChuyenTrangThai = new()
    {
        [TrangThaiDangKyHoc.ChoXacNhan] = new[] { TrangThaiDangKyHoc.DaXacNhan, TrangThaiDangKyHoc.DaHuy },
        [TrangThaiDangKyHoc.DaXacNhan] = new[] { TrangThaiDangKyHoc.DaHuy },
        [TrangThaiDangKyHoc.DaHuy] = new[] { TrangThaiDangKyHoc.ChoXacNhan }
    };

    // GET: QuanLyDangKy/Index - tìm theo tên học viên, lọc theo lớp và trạng thái, phân trang
    public async Task<IActionResult> Index(string? tuKhoa, int? maLop, string? trangThai,
                                           int trang = 1, int kichThuocTrang = 10)
    {
        if (kichThuocTrang < 1 || kichThuocTrang > 50) kichThuocTrang = 10;

        // Giữ IQueryable, chưa gửi SQL
        var truyVan = _db.DangKyHocs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            tuKhoa = tuKhoa.Trim();
            truyVan = truyVan.Where(d => d.HocVien!.HoTen.Contains(tuKhoa));
        }
        if (maLop.HasValue)
            truyVan = truyVan.Where(d => d.MaLop == maLop.Value);
        if (!string.IsNullOrWhiteSpace(trangThai))
            truyVan = truyVan.Where(d => d.TrangThai == trangThai);

        int tongSoBanGhi = await truyVan.CountAsync();
        int tongSoTrang = Math.Max(1, (int)Math.Ceiling(tongSoBanGhi / (double)kichThuocTrang));
        trang = Math.Clamp(trang, 1, tongSoTrang);

        var danhSach = await truyVan
            .OrderByDescending(d => d.NgayDangKy)
            .ThenByDescending(d => d.MaDangKy)
            .Skip((trang - 1) * kichThuocTrang)
            .Take(kichThuocTrang)
            .Select(d => new QuanLyDangKyItemViewModel
            {
                MaDangKy = d.MaDangKy,
                HoTenHocVien = d.HocVien!.HoTen,
                MaLop = d.MaLop,
                TenLop = d.LopHoc!.TenLop,
                TenKhoaHoc = d.LopHoc.KhoaHoc!.TenKhoaHoc,
                NgayDangKy = d.NgayDangKy,
                HocPhi = d.HocPhi,
                TrangThai = d.TrangThai,
                SoLuongToiDa = d.LopHoc.SoLuongToiDa,
                // subquery Count trong cùng câu Select => không bị N+1
                SoDaXacNhan = d.LopHoc.DanhSachDangKy
                    .Count(x => x.TrangThai == TrangThaiDangKyHoc.DaXacNhan)
            })
            .ToListAsync();

        // Dữ liệu cho select lọc theo lớp (lấy từ DB)
        var dsLop = await _db.LopHocs.AsNoTracking()
            .OrderBy(l => l.TenLop)
            .Select(l => new { l.MaLop, l.TenLop })
            .ToListAsync();

        var vm = new QuanLyDangKyIndexViewModel
        {
            TuKhoa = tuKhoa,
            MaLop = maLop,
            TrangThai = trangThai,
            Trang = trang,
            KichThuocTrang = kichThuocTrang,
            TongSoTrang = tongSoTrang,
            TongSoBanGhi = tongSoBanGhi,
            DanhSach = danhSach,
            DanhSachLop = dsLop
                .Select(l => new SelectListItem(l.TenLop, l.MaLop.ToString(), maLop == l.MaLop))
                .ToList(),
            DanhSachTrangThai = TrangThaiDangKyHoc.TatCa.ToList()
        };
        return View(vm);
    }

    // GET: QuanLyDangKy/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();

        var vm = await _db.DangKyHocs.AsNoTracking()
            .Where(d => d.MaDangKy == id)
            .Select(d => new QuanLyDangKyChiTietViewModel
            {
                MaDangKy = d.MaDangKy,
                NgayDangKy = d.NgayDangKy,
                HocPhi = d.HocPhi,
                TrangThai = d.TrangThai,
                MaHocVien = d.MaHocVien,
                HoTenHocVien = d.HocVien!.HoTen,
                EmailHocVien = d.HocVien.Email,
                SoDienThoaiHocVien = d.HocVien.SoDienThoai,
                MaLop = d.MaLop,
                TenLop = d.LopHoc!.TenLop,
                TenKhoaHoc = d.LopHoc.KhoaHoc!.TenKhoaHoc,
                NgayKhaiGiang = d.LopHoc.NgayKhaiGiang,
                LichHoc = d.LopHoc.LichHoc,
                PhongHoc = d.LopHoc.PhongHoc,
                TrangThaiLop = d.LopHoc.TrangThai,
                SoLuongToiDa = d.LopHoc.SoLuongToiDa,
                SoDaXacNhan = d.LopHoc.DanhSachDangKy
                    .Count(x => x.TrangThai == TrangThaiDangKyHoc.DaXacNhan)
            })
            .FirstOrDefaultAsync();

        if (vm == null) return NotFound();

        vm.TrangThaiChoPhep = BangChuyenTrangThai.TryGetValue(vm.TrangThai, out var ds)
            ? ds.ToList()
            : new List<string>();
        return View(vm);
    }

    // POST: Xác nhận (Chờ xác nhận -> Đã xác nhận, kiểm tra lớp còn chỗ)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XacNhan(int maDangKy, string? returnUrl)
    {
        var kq = await DoiTrangThaiAsync(maDangKy, TrangThaiDangKyHoc.DaXacNhan);
        DatThongBao(kq);
        return QuayLai(returnUrl);
    }

    // POST: Hủy (Chờ xác nhận hoặc Đã xác nhận -> Đã hủy)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Huy(int maDangKy, string? returnUrl)
    {
        var kq = await DoiTrangThaiAsync(maDangKy, TrangThaiDangKyHoc.DaHuy);
        DatThongBao(kq);
        return QuayLai(returnUrl);
    }

    // POST: Đổi trạng thái thủ công, vẫn áp dụng đúng bảng chuyển trạng thái
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CapNhatTrangThai(int maDangKy, string? trangThaiMoi, string? returnUrl)
    {
        var kq = await DoiTrangThaiAsync(maDangKy, trangThaiMoi ?? "");
        DatThongBao(kq);
        return QuayLai(returnUrl);
    }

    // ===== Các hàm dùng chung =====

    // Nơi DUY NHẤT thay đổi trạng thái đăng ký, mọi quy tắc nằm ở đây
    private async Task<(bool ThanhCong, string ThongBao)> DoiTrangThaiAsync(int maDangKy, string trangThaiMoi)
    {
        // Có tracking vì cần sửa
        var dk = await _db.DangKyHocs
            .Include(d => d.LopHoc)
            .Include(d => d.HocVien)
            .FirstOrDefaultAsync(d => d.MaDangKy == maDangKy);

        if (dk == null)
            return (false, "Không tìm thấy đăng ký.");

        string trangThaiCu = dk.TrangThai;

        // 1. Kiểm tra theo bảng chuyển trạng thái
        if (!BangChuyenTrangThai.TryGetValue(trangThaiCu, out var duocPhep) || !duocPhep.Contains(trangThaiMoi))
            return (false, "Không được chuyển đăng ký từ \"" + trangThaiCu + "\" sang \"" + trangThaiMoi + "\".");

        // 2. Chuyển sang Đã xác nhận: lớp phải còn chỗ
        if (trangThaiMoi == TrangThaiDangKyHoc.DaXacNhan)
        {
            int daXacNhan = _dichVu.DemDaXacNhan(dk.MaLop);
            if (daXacNhan >= dk.LopHoc!.SoLuongToiDa)
                return (false, "Lớp \"" + dk.LopHoc.TenLop + "\" đã đủ " + daXacNhan + "/"
                               + dk.LopHoc.SoLuongToiDa + " học viên, không thể xác nhận thêm.");
        }

        // 3. Khôi phục đăng ký đã hủy: không được trùng với đăng ký còn hiệu lực khác
        if (trangThaiCu == TrangThaiDangKyHoc.DaHuy && trangThaiMoi == TrangThaiDangKyHoc.ChoXacNhan)
        {
            bool biTrung = await _db.DangKyHocs.AnyAsync(d => d.MaHocVien == dk.MaHocVien
                                                           && d.MaLop == dk.MaLop
                                                           && d.MaDangKy != dk.MaDangKy
                                                           && d.TrangThai != TrangThaiDangKyHoc.DaHuy);
            if (biTrung)
                return (false, "Học viên này đã có một đăng ký còn hiệu lực ở lớp này, không thể khôi phục.");
        }

        dk.TrangThai = trangThaiMoi;
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            return (false, "Không thể lưu thay đổi, vui lòng thử lại.");
        }

        return (true, "Đã chuyển đăng ký của " + dk.HocVien!.HoTen + " (lớp " + dk.LopHoc!.TenLop
                      + ") sang \"" + trangThaiMoi + "\".");
    }

    private void DatThongBao((bool ThanhCong, string ThongBao) kq)
    {
        TempData[kq.ThanhCong ? "ThanhCong" : "Loi"] = kq.ThongBao;
    }

    // Quay về trang trước (giữ điều kiện lọc). Chỉ nhận URL nội bộ để tránh open redirect
    private IActionResult QuayLai(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);
        return RedirectToAction(nameof(Index));
    }
}