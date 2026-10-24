// Họ và tên: Ngụy Thế Toàn
// Mã sinh viên: 2320310050
// Nội dung thực hiện: 6 thống kê bằng LINQ (Count, Sum, GroupBy, OrderBy, OrderByDescending)

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Filters;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Controllers;

[YeuCauAdmin]
public class ThongKeController : Controller
{
    private readonly AppDbContext _db;

    public ThongKeController(AppDbContext db)
    {
        _db = db;
    }

    // Trang chủ thống kê: danh sách liên kết đến 6 thống kê
    public IActionResult Index()
    {
        return View();
    }

    // _Layout đang gọi ThongKe/ChiTiet, chuyển về trang chủ thống kê để không bị 404
    public IActionResult ChiTiet()
    {
        return RedirectToAction(nameof(Index));
    }

    // 1. Số lớp theo từng khóa học (khóa chưa có lớp vẫn hiện số 0)
    public async Task<IActionResult> LopTheoKhoaHoc()
    {
        // Lấy KhoaHocs làm gốc nên khóa không có lớp vẫn có dòng
        var ds = await _db.KhoaHocs.AsNoTracking()
            .OrderByDescending(k => k.DanhSachLop.Count())
            .ThenBy(k => k.TenKhoaHoc)
            .Select(k => new ThongKeLopTheoKhoaHocVM
            {
                MaKhoaHoc = k.MaKhoaHoc,
                TenKhoaHoc = k.TenKhoaHoc,
                SoLop = k.DanhSachLop.Count()
            })
            .ToListAsync();
        return View(ds);
    }

    // 2. Số đăng ký theo từng lớp, tách theo trạng thái
    public async Task<IActionResult> DangKyTheoLop()
    {
        var ds = await _db.LopHocs.AsNoTracking()
            .OrderByDescending(l => l.DanhSachDangKy.Count())
            .ThenBy(l => l.TenLop)
            .Select(l => new ThongKeDangKyTheoLopVM
            {
                MaLop = l.MaLop,
                TenLop = l.TenLop,
                TenKhoaHoc = l.KhoaHoc!.TenKhoaHoc,
                SoLuongToiDa = l.SoLuongToiDa,
                SoChoXacNhan = l.DanhSachDangKy.Count(d => d.TrangThai == TrangThaiDangKyHoc.ChoXacNhan),
                SoDaXacNhan = l.DanhSachDangKy.Count(d => d.TrangThai == TrangThaiDangKyHoc.DaXacNhan),
                SoDaHuy = l.DanhSachDangKy.Count(d => d.TrangThai == TrangThaiDangKyHoc.DaHuy),
                TongSo = l.DanhSachDangKy.Count()
            })
            .ToListAsync();
        return View(ds);
    }

    // 3. Số đăng ký theo từng trạng thái (GroupBy)
    public async Task<IActionResult> DangKyTheoTrangThai()
    {
        // GroupBy chạy ở SQL, chỉ trả về tối đa 3 dòng
        var daCo = await _db.DangKyHocs.AsNoTracking()
            .GroupBy(d => d.TrangThai)
            .Select(g => new ThongKeDangKyTheoTrangThaiVM
            {
                TrangThai = g.Key,
                SoLuong = g.Count()
            })
            .OrderByDescending(x => x.SoLuong)
            .ToListAsync();

        // Trạng thái chưa có đăng ký nào vẫn hiện với số 0 (chỉ vài dòng nên bổ sung trong bộ nhớ)
        foreach (var tt in TrangThaiDangKyHoc.TatCa)
        {
            if (!daCo.Any(x => x.TrangThai == tt))
                daCo.Add(new ThongKeDangKyTheoTrangThaiVM { TrangThai = tt, SoLuong = 0 });
        }
        return View(daCo);
    }

    // 4. Lớp có nhiều học viên đăng ký nhất (tính đăng ký còn hiệu lực = khác Đã hủy, hiện tất cả lớp đồng hạng)
    public async Task<IActionResult> LopNhieuDangKyNhat()
    {
        // Bước 1: tìm số đăng ký lớn nhất (GroupBy theo lớp, sắp giảm dần, lấy dòng đầu). Không có dữ liệu thì = 0
        int soLonNhat = await _db.DangKyHocs.AsNoTracking()
            .Where(d => d.TrangThai != TrangThaiDangKyHoc.DaHuy)
            .GroupBy(d => d.MaLop)
            .Select(g => g.Count())
            .OrderByDescending(c => c)
            .FirstOrDefaultAsync();

        if (soLonNhat == 0)
            return View(new List<ThongKeLopNhieuDangKyNhatVM>());

        // Bước 2: lấy TẤT CẢ lớp có số đăng ký bằng đúng số lớn nhất (xử lý đồng hạng)
        var ds = await _db.LopHocs.AsNoTracking()
            .Where(l => l.DanhSachDangKy.Count(d => d.TrangThai != TrangThaiDangKyHoc.DaHuy) == soLonNhat)
            .OrderBy(l => l.TenLop)
            .Select(l => new ThongKeLopNhieuDangKyNhatVM
            {
                MaLop = l.MaLop,
                TenLop = l.TenLop,
                TenKhoaHoc = l.KhoaHoc!.TenKhoaHoc,
                SoDangKy = l.DanhSachDangKy.Count(d => d.TrangThai != TrangThaiDangKyHoc.DaHuy)
            })
            .ToListAsync();
        return View(ds);
    }

    // 5. Tổng học phí theo lớp (CHỈ đăng ký Đã xác nhận)
    public async Task<IActionResult> HocPhiTheoLop()
    {
        var ds = await _db.DangKyHocs.AsNoTracking()
            .Where(d => d.TrangThai == TrangThaiDangKyHoc.DaXacNhan)
            .GroupBy(d => new { d.MaLop, d.LopHoc!.TenLop, d.LopHoc.KhoaHoc!.TenKhoaHoc })
            .Select(g => new ThongKeHocPhiTheoLopVM
            {
                MaLop = g.Key.MaLop,
                TenLop = g.Key.TenLop,
                TenKhoaHoc = g.Key.TenKhoaHoc,
                SoDangKy = g.Count(),
                TongHocPhi = g.Sum(d => d.HocPhi)
            })
            .OrderByDescending(x => x.TongHocPhi)
            .ThenBy(x => x.TenLop)
            .ToListAsync();
        return View(ds);
    }

    // 6. Tổng học phí theo khóa học (CHỈ đăng ký Đã xác nhận)
    public async Task<IActionResult> HocPhiTheoKhoaHoc()
    {
        var ds = await _db.DangKyHocs.AsNoTracking()
            .Where(d => d.TrangThai == TrangThaiDangKyHoc.DaXacNhan)
            .GroupBy(d => new { d.LopHoc!.MaKhoaHoc, d.LopHoc.KhoaHoc!.TenKhoaHoc })
            .Select(g => new ThongKeHocPhiTheoKhoaHocVM
            {
                MaKhoaHoc = g.Key.MaKhoaHoc,
                TenKhoaHoc = g.Key.TenKhoaHoc,
                SoDangKy = g.Count(),
                TongHocPhi = g.Sum(d => d.HocPhi)
            })
            .OrderByDescending(x => x.TongHocPhi)
            .ThenBy(x => x.TenKhoaHoc)
            .ToListAsync();
        return View(ds);
    }
}