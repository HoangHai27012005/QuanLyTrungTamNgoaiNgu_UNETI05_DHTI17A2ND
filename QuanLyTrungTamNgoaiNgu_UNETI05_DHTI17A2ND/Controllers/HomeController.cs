// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: Trang chủ giới thiệu trung tâm, hiển thị khóa học nổi bật
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            // 3 khóa học đang mở để giới thiệu ở trang chủ (Take(3) => SQL: TOP(3))
            var khoaHocNoiBat = await _db.KhoaHocs.AsNoTracking()
                .Where(k => k.TrangThai)
                .OrderBy(k => k.MaKhoaHoc)
                .Take(3)
                .ToListAsync();

            return View(khoaHocNoiBat);
        }

        // Giữ nguyên action Error do template sinh ra (Program.cs dùng /Home/Error)
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}