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
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;
        public DashboardController(AppDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var vm = new DashboardViewModel
            {
                TongKhoaHoc = await _context.KhoaHocs.CountAsync(),
                TongLopHoc = await _context.LopHocs.CountAsync(),
                TongHocVien = await _context.HocViens.CountAsync(),
                TongDangKy = await _context.DangKyHocs.CountAsync(),
                DangKyChoXacNhan = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Chờ xác nhận"),
                DangKyDaXacNhan = await _context.DangKyHocs.CountAsync(d => d.TrangThai == "Đã xác nhận"),
                LopDangTuyenSinh = await _context.LopHocs.CountAsync(l => l.TrangThai == "Đang tuyển sinh")
            };
            return View(vm);
        }
    }
}