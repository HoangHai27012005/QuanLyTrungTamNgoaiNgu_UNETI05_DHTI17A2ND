// Họ và tên: Nguyễn Hoàng Huy
// Mã sinh viên: [...]
// Nội dung thực hiện: ViewModel hiển thị danh sách lịch sử đăng ký lớp học của học viên

using System;

namespace QuanLyTrungTamNgoaiNgu.Models.ViewModels
{
    public class LichSuDangKyViewModel
    {
        public int MaDangKy { get; set; }
        public string TenLop { get; set; } = string.Empty;
        public string TenKhoaHoc { get; set; } = string.Empty;
        public DateTime NgayDangKy { get; set; }
        public decimal HocPhi { get; set; }
        public string TrangThai { get; set; } = string.Empty;
    }
}