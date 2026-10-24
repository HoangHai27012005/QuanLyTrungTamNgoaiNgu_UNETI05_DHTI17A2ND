// Họ và tên: Ngụy Thế Toàn
// Mã sinh viên: 2320310050
// Nội dung thực hiện: Các ViewModel cho 6 thống kê

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;

// 1. Số lớp theo khóa học
public class ThongKeLopTheoKhoaHocVM
{
    public int MaKhoaHoc { get; set; }
    public string TenKhoaHoc { get; set; } = "";
    public int SoLop { get; set; }
}

// 2. Số đăng ký theo lớp (tách theo trạng thái)
public class ThongKeDangKyTheoLopVM
{
    public int MaLop { get; set; }
    public string TenLop { get; set; } = "";
    public string TenKhoaHoc { get; set; } = "";
    public int SoLuongToiDa { get; set; }
    public int SoChoXacNhan { get; set; }
    public int SoDaXacNhan { get; set; }
    public int SoDaHuy { get; set; }
    public int TongSo { get; set; }
}

// 3. Số đăng ký theo trạng thái
public class ThongKeDangKyTheoTrangThaiVM
{
    public string TrangThai { get; set; } = "";
    public int SoLuong { get; set; }
}

// 4. Lớp có nhiều đăng ký nhất (danh sách có thể có nhiều lớp đồng hạng)
public class ThongKeLopNhieuDangKyNhatVM
{
    public int MaLop { get; set; }
    public string TenLop { get; set; } = "";
    public string TenKhoaHoc { get; set; } = "";
    public int SoDangKy { get; set; }
}

// 5. Tổng học phí theo lớp (chỉ đăng ký Đã xác nhận)
public class ThongKeHocPhiTheoLopVM
{
    public int MaLop { get; set; }
    public string TenLop { get; set; } = "";
    public string TenKhoaHoc { get; set; } = "";
    public int SoDangKy { get; set; }
    public decimal TongHocPhi { get; set; }
}

// 6. Tổng học phí theo khóa học (chỉ đăng ký Đã xác nhận)
public class ThongKeHocPhiTheoKhoaHocVM
{
    public int MaKhoaHoc { get; set; }
    public string TenKhoaHoc { get; set; } = "";
    public int SoDangKy { get; set; }
    public decimal TongHocPhi { get; set; }
}