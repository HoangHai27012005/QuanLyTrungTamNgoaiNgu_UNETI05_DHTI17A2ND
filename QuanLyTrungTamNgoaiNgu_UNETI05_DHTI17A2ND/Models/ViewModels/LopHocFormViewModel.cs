// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: ViewModel form thêm/sửa lớp học (kèm danh sách select khóa học, trạng thái)
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels
{
    public class LopHocFormViewModel
    {
        public int MaLop { get; set; }

        [Required(ErrorMessage = "Tên lớp bắt buộc nhập")]
        [StringLength(150, ErrorMessage = "Tên lớp tối đa 150 ký tự")]
        [Display(Name = "Tên lớp")]
        public string TenLop { get; set; } = string.Empty;

        // int? để khi chọn "-- Chọn khóa học --" (value rỗng) thì [Required] báo đúng thông báo
        [Required(ErrorMessage = "Vui lòng chọn khóa học")]
        [Display(Name = "Khóa học")]
        public int? MaKhoaHoc { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày khai giảng")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày khai giảng")]
        public DateTime? NgayKhaiGiang { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lịch học")]
        [StringLength(100, ErrorMessage = "Lịch học tối đa 100 ký tự")]
        [Display(Name = "Lịch học")]
        public string LichHoc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập phòng học")]
        [StringLength(50, ErrorMessage = "Phòng học tối đa 50 ký tự")]
        [Display(Name = "Phòng học")]
        public string PhongHoc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số lượng tối đa")]
        [Range(1, 500, ErrorMessage = "Số lượng tối đa phải lớn hơn 0")]
        [Display(Name = "Số lượng tối đa")]
        public int? SoLuongToiDa { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập học phí")]
        [Range(1, 1000000000, ErrorMessage = "Học phí phải lớn hơn 0")]
        [Display(Name = "Học phí (VNĐ)")]
        public decimal? HocPhi { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = TrangThaiLopHoc.SapMo;

        // Chỉ để hiển thị gợi ý ở form Sửa
        public int SoDaXacNhan { get; set; }

        public IEnumerable<SelectListItem> DanhSachKhoaHoc { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> DanhSachTrangThai { get; set; } = new List<SelectListItem>();
    }
}