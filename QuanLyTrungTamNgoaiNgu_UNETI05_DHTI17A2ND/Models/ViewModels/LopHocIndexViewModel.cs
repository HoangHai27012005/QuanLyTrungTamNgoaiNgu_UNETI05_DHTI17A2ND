// Họ và tên: Hoàng Thanh Hải   
// Mã sinh viên: 23203100089
// Nội dung thực hiện: ViewModel trang danh sách lớp học (điều kiện tìm kiếm/lọc/sắp xếp/phân trang)
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels
{
    public class LopHocIndexViewModel
    {
        // Dữ liệu của trang hiện tại
        public List<LopHocItemViewModel> DanhSach { get; set; } = new();

        // Điều kiện đang áp dụng (để hiện lại trên form và mang theo khi chuyển trang)
        public string? TuKhoa { get; set; }
        public int? MaKhoaHoc { get; set; }
        public string? NgoaiNgu { get; set; }
        public string? TrangThai { get; set; }
        public decimal? HocPhiTu { get; set; }
        public decimal? HocPhiDen { get; set; }
        public string SapXep { get; set; } = "ngay_desc";

        // Phân trang
        public int Trang { get; set; } = 1;
        public int KichThuocTrang { get; set; } = 5;
        public int TongSoBanGhi { get; set; }
        public int TongSoTrang { get; set; } = 1;

        public string? CanhBao { get; set; }

        // Dữ liệu cho các ô select
        public IEnumerable<SelectListItem> DanhSachKhoaHoc { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> DanhSachNgoaiNgu { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> DanhSachTrangThai { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> DanhSachSapXep { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> DanhSachKichThuoc { get; set; } = new List<SelectListItem>();

        public bool CoTrangTruoc => Trang > 1;
        public bool CoTrangSau => Trang < TongSoTrang;

        public int TuBanGhi => TongSoBanGhi == 0 ? 0 : (Trang - 1) * KichThuocTrang + 1;
        public int DenBanGhi => TongSoBanGhi == 0 ? 0 : TuBanGhi + DanhSach.Count - 1;

        // Có điều kiện tìm kiếm/lọc nào đang bật không (để hiện thông báo phù hợp)
        public bool CoDieuKienLoc =>
            !string.IsNullOrEmpty(TuKhoa) || MaKhoaHoc.HasValue || !string.IsNullOrEmpty(NgoaiNgu) ||
            !string.IsNullOrEmpty(TrangThai) || HocPhiTu.HasValue || HocPhiDen.HasValue;

        // Gom các điều kiện KHÁC RỖNG để gắn vào mọi liên kết chuyển trang (asp-all-route-data)
        public Dictionary<string, string> LayDuLieuLoc()
        {
            var d = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(TuKhoa)) d["tuKhoa"] = TuKhoa;
            if (MaKhoaHoc.HasValue) d["maKhoaHoc"] = MaKhoaHoc.Value.ToString();
            if (!string.IsNullOrEmpty(NgoaiNgu)) d["ngoaiNgu"] = NgoaiNgu;
            if (!string.IsNullOrEmpty(TrangThai)) d["trangThai"] = TrangThai;
            if (HocPhiTu.HasValue) d["hocPhiTu"] = HocPhiTu.Value.ToString(CultureInfo.InvariantCulture);
            if (HocPhiDen.HasValue) d["hocPhiDen"] = HocPhiDen.Value.ToString(CultureInfo.InvariantCulture);
            d["sapXep"] = SapXep;
            d["kichThuocTrang"] = KichThuocTrang.ToString();
            return d;
        }
    }
}