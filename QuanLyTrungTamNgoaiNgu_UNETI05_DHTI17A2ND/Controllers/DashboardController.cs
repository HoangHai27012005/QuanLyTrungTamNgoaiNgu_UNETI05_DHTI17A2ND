// Họ và tên: Ngụy Thế Toàn
// Mã sinh viên: 2320310050
// Nội dung thực hiện: Dashboard tổng quan cho Admin

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Filters;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Controllers;

[YeuCauAdmin]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    // GET: Dashboard/Index
    public async Task<IActionResult> Index()
    {
        // Mỗi số liệu là một câu COUNT chạy ở SQL Server (DbContext không chạy song song nên await lần lượt)
        var vm = new DashboardViewModel
        {
            TongKhoaHoc = await _db.KhoaHocs.CountAsync(),
            TongLopHoc = await _db.LopHocs.CountAsync(),
            TongHocVien = await _db.HocViens.CountAsync(),
            TongLuotDangKy = await _db.DangKyHocs.CountAsync(),
            SoDangKyChoXacNhan = await _db.DangKyHocs
                .CountAsync(d => d.TrangThai == TrangThaiDangKyHoc.ChoXacNhan),
            SoDangKyDaXacNhan = await _db.DangKyHocs
                .CountAsync(d => d.TrangThai == TrangThaiDangKyHoc.DaXacNhan),
            SoLopDangTuyenSinh = await _db.LopHocs
                .CountAsync(l => l.TrangThai == TrangThaiLopHoc.DangTuyenSinh)
        };
        return View(vm);
    }
}