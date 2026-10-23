// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: Entity DangKyHoc
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models
{
    public class DangKyHoc
    {
        [Key]
        [Display(Name = "Mã đăng ký")]
        public int MaDangKy { get; set; }

        [Display(Name = "Học viên")]
        public int MaHocVien { get; set; }

        [Display(Name = "Lớp học")]
        public int MaLop { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = false)]
        [Display(Name = "Ngày đăng ký")]
        public DateTime NgayDangKy { get; set; } = DateTime.Now;

        [Range(0, 1000000000, ErrorMessage = "Học phí không hợp lệ")]
        [Column(TypeName = "decimal(18,0)")]
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = false)]
        [Display(Name = "Học phí (VNĐ)")]
        public decimal HocPhi { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = TrangThaiDangKyHoc.ChoXacNhan;

        public HocVien? HocVien { get; set; }
        public LopHoc? LopHoc { get; set; }
    }
}