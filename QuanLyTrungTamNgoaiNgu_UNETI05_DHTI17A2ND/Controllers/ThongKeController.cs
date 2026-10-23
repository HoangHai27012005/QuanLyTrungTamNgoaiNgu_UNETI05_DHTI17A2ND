// Họ và tên: Nguỵ Thế Toàn
// Mã sinh viên: 2320310050
// Nội dung thực hiện:Quản lý đăng ký -Dashboard- Thống Kê

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Filters;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Controllers
{
    [YeuCauAdmin]
    public class ThongKeController : Controller
    {
        private readonly AppDbContext _context;
        public ThongKeController(AppDbContext context) => _context = context;

        public IActionResult Index() => View(); // Chứa các link tới các thống kê

        // 1. Số lớp theo từng khóa học
        public async Task<IActionResult> SoLopTheoKhoaHoc()
        {
            var data = await _context.KhoaHocs
                .Select(k => new ThongKeLopTheoKhoaHocVM
                {
                    TenKhoaHoc = k.TenKhoaHoc,
                    SoLop = k.DanhSachLop.Count()
                })
                .OrderByDescending(x => x.SoLop)
                .ToListAsync();
            return View(data);
        }

        // 2. Số học viên đăng ký theo từng lớp
        public async Task<IActionResult> DangKyTheoLop()
        {
            var data = await _context.LopHocs
                .Select(l => new ThongKeDangKyTheoLopVM
                {
                    TenLop = l.TenLop,
                    SoChoXacNhan = l.DanhSachDangKy.Count(d => d.TrangThai == "Chờ xác nhận"),
                    SoDaXacNhan = l.DanhSachDangKy.Count(d => d.TrangThai == "Đã xác nhận"),
                    SoDaHuy = l.DanhSachDangKy.Count(d => d.TrangThai == "Đã hủy")
                })
                .OrderByDescending(x => x.SoDaXacNhan)
                .ToListAsync();
            return View(data);
        }

        // 3. Số đăng ký theo từng trạng thái
        public async Task<IActionResult> DangKyTheoTrangThai()
        {
            var data = await _context.DangKyHocs
                .GroupBy(d => d.TrangThai)
                .Select(g => new ThongKeTheoTrangThaiVM
                {
                    TrangThai = g.Key,
                    SoLuong = g.Count()
                })
                .ToListAsync();
            return View(data);
        }

        // 4. Lớp có nhiều học viên đăng ký nhất (đồng hạng)
        public async Task<IActionResult> LopNhieuHocVienNhat()
        {
            var lopVaSoLuong = await _context.LopHocs
                .Select(l => new { Lop = l, Count = l.DanhSachDangKy.Count() })
                .ToListAsync(); // Tính toán số lượng trước

            int maxCount = lopVaSoLuong.Any() ? lopVaSoLuong.Max(x => x.Count) : 0;

            var result = lopVaSoLuong.Where(x => x.Count == maxCount && maxCount > 0)
                                     .Select(x => x.Lop)
                                     .ToList();

            ViewBag.MaxCount = maxCount;
            return View(result);
        }

        // 5. Tổng học phí theo lớp (Chỉ tính đã xác nhận)
        public async Task<IActionResult> HocPhiTheoLop()
        {
            var data = await _context.LopHocs
                .Select(l => new ThongKeHocPhiTheoLopVM
                {
                    TenLop = l.TenLop,
                    TenKhoaHoc = l.KhoaHoc.TenKhoaHoc,
                    // Ép kiểu nullable để tránh lỗi khi mảng rỗng
                    TongHocPhi = l.DanhSachDangKy.Where(d => d.TrangThai == "Đã xác nhận").Sum(d => (decimal?)d.HocPhi) ?? 0
                })
                .OrderByDescending(x => x.TongHocPhi)
                .ToListAsync();
            return View(data);
        }

        // 6. Tổng học phí theo khóa học (Chỉ tính đã xác nhận)
        public async Task<IActionResult> HocPhiTheoKhoaHoc()
        {
            var data = await _context.KhoaHocs
                .Select(k => new ThongKeHocPhiTheoKhoaHocVM
                {
                    TenKhoaHoc = k.TenKhoaHoc,
                    TongHocPhi = k.DanhSachLop.SelectMany(l => l.DanhSachDangKy)
                                          .Where(d => d.TrangThai == "Đã xác nhận")
                                          .Sum(d => (decimal?)d.HocPhi) ?? 0
                })
                .OrderByDescending(x => x.TongHocPhi)
                .ToListAsync();
            return View(data);
        }
    }
}