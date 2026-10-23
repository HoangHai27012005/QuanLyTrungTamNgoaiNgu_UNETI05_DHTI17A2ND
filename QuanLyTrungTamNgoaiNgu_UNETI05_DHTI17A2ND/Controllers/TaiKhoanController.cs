// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 2320310089
// Nội dung thực hiện: Đăng nhập (Session), đăng ký tài khoản học viên, đăng xuất
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Helpers;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly AppDbContext _db;

        public TaiKhoanController(AppDbContext db)
        {
            _db = db;
        }

        // ===================== ĐĂNG NHẬP =====================
        [HttpGet]
        public IActionResult DangNhap(string? returnUrl = null)
        {
            // Đã đăng nhập rồi thì không cần vào trang đăng nhập nữa
            if (PhienDangNhap.DaDangNhap(HttpContext.Session))
                return RedirectToAction("Index", "Home");

            return View(new DangNhapViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var tenDangNhap = vm.TenDangNhap.Trim();

            // LINQ: tìm tài khoản khớp CẢ tên đăng nhập và mật khẩu
            var taiKhoan = await _db.TaiKhoans
                .FirstOrDefaultAsync(t => t.TenDangNhap == tenDangNhap && t.MatKhau == vm.MatKhau);

            // SQL Server mặc định không phân biệt hoa/thường, nên so sánh lại bằng C# (phân biệt hoa/thường)
            if (taiKhoan == null || taiKhoan.MatKhau != vm.MatKhau)
            {
                // Thông báo chung, không cho biết sai tên đăng nhập hay sai mật khẩu
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(vm);
            }

            // Đúng thông tin nhưng tài khoản bị khóa
            if (!taiKhoan.TrangThai)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên.");
                return View(vm);
            }

            // Thành công: xóa Session cũ rồi lưu mã tài khoản, họ tên, vai trò
            HttpContext.Session.Clear();
            PhienDangNhap.Luu(HttpContext.Session, taiKhoan);
            TempData["ThanhCong"] = $"Xin chào {taiKhoan.HoTen}, bạn đã đăng nhập thành công.";

            // Nếu bị filter đẩy sang đăng nhập thì quay lại trang đang xem (chỉ chấp nhận địa chỉ nội bộ)
            if (!string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                return LocalRedirect(vm.ReturnUrl);

            if (taiKhoan.VaiTro == VaiTroTaiKhoan.Admin)
                return RedirectToAction("Index", "ThongKe");     // Dashboard (Module 4)

            return RedirectToAction("Index", "LopHoc");          // Danh sách lớp học (Module 2)
        }

        // ===================== ĐĂNG KÝ =====================
        [HttpGet]
        public IActionResult DangKy()
        {
            if (PhienDangNhap.DaDangNhap(HttpContext.Session))
                return RedirectToAction("Index", "Home");

            return View(new DangKyTaiKhoanViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyTaiKhoanViewModel vm)
        {
            var tenDangNhap = vm.TenDangNhap?.Trim() ?? string.Empty;

            // Nghiệp vụ Data Annotation không làm được: kiểm tra trùng tên đăng nhập trong CSDL
            if (tenDangNhap != string.Empty &&
                await _db.TaiKhoans.AnyAsync(t => t.TenDangNhap == tenDangNhap))
            {
                ModelState.AddModelError(nameof(vm.TenDangNhap), "Tên đăng nhập đã tồn tại, vui lòng chọn tên khác");
            }

            if (!ModelState.IsValid) return View(vm);

            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = tenDangNhap,
                MatKhau = vm.MatKhau,
                HoTen = vm.HoTen.Trim(),
                Email = vm.Email.Trim(),
                VaiTro = VaiTroTaiKhoan.HocVien,
                TrangThai = true                       // hoạt động
            };

            // Gắn HocVien vào TaiKhoan: EF tự điền MaTaiKhoan cho HocVien khi lưu.
            // Ngày sinh, giới tính, SĐT là giá trị tạm; học viên cập nhật ở "Thông tin cá nhân" (Module 3).
            taiKhoan.HocVien = new HocVien
            {
                HoTen = taiKhoan.HoTen,
                Email = taiKhoan.Email,
                NgaySinh = new DateTime(2000, 1, 1),
                GioiTinh = "Nam",
                SoDienThoai = string.Empty
            };

            _db.TaiKhoans.Add(taiKhoan);
            await _db.SaveChangesAsync();             // MỘT lần lưu: ghi cả 2 bảng trong cùng một transaction

            TempData["ThanhCong"] = "Đăng ký thành công! Bạn có thể đăng nhập ngay.";
            return RedirectToAction(nameof(DangNhap));
        }

        // ===================== ĐĂNG XUẤT =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DangXuat()
        {
            PhienDangNhap.Xoa(HttpContext.Session);   // Session.Clear()
            TempData["ThanhCong"] = "Bạn đã đăng xuất.";
            return RedirectToAction("Index", "Home");
        }
    }
}