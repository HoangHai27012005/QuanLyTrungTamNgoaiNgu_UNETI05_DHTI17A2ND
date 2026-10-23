// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: ViewModel cho form đăng nhập
using System.ComponentModel.DataAnnotations;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels
{
    public class DangNhapViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        // Địa chỉ muốn quay lại sau khi đăng nhập (do filter truyền sang)
        public string? ReturnUrl { get; set; }
    }
}