// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: Lớp hỗ trợ đọc/ghi thông tin đăng nhập trong Session (dùng chung)
using Microsoft.AspNetCore.Http;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Helpers
{
    public static class PhienDangNhap
    {
        public const string KeyMaTaiKhoan = "MaTaiKhoan";
        public const string KeyHoTen = "HoTen";
        public const string KeyVaiTro = "VaiTro";

        // Gọi sau khi đăng nhập thành công
        public static void Luu(ISession session, TaiKhoan tk)
        {
            session.SetInt32(KeyMaTaiKhoan, tk.MaTaiKhoan);
            session.SetString(KeyHoTen, tk.HoTen);
            session.SetString(KeyVaiTro, tk.VaiTro);
        }

        public static void Xoa(ISession session) => session.Clear();

        public static bool DaDangNhap(ISession session) => session.GetInt32(KeyMaTaiKhoan) != null;

        public static bool LaAdmin(ISession session) =>
            session.GetString(KeyVaiTro) == VaiTroTaiKhoan.Admin;

        public static bool LaHocVien(ISession session) =>
            session.GetString(KeyVaiTro) == VaiTroTaiKhoan.HocVien;

        public static int? LayMaTaiKhoan(ISession session) => session.GetInt32(KeyMaTaiKhoan);

        public static string? LayHoTen(ISession session) => session.GetString(KeyHoTen);
    }
}