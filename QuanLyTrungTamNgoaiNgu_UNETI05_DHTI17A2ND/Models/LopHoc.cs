// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: Entity LopHoc
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models
{
    public class LopHoc
    {
        [Key]
        [Display(Name = "Mã lớp")]
        public int MaLop { get; set; }

        [Required(ErrorMessage = "Tên lớp bắt buộc nhập")]
        [StringLength(150, ErrorMessage = "Tên lớp tối đa 150 ký tự")]
        [Display(Name = "Tên lớp")]
        public string TenLop { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn khóa học")]
        [Display(Name = "Khóa học")]
        public int MaKhoaHoc { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày khai giảng")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Ngày khai giảng")]
        public DateTime NgayKhaiGiang { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Vui lòng nhập lịch học")]
        [StringLength(100)]
        [Display(Name = "Lịch học")]
        public string LichHoc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập phòng học")]
        [StringLength(50)]
        [Display(Name = "Phòng học")]
        public string PhongHoc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số lượng tối đa")]
        [Range(1, 500, ErrorMessage = "Số lượng tối đa phải lớn hơn 0")]
        [Display(Name = "Số lượng tối đa")]
        public int SoLuongToiDa { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập học phí")]
        [Range(1, 1000000000, ErrorMessage = "Học phí phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,0)")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        [Display(Name = "Học phí (VNĐ)")]
        public decimal HocPhi { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = TrangThaiLopHoc.SapMo;

        // Navigation (nullable để không bị báo "Required" khi POST form)
        public KhoaHoc? KhoaHoc { get; set; }

        // LopHoc 1-n DangKyHoc
        public ICollection<DangKyHoc> DanhSachDangKy { get; set; } = new List<DangKyHoc>();
    }
}