namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TongKhoaHoc { get; set; }
        public int TongLopHoc { get; set; }
        public int TongHocVien { get; set; }
        public int TongDangKy { get; set; }
        public int DangKyChoXacNhan { get; set; }
        public int DangKyDaXacNhan { get; set; }
        public int LopDangTuyenSinh { get; set; }
    }

    public class ThongKeLopTheoKhoaHocVM
    {
        public string TenKhoaHoc { get; set; }
        public int SoLop { get; set; }
    }

    public class ThongKeDangKyTheoLopVM
    {
        public string TenLop { get; set; }
        public int SoChoXacNhan { get; set; }
        public int SoDaXacNhan { get; set; }
        public int SoDaHuy { get; set; }
        public int TongDangKy => SoChoXacNhan + SoDaXacNhan + SoDaHuy;
    }

    public class ThongKeTheoTrangThaiVM
    {
        public string TrangThai { get; set; }
        public int SoLuong { get; set; }
    }

    public class ThongKeHocPhiTheoLopVM
    {
        public string TenLop { get; set; }
        public string TenKhoaHoc { get; set; }
        public decimal TongHocPhi { get; set; }
    }

    public class ThongKeHocPhiTheoKhoaHocVM
    {
        public string TenKhoaHoc { get; set; }
        public decimal TongHocPhi { get; set; }
    }
}
