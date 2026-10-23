// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: Quản lý khóa học (CRUD cho Admin, xem danh sách/chi tiết cho học viên)
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Filters;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Helpers;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Controllers
{
    [YeuCauDangNhap]   // Cả controller: phải đăng nhập mới vào được
    public class KhoaHocController : Controller
    {
        private readonly AppDbContext _db;

        public KhoaHocController(AppDbContext db)
        {
            _db = db;
        }

        // ===== Danh sách (Admin và học viên dùng chung) =====
        public async Task<IActionResult> Index()
        {
            var query = _db.KhoaHocs.AsNoTracking();

            // Học viên chỉ thấy khóa đang mở; Admin thấy tất cả
            if (!PhienDangNhap.LaAdmin(HttpContext.Session))
                query = query.Where(k => k.TrangThai);

            var danhSach = await query
                .OrderBy(k => k.NgoaiNgu)
                .ThenBy(k => k.TenKhoaHoc)
                .ToListAsync();

            return View(danhSach);
        }

        // ===== Chi tiết =====
        public async Task<IActionResult> Details(int id)
        {
            var khoaHoc = await _db.KhoaHocs.AsNoTracking()
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);
            if (khoaHoc == null) return NotFound();

            // Học viên không được xem khóa đã đóng
            if (!khoaHoc.TrangThai && !PhienDangNhap.LaAdmin(HttpContext.Session))
                return NotFound();

            ViewBag.SoLop = await _db.LopHocs.CountAsync(l => l.MaKhoaHoc == id);
            return View(khoaHoc);
        }

        // ===== Thêm =====
        [YeuCauAdmin]
        public IActionResult Create()
        {
            return View(new KhoaHoc { TrangThai = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [YeuCauAdmin]
        public async Task<IActionResult> Create(
            [Bind("TenKhoaHoc,NgoaiNgu,CapDo,HocPhi,SoBuoi,MoTa,TrangThai")] KhoaHoc khoaHoc)
        {
            if (!ModelState.IsValid) return View(khoaHoc);

            _db.KhoaHocs.Add(khoaHoc);
            await _db.SaveChangesAsync();

            TempData["ThanhCong"] = $"Đã thêm khóa học \"{khoaHoc.TenKhoaHoc}\".";
            return RedirectToAction(nameof(Index));
        }

        // ===== Sửa =====
        [YeuCauAdmin]
        public async Task<IActionResult> Edit(int id)
        {
            var khoaHoc = await _db.KhoaHocs.FindAsync(id);
            if (khoaHoc == null) return NotFound();
            return View(khoaHoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [YeuCauAdmin]
        public async Task<IActionResult> Edit(int id,
            [Bind("MaKhoaHoc,TenKhoaHoc,NgoaiNgu,CapDo,HocPhi,SoBuoi,MoTa,TrangThai")] KhoaHoc khoaHoc)
        {
            if (id != khoaHoc.MaKhoaHoc) return NotFound();
            if (!ModelState.IsValid) return View(khoaHoc);

            if (!await _db.KhoaHocs.AnyAsync(k => k.MaKhoaHoc == id)) return NotFound();

            _db.Update(khoaHoc);
            await _db.SaveChangesAsync();

            TempData["ThanhCong"] = $"Đã cập nhật khóa học \"{khoaHoc.TenKhoaHoc}\".";
            return RedirectToAction(nameof(Index));
        }

        // ===== Xóa: bước 1 - trang xác nhận =====
        [YeuCauAdmin]
        public async Task<IActionResult> Delete(int id)
        {
            var khoaHoc = await _db.KhoaHocs.AsNoTracking()
                .FirstOrDefaultAsync(k => k.MaKhoaHoc == id);
            if (khoaHoc == null) return NotFound();

            ViewBag.SoLop = await _db.LopHocs.CountAsync(l => l.MaKhoaHoc == id);
            return View(khoaHoc);
        }

        // ===== Xóa: bước 2 - thực hiện xóa (POST) =====
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [YeuCauAdmin]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khoaHoc = await _db.KhoaHocs.FindAsync(id);
            if (khoaHoc == null)
            {
                TempData["Loi"] = "Khóa học không tồn tại hoặc đã bị xóa.";
                return RedirectToAction(nameof(Index));
            }

            // Quy tắc nghiệp vụ: khóa đã có lớp thì không được xóa
            if (await _db.LopHocs.AnyAsync(l => l.MaKhoaHoc == id))
            {
                TempData["Loi"] = $"Không thể xóa khóa học \"{khoaHoc.TenKhoaHoc}\" vì đã có lớp học thuộc khóa này.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _db.KhoaHocs.Remove(khoaHoc);
                await _db.SaveChangesAsync();
                TempData["ThanhCong"] = $"Đã xóa khóa học \"{khoaHoc.TenKhoaHoc}\".";
            }
            catch (DbUpdateException)
            {
                // Lưới an toàn: nếu vẫn dính ràng buộc khóa ngoại thì báo lỗi thân thiện
                TempData["Loi"] = "Không thể xóa khóa học do còn dữ liệu liên quan.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ===== Đổi trạng thái nhanh (Đang mở <-> Tạm đóng) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        [YeuCauAdmin]
        public async Task<IActionResult> DoiTrangThai(int id)
        {
            var khoaHoc = await _db.KhoaHocs.FindAsync(id);
            if (khoaHoc == null) return NotFound();

            khoaHoc.TrangThai = !khoaHoc.TrangThai;
            await _db.SaveChangesAsync();

            TempData["ThanhCong"] =
                $"Khóa học \"{khoaHoc.TenKhoaHoc}\" đã chuyển sang trạng thái {(khoaHoc.TrangThai ? "Đang mở" : "Tạm đóng")}.";
            return RedirectToAction(nameof(Index));
        }
    }
}