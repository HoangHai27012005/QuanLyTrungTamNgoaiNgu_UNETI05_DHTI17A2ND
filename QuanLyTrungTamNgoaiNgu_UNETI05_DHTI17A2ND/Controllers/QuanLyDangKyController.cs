// Họ và tên: Nguỵ Thế Toàn
// Mã sinh viên: 2320310050
// Nội dung thực hiện:Quản lý đăng ký -Dashboard- Thống Kê
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Filters;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Services;
// TODO: using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Services; (Nếu bạn có file Service chứa interface IDangKyHocService)

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Controllers
{
    [YeuCauAdmin]
    public class QuanLyDangKyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IDangKyHocService _dangKyService; // Của Sinh viên 3

        public QuanLyDangKyController(AppDbContext context, IDangKyHocService dangKyService)
        {
            _context = context;
            _dangKyService = dangKyService;
        }

        public async Task<IActionResult> Index(string searchTen, int? maLop, string trangThai, int page = 1)
        {
            int pageSize = 10;
            var query = _context.DangKyHocs
                .Include(d => d.HocVien)
                .Include(d => d.LopHoc).ThenInclude(l => l.KhoaHoc)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTen))
                query = query.Where(d => d.HocVien.HoTen.Contains(searchTen));
            if (maLop.HasValue)
                query = query.Where(d => d.MaLop == maLop);
            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(d => d.TrangThai == trangThai);

            // Phân trang
            int totalItems = await query.CountAsync();
            var data = await query.OrderByDescending(d => d.NgayDangKy)
                                  .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.LopHocs = new SelectList(_context.LopHocs, "MaLop", "TenLop");
            ViewBag.SearchTen = searchTen;
            ViewBag.TrangThai = trangThai;

            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XacNhan(int id)
        {
            var dk = await _context.DangKyHocs.Include(d => d.LopHoc).FirstOrDefaultAsync(d => d.MaDangKy == id);
            if (dk == null || dk.TrangThai != "Chờ xác nhận") return NotFound();

            int daXacNhan = _dangKyService.DemDaXacNhan(dk.MaLop);
            if (daXacNhan >= dk.LopHoc.SoLuongToiDa)
            {
                TempData["Error"] = $"Lớp '{dk.LopHoc.TenLop}' đã đủ số lượng ({dk.LopHoc.SoLuongToiDa}). Không thể xác nhận thêm.";
                return RedirectToAction(nameof(Index));
            }

            dk.TrangThai = "Đã xác nhận";
            await _context.SaveChangesAsync();
            TempData["Success"] = "Xác nhận đăng ký thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            var dk = await _context.DangKyHocs.FindAsync(id);
            if (dk == null) return NotFound();

            dk.TrangThai = "Đã hủy";
            await _context.SaveChangesAsync();
            TempData["Success"] = "Đã hủy đăng ký thành công.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(int id, string trangThaiMoi)
        {
            var dk = await _context.DangKyHocs.Include(d => d.LopHoc).FirstOrDefaultAsync(d => d.MaDangKy == id);
            if (dk == null) return NotFound();

            // Nếu muốn chuyển sang "Đã xác nhận" từ trạng thái khác
            if (trangThaiMoi == "Đã xác nhận" && dk.TrangThai != "Đã xác nhận")
            {
                int daXacNhan = _dangKyService.DemDaXacNhan(dk.MaLop);
                if (daXacNhan >= dk.LopHoc.SoLuongToiDa)
                {
                    TempData["Error"] = "Cập nhật thất bại. Lớp đã đủ học viên.";
                    return RedirectToAction(nameof(Index));
                }
            }

            dk.TrangThai = trangThaiMoi;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật trạng thái thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}