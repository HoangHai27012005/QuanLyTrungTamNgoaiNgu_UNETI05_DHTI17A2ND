// Họ và tên: Nguyễn Hoàng Huy
// Mã sinh viên: [...]
// Nội dung thực hiện: ViewModel thông tin cá nhân và cập nhật thông tin học viên (Tránh Overposting)

using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTrungTamNgoaiNgu.Models.ViewModels
{
    public class HocVienThongTinViewModel
    {
        [Required(ErrorMessage = "Họ tên không được để trống.")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sinh không được để trống.")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính.")]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại không được để trống.")]
        [RegularExpression(@"^(0|\+84)\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ (định dạng 0xxxxxxxxx hoặc +84xxxxxxxxx).")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }
    }
}