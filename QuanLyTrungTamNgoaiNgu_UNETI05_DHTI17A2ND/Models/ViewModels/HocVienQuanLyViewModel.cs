// Họ và tên: Nguyễn Hoàng Huy
// Mã sinh viên: 23203100054
// Nội dung thực hiện: ViewModel danh sách và chi tiết học viên cho Admin

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;

public class HocVienItemViewModel
{
    public int MaHocVien { get; set; }
    public string HoTen { get; set; } = "";
    public string Email { get; set; } = "";
    public string SoDienThoai { get; set; } = "";
    public int SoDangKy { get; set; } // số đăng ký còn hiệu lực
}

public class HocVienIndexViewModel
{
    public string? TuKhoa { get; set; }
    public int Trang { get; set; }
    public int KichThuocTrang { get; set; }
    public int TongSoTrang { get; set; }
    public int TongSoBanGhi { get; set; }
    public List<HocVienItemViewModel> DanhSach { get; set; } = new();
}

public class HocVienChiTietViewModel
{
    public int MaHocVien { get; set; }
    public string HoTen { get; set; } = "";
    public DateTime NgaySinh { get; set; }
    public string GioiTinh { get; set; } = "";
    public string SoDienThoai { get; set; } = "";
    public string Email { get; set; } = "";
    public string? DiaChi { get; set; }
    public List<LichSuDangKyViewModel> DanhSachDangKy { get; set; } = new();
}