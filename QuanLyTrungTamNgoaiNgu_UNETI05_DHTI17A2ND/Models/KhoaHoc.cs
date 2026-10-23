// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: Entity KhoaHoc
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models
{
    public class KhoaHoc
    {
        [Key]
        [Display(Name = "Mã khóa học")]
        public int MaKhoaHoc { get; set; }

        [Required(ErrorMessage = "Tên khóa học không được để trống")]
        [StringLength(150, ErrorMessage = "Tên khóa học tối đa 150 ký tự")]
        [Display(Name = "Tên khóa học")]
        public string TenKhoaHoc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập ngoại ngữ")]
        [StringLength(50)]
        [Display(Name = "Ngoại ngữ")]
        public string NgoaiNgu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập cấp độ")]
        [StringLength(50)]
        [Display(Name = "Cấp độ")]
        public string CapDo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập học phí")]
        [Range(1, 1000000000, ErrorMessage = "Học phí phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,0)")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        [Display(Name = "Học phí (VNĐ)")]
        public decimal HocPhi { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số buổi")]
        [Range(1, 200, ErrorMessage = "Số buổi phải lớn hơn 0")]
        [Display(Name = "Số buổi")]
        public int SoBuoi { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [Display(Name = "Đang mở")]
        public bool TrangThai { get; set; } = true;

        // KhoaHoc 1-n LopHoc
        public ICollection<LopHoc> DanhSachLop { get; set; } = new List<LopHoc>();
    }
}