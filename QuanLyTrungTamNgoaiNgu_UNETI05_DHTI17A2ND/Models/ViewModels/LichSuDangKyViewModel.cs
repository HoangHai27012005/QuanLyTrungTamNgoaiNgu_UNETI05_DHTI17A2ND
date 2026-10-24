// Họ và tên: Nguyễn Hoàng Huy
// Mã sinh viên: 23203100054
// Nội dung thực hiện: ViewModel hiển thị một dòng lịch sử đăng ký

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;

public class LichSuDangKyViewModel
{
    public int MaDangKy { get; set; }
    public int MaLop { get; set; }
    public string TenLop { get; set; } = "";
    public string TenKhoaHoc { get; set; } = "";
    public DateTime NgayDangKy { get; set; }
    public decimal HocPhi { get; set; }
    public string TrangThai { get; set; } = "";
}