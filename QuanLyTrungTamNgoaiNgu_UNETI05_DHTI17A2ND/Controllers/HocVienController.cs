// Họ và tên: Nguyễn Hoàng Huy
// Nội dung thực hiện: Controller quản lý Học viên dành cho Admin (Xem danh sách, Chi tiết, Tìm kiếm, Phân trang) và Học viên (Thông tin cá nhân, Cập nhật thông tin)

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu.Models.ViewModels;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;
using System;
using System.Linq;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Controllers
{
    public class HocVienController : Controller
    {
        private readonly AppDbContext _context;

        public HocVienController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // KHU VỰC DÀNH CHO ADMIN
        // ==========================================

        // GET: HocVien/Index
        public IActionResult Index(string searchString, int page = 1)
        {
            string? vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            int pageSize = 10;
            var query = _context.HocViens.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim().ToLower();
                query = query.Where(hv =>
                    hv.HoTen.ToLower().Contains(searchString) ||
                    hv.Email.ToLower().Contains(searchString) ||
                    hv.SoDienThoai.Contains(searchString));
            }

            int totalItems = query.Count();
            var danhSach = query
                .OrderBy(hv => hv.MaHocVien)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(danhSach);
        }

        // GET: HocVien/Details/5
        public IActionResult Details(int? id)
        {
            string? vaiTro = HttpContext.Session.GetString("VaiTro");
            if (vaiTro != "Admin")
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (id == null) return NotFound();

            // Sử dụng l!.KhoaHoc để xử lý null-safety cho Navigation Property, hết cảnh báo CS8602
            var hocVien = _context.HocViens
                .Include(hv => hv.DanhSachDangKy)
                .ThenInclude(dk => dk.LopHoc)
                .ThenInclude(l => l!.KhoaHoc)
                .FirstOrDefault(hv => hv.MaHocVien == id);

            if (hocVien == null) return NotFound();

            return View(hocVien);
        }

        // ==========================================
        // KHU VỰC DÀNH CHO HỌC VIÊN
        // ==========================================

        // GET: HocVien/ThongTinCaNhan
        [HttpGet]
        public IActionResult ThongTinCaNhan()
        {
            int? maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var hocVien = _context.HocViens.FirstOrDefault(hv => hv.MaTaiKhoan == maTaiKhoan.Value);
            if (hocVien == null) return NotFound();

            var model = new HocVienThongTinViewModel
            {
                HoTen = hocVien.HoTen,
                NgaySinh = hocVien.NgaySinh,
                GioiTinh = hocVien.GioiTinh,
                SoDienThoai = hocVien.SoDienThoai,
                Email = hocVien.Email,
                DiaChi = hocVien.DiaChi
            };

            return View(model);
        }

        // POST: HocVien/CapNhat
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CapNhat(HocVienThongTinViewModel model)
        {
            int? maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Validation bổ sung về giới hạn Ngày sinh
            if (model.NgaySinh > DateTime.Now)
            {
                ModelState.AddModelError("NgaySinh", "Ngày sinh không được ở thời điểm tương lai.");
            }
            if (model.NgaySinh.Year < 1900)
            {
                ModelState.AddModelError("NgaySinh", "Năm sinh không hợp lệ (không được trước năm 1900).");
            }

            if (!ModelState.IsValid)
            {
                return View("ThongTinCaNhan", model);
            }

            var hocVien = _context.HocViens.FirstOrDefault(hv => hv.MaTaiKhoan == maTaiKhoan.Value);
            var taiKhoan = _context.TaiKhoans.FirstOrDefault(tk => tk.MaTaiKhoan == maTaiKhoan.Value);

            if (hocVien == null || taiKhoan == null) return NotFound();

            // Cập nhật thông tin bảng HocVien
            hocVien.HoTen = model.HoTen;
            hocVien.NgaySinh = model.NgaySinh;
            hocVien.GioiTinh = model.GioiTinh;
            hocVien.SoDienThoai = model.SoDienThoai;
            hocVien.Email = model.Email;
            hocVien.DiaChi = model.DiaChi;

            // Đồng bộ Họ tên và Email sang bảng TaiKhoan
            taiKhoan.HoTen = model.HoTen;
            taiKhoan.Email = model.Email;

            _context.SaveChanges();

            // Cập nhật lại Session HoTen
            HttpContext.Session.SetString("HoTen", model.HoTen);

            TempData["ThongBaoThanhCong"] = "Cập nhật thông tin cá nhân thành công!";
            return RedirectToAction(nameof(ThongTinCaNhan));
        }
    }
}