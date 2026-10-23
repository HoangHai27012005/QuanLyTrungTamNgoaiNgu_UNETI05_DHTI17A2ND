// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 2320310089
// Nội dung thực hiện: Entity HocVien
using System.ComponentModel.DataAnnotations;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models
{
    public class HocVien
    {
        [Key]
        [Display(Name = "Mã học viên")]
        public int MaHocVien { get; set; }

        [Display(Name = "Mã tài khoản")]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ tên bắt buộc nhập")]
        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn ngày sinh")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Ngày sinh")]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn giới tính")]
        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string GioiTinh { get; set; } = "Nam";

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [RegularExpression(@"^(0|\+84)\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số, bắt đầu bằng 0")]
        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Địa chỉ tối đa 200 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        // Navigation
        public TaiKhoan? TaiKhoan { get; set; }

        // HocVien 1-n DangKyHoc
        public ICollection<DangKyHoc> DanhSachDangKy { get; set; } = new List<DangKyHoc>();
    }
}