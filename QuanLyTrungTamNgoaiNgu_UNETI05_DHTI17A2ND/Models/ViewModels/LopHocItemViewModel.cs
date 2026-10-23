// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: ViewModel một dòng lớp học (dùng cho danh sách, chi tiết, xóa)
using System.ComponentModel.DataAnnotations;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels
{
    public class LopHocItemViewModel
    {
        [Display(Name = "Mã lớp")]
        public int MaLop { get; set; }

        [Display(Name = "Tên lớp")]
        public string TenLop { get; set; } = string.Empty;

        public int MaKhoaHoc { get; set; }

        [Display(Name = "Khóa học")]
        public string TenKhoaHoc { get; set; } = string.Empty;

        [Display(Name = "Ngoại ngữ")]
        public string NgoaiNgu { get; set; } = string.Empty;

        [Display(Name = "Cấp độ")]
        public string CapDo { get; set; } = string.Empty;

        [Display(Name = "Ngày khai giảng")]
        public DateTime NgayKhaiGiang { get; set; }

        [Display(Name = "Lịch học")]
        public string LichHoc { get; set; } = string.Empty;

        [Display(Name = "Phòng học")]
        public string PhongHoc { get; set; } = string.Empty;

        [Display(Name = "Số lượng tối đa")]
        public int SoLuongToiDa { get; set; }

        [Display(Name = "Học phí")]
        public decimal HocPhi { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = string.Empty;

        // Số đăng ký "Đã xác nhận": được tính ngay trong câu truy vấn SQL (subquery)
        public int SoDaXacNhan { get; set; }

        // Số chỗ còn lại = tối đa - đã xác nhận (không bao giờ âm)
        [Display(Name = "Số chỗ còn lại")]
        public int ConLai => Math.Max(0, SoLuongToiDa - SoDaXacNhan);

        public bool DaDay => ConLai <= 0;
    }
}