// Họ và tên: Nguyễn Hoàng Huy
// Mã sinh viên: 23203100054
// Nội dung thực hiện: ViewModel cập nhật thông tin cá nhân học viên (chống overposting)

using System.ComponentModel.DataAnnotations;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;

// Chỉ chứa các trường được phép sửa. Không có MaHocVien, MaTaiKhoan
public class HocVienThongTinViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    [Display(Name = "Họ và tên")]
    public string HoTen { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngày sinh")]
    public DateTime? NgaySinh { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn giới tính")]
    [Display(Name = "Giới tính")]
    public string GioiTinh { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [RegularExpression(@"^(0\d{9}|\+84\d{9})$",
        ErrorMessage = "Số điện thoại không hợp lệ (0xxxxxxxxx hoặc +84xxxxxxxxx)")]
    [Display(Name = "Số điện thoại")]
    public string SoDienThoai { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
    [Display(Name = "Email")]
    public string Email { get; set; } = "";

    [StringLength(200, ErrorMessage = "Địa chỉ tối đa 200 ký tự")]
    [Display(Name = "Địa chỉ")]
    public string? DiaChi { get; set; }

    // Kiểm tra ngày sinh: không ở tương lai, không trước năm 1900
    public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
    {
        if (NgaySinh.HasValue)
        {
            if (NgaySinh.Value.Date > DateTime.Today)
                yield return new ValidationResult("Ngày sinh không được ở tương lai", new[] { nameof(NgaySinh) });
            else if (NgaySinh.Value.Year < 1900)
                yield return new ValidationResult("Ngày sinh không được trước năm 1900", new[] { nameof(NgaySinh) });
        }
    }
}