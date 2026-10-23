// Họ và tên: Hoàng Thanh Hải   
// Mã sinh viên: 23203100089    
// Nội dung thực hiện: Định nghĩa hằng số vai trò, trạng thái lớp học, trạng thái đăng ký (dùng chung toàn nhóm)
namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models
{
    public static class VaiTroTaiKhoan
    {
        public const string Admin = "Admin";
        public const string HocVien = "HocVien";
    }

    public static class TrangThaiLopHoc
    {
        public const string SapMo = "Sắp mở";
        public const string DangTuyenSinh = "Đang tuyển sinh";
        public const string DangHoc = "Đang học";
        public const string DaKetThuc = "Đã kết thúc";

        // Dùng để đổ vào <select> lọc/nhập liệu
        public static readonly string[] TatCa = { SapMo, DangTuyenSinh, DangHoc, DaKetThuc };
    }

    public static class TrangThaiDangKyHoc
    {
        public const string ChoXacNhan = "Chờ xác nhận";
        public const string DaXacNhan = "Đã xác nhận";
        public const string DaHuy = "Đã hủy";

        public static readonly string[] TatCa = { ChoXacNhan, DaXacNhan, DaHuy };
    }
}