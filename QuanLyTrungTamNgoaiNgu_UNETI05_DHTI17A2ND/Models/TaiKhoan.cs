// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089    
// Nội dung thực hiện: Entity TaiKhoan (tài khoản Admin/Học viên)
using System.ComponentModel.DataAnnotations;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models
{
    public class TaiKhoan
    {
        [Key]
        [Display(Name = "Mã tài khoản")]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Tên đăng nhập từ 4 đến 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 đến 100 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn vai trò")]
        [StringLength(20)]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = VaiTroTaiKhoan.HocVien;

        [Display(Name = "Đang hoạt động")]
        public bool TrangThai { get; set; } = true;   // true = hoạt động, false = bị khóa

        // Navigation: TaiKhoan 1-1 HocVien (Admin thì không có HocVien nên để nullable)
        public HocVien? HocVien { get; set; }
    }
}