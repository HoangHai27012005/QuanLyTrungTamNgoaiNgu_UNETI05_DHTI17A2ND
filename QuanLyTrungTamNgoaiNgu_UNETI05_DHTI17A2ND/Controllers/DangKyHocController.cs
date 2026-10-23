// Họ và tên: Nguyễn Hoàng Huy
// Nội dung thực hiện: Controller quản lý action Đăng ký lớp học và Lịch sử đăng ký phía Học viên

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu.Models.ViewModels;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Services;
using System;
using System.Linq;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Controllers
{
    public class DangKyHocController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IDangKyHocService _dangKyHocService;

        public DangKyHocController(AppDbContext context, IDangKyHocService dangKyHocService)
        {
            _context = context;
            _dangKyHocService = dangKyHocService;
        }

        // POST: DangKyHoc/DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangKy(int maLop)
        {
            // Kiểm tra đăng nhập
            int? maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            string? vaiTro = HttpContext.Session.GetString("VaiTro");

            if (maTaiKhoan == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // Từ chối nếu tài khoản có vai trò Admin
            if (vaiTro == "Admin")
            {
                TempData["ThongBaoLoi"] = "Tài khoản Quản trị viên không thể đăng ký khóa học.";
                return RedirectToAction("Index", "LopHoc");
            }

            // Lấy MaHocVien an toàn từ Session qua MaTaiKhoan
            var hocVien = _context.HocViens.FirstOrDefault(hv => hv.MaTaiKhoan == maTaiKhoan.Value);
            if (hocVien == null)
            {
                TempData["ThongBaoLoi"] = "Không tìm thấy hồ sơ học viên liên kết với tài khoản này.";
                return RedirectToAction("Index", "LopHoc");
            }

            // Gọi kiểm tra điều kiện nghiệp vụ
            var (hopLe, thongBao) = _dangKyHocService.KiemTraDieuKienDangKy(hocVien.MaHocVien, maLop);
            if (!hopLe)
            {
                TempData["ThongBaoLoi"] = thongBao;
                return RedirectToAction("Index", "LopHoc");
            }

            // Lấy giá trị HocPhi trực tiếp từ server (LopHoc)
            var lopHoc = _context.LopHocs.Find(maLop);
            if (lopHoc == null)
            {
                TempData["ThongBaoLoi"] = "Lớp học không tồn tại.";
                return RedirectToAction("Index", "LopHoc");
            }

            var dangKy = new DangKyHoc
            {
                MaHocVien = hocVien.MaHocVien,
                MaLop = maLop,
                NgayDangKy = DateTime.Now,
                HocPhi = lopHoc.HocPhi,
                TrangThai = "Chờ xác nhận"
            };

            _context.DangKyHocs.Add(dangKy);
            _context.SaveChanges();

            TempData["ThongBaoThanhCong"] = "Đăng ký lớp học thành công! Vui lòng chờ trung tâm xác nhận.";
            return RedirectToAction(nameof(LichSu));
        }

        // GET: DangKyHoc/LichSu
        [HttpGet]
        public IActionResult LichSu()
        {
            int? maTaiKhoan = HttpContext.Session.GetInt32("MaTaiKhoan");
            if (maTaiKhoan == null)
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var hocVien = _context.HocViens.FirstOrDefault(hv => hv.MaTaiKhoan == maTaiKhoan.Value);
            if (hocVien == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Lấy danh sách đăng ký CỦA CHÍNH học viên đang đăng nhập, sắp xếp giảm dần theo ngày
            // Sử dụng null-check an toàn để loại bỏ hoàn toàn cảnh báo CS8602
            var danhSach = _context.DangKyHocs
                .Include(dk => dk.LopHoc)
                .ThenInclude(l => l!.KhoaHoc)
                .Where(dk => dk.MaHocVien == hocVien.MaHocVien)
                .OrderByDescending(dk => dk.NgayDangKy)
                .Select(dk => new LichSuDangKyViewModel
                {
                    MaDangKy = dk.MaDangKy,
                    TenLop = dk.LopHoc != null ? dk.LopHoc.TenLop : string.Empty,
                    TenKhoaHoc = (dk.LopHoc != null && dk.LopHoc.KhoaHoc != null) ? dk.LopHoc.KhoaHoc.TenKhoaHoc : string.Empty,
                    NgayDangKy = dk.NgayDangKy,
                    HocPhi = dk.HocPhi,
                    TrangThai = dk.TrangThai
                })
                .ToList();

            return View(danhSach);
        }
    }
}