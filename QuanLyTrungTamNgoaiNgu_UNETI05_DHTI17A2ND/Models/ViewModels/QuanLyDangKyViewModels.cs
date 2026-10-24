// Họ và tên: Ngụy Thế Toàn
// Mã sinh viên: 2320310050
// Nội dung thực hiện: ViewModel cho màn hình quản lý đăng ký (danh sách và chi tiết)

using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;

// Một dòng trong danh sách đăng ký
public class QuanLyDangKyItemViewModel
{
    public int MaDangKy { get; set; }
    public string HoTenHocVien { get; set; } = "";
    public int MaLop { get; set; }
    public string TenLop { get; set; } = "";
    public string TenKhoaHoc { get; set; } = "";
    public DateTime NgayDangKy { get; set; }
    public decimal HocPhi { get; set; }
    public string TrangThai { get; set; } = "";
    public int SoLuongToiDa { get; set; }
    public int SoDaXacNhan { get; set; }
    public bool DaDay => SoDaXacNhan >= SoLuongToiDa;
}

// Dữ liệu cho trang danh sách: điều kiện lọc + phân trang + dữ liệu cho select
public class QuanLyDangKyIndexViewModel
{
    public string? TuKhoa { get; set; }
    public int? MaLop { get; set; }
    public string? TrangThai { get; set; }

    public int Trang { get; set; }
    public int KichThuocTrang { get; set; }
    public int TongSoTrang { get; set; }
    public int TongSoBanGhi { get; set; }

    public List<QuanLyDangKyItemViewModel> DanhSach { get; set; } = new();
    public List<SelectListItem> DanhSachLop { get; set; } = new();
    public List<string> DanhSachTrangThai { get; set; } = new();
}

// Chi tiết một đăng ký
public class QuanLyDangKyChiTietViewModel
{
    public int MaDangKy { get; set; }
    public DateTime NgayDangKy { get; set; }
    public decimal HocPhi { get; set; }
    public string TrangThai { get; set; } = "";

    public int MaHocVien { get; set; }
    public string HoTenHocVien { get; set; } = "";
    public string EmailHocVien { get; set; } = "";
    public string SoDienThoaiHocVien { get; set; } = "";

    public int MaLop { get; set; }
    public string TenLop { get; set; } = "";
    public string TenKhoaHoc { get; set; } = "";
    public DateTime NgayKhaiGiang { get; set; }
    public string LichHoc { get; set; } = "";
    public string PhongHoc { get; set; } = "";
    public string TrangThaiLop { get; set; } = "";
    public int SoLuongToiDa { get; set; }
    public int SoDaXacNhan { get; set; }
    public bool DaDay => SoDaXacNhan >= SoLuongToiDa;

    // Các trạng thái được phép chuyển sang (lấy từ bảng chuyển trạng thái)
    public List<string> TrangThaiChoPhep { get; set; } = new();
}