// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: Nạp dữ liệu mẫu (khóa học, lớp học, học viên, tài khoản, đăng ký)
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data
{
    public static class DuLieuMau
    {
        public static void KhoiTao(AppDbContext db)
        {
            // Đã có dữ liệu thì không nạp lại
            if (db.TaiKhoans.Any()) return;

            // ===== 1. TÀI KHOẢN ADMIN =====
            var admins = new List<TaiKhoan>
            {
                new TaiKhoan { TenDangNhap = "admin",   MatKhau = "admin123", HoTen = "Quản trị viên",   Email = "admin@uneti.edu.vn",   VaiTro = VaiTroTaiKhoan.Admin, TrangThai = true },
                new TaiKhoan { TenDangNhap = "admin02", MatKhau = "admin123", HoTen = "Nguyễn Thu Trang", Email = "admin02@uneti.edu.vn", VaiTro = VaiTroTaiKhoan.Admin, TrangThai = true }
            };

            // ===== 2. HỌC VIÊN + TÀI KHOẢN HỌC VIÊN (15 người) =====
            var thongTin = new (string HoTen, DateTime NgaySinh, string GioiTinh, string DiaChi)[]
            {
                ("Nguyễn Văn An",   new DateTime(2003, 3, 15),  "Nam", "12 Trần Phú, Hà Đông, Hà Nội"),
                ("Trần Thị Bình",   new DateTime(2002, 7, 21),  "Nữ",  "45 Nguyễn Trãi, Thanh Xuân, Hà Nội"),
                ("Lê Minh Châu",    new DateTime(2004, 1, 9),   "Nữ",  "8 Láng Hạ, Đống Đa, Hà Nội"),
                ("Phạm Quốc Dũng",  new DateTime(2001, 11, 30), "Nam", "23 Cầu Giấy, Cầu Giấy, Hà Nội"),
                ("Hoàng Thị Hà",    new DateTime(2003, 5, 18),  "Nữ",  "56 Kim Mã, Ba Đình, Hà Nội"),
                ("Vũ Đức Huy",      new DateTime(2000, 9, 2),   "Nam", "17 Lê Văn Lương, Thanh Xuân, Hà Nội"),
                ("Đặng Thu Hương",  new DateTime(2004, 12, 25), "Nữ",  "9 Xuân Thủy, Cầu Giấy, Hà Nội"),
                ("Bùi Văn Khoa",    new DateTime(2002, 4, 14),  "Nam", "31 Giải Phóng, Hai Bà Trưng, Hà Nội"),
                ("Ngô Thị Lan",     new DateTime(2003, 8, 8),   "Nữ",  "74 Tây Sơn, Đống Đa, Hà Nội"),
                ("Đỗ Minh Long",    new DateTime(2001, 2, 27),  "Nam", "5 Nguyễn Chí Thanh, Đống Đa, Hà Nội"),
                ("Phan Thị Mai",    new DateTime(2005, 6, 11),  "Nữ",  "88 Minh Khai, Hai Bà Trưng, Hà Nội"),
                ("Trịnh Văn Nam",   new DateTime(1999, 10, 5),  "Nam", "14 Quang Trung, Hà Đông, Hà Nội"),
                ("Lý Thanh Phương", new DateTime(2002, 3, 19),  "Nữ",  "62 Phạm Văn Đồng, Bắc Từ Liêm, Hà Nội"),
                ("Mai Văn Quang",   new DateTime(2000, 12, 12), "Nam", "27 Nguyễn Văn Cừ, Long Biên, Hà Nội"),
                ("Đinh Thị Thảo",   new DateTime(2004, 9, 23),  "Nữ",  "3 Hoàng Quốc Việt, Cầu Giấy, Hà Nội")
            };

            var hocViens = new List<HocVien>();
            for (int i = 0; i < thongTin.Length; i++)
            {
                var tk = new TaiKhoan
                {
                    TenDangNhap = $"hocvien{i + 1:00}",
                    MatKhau = "123456",
                    HoTen = thongTin[i].HoTen,
                    Email = $"hocvien{i + 1:00}@gmail.com",
                    VaiTro = VaiTroTaiKhoan.HocVien,
                    TrangThai = i != 14            // hocvien15 bị khóa để test đăng nhập
                };
                hocViens.Add(new HocVien
                {
                    TaiKhoan = tk,                 // EF tự điền MaTaiKhoan khi SaveChanges
                    HoTen = thongTin[i].HoTen,
                    NgaySinh = thongTin[i].NgaySinh,
                    GioiTinh = thongTin[i].GioiTinh,
                    SoDienThoai = $"09012345{i + 1:00}",
                    Email = tk.Email,
                    DiaChi = thongTin[i].DiaChi
                });
            }

            // ===== 3. KHÓA HỌC (5) =====
            var khoaHocs = new List<KhoaHoc>
            {
                new KhoaHoc { TenKhoaHoc = "Tiếng Anh Giao tiếp Cơ bản", NgoaiNgu = "Tiếng Anh", CapDo = "A2",       HocPhi = 3000000, SoBuoi = 24, MoTa = "Rèn phản xạ nghe nói cho người mới bắt đầu.", TrangThai = true },
                new KhoaHoc { TenKhoaHoc = "IELTS 6.5",                  NgoaiNgu = "Tiếng Anh", CapDo = "B2",       HocPhi = 8500000, SoBuoi = 36, MoTa = "Luyện thi IELTS 4 kỹ năng, mục tiêu 6.5.",   TrangThai = true },
                new KhoaHoc { TenKhoaHoc = "TOEIC 600+",                 NgoaiNgu = "Tiếng Anh", CapDo = "B1",       HocPhi = 4800000, SoBuoi = 30, MoTa = "Luyện đề TOEIC Listening & Reading.",         TrangThai = true },
                new KhoaHoc { TenKhoaHoc = "Tiếng Nhật N5",              NgoaiNgu = "Tiếng Nhật", CapDo = "N5",      HocPhi = 5500000, SoBuoi = 40, MoTa = "Bảng chữ cái, ngữ pháp và từ vựng N5.",       TrangThai = true },
                new KhoaHoc { TenKhoaHoc = "Tiếng Hàn Sơ cấp 1",         NgoaiNgu = "Tiếng Hàn", CapDo = "Sơ cấp 1", HocPhi = 4200000, SoBuoi = 32, MoTa = "Hangul và giao tiếp cơ bản.",                  TrangThai = true }
            };

            // ===== 4. LỚP HỌC (10) =====
            var lopHocs = new List<LopHoc>
            {
                Lop("Giao tiếp Cơ bản - K01", 1, new DateTime(2026, 8, 15),  "Thứ 2-4-6 (18:00-20:00)", "P101", 15, 3000000, TrangThaiLopHoc.DaKetThuc),
                Lop("Giao tiếp Cơ bản - K02", 1, new DateTime(2026, 10, 5),  "Thứ 3-5-7 (18:30-20:30)", "P102", 15, 3200000, TrangThaiLopHoc.DangHoc),
                Lop("Giao tiếp Cơ bản - K03", 1, new DateTime(2026, 11, 16), "Thứ 2-4-6 (19:00-21:00)", "P101", 20, 3200000, TrangThaiLopHoc.DangTuyenSinh),
                Lop("IELTS 6.5 - K01",        2, new DateTime(2026, 11, 2),  "Thứ 2-4 (18:00-20:30)",   "P201", 12, 8500000, TrangThaiLopHoc.DangTuyenSinh),
                Lop("IELTS 6.5 - K02",        2, new DateTime(2027, 1, 11),  "Thứ 3-5 (18:00-20:30)",   "P202", 12, 9000000, TrangThaiLopHoc.SapMo),
                Lop("TOEIC 600+ - K01",       3, new DateTime(2026, 11, 9),  "Thứ 7-CN (08:00-11:00)",  "P103", 3,  4800000, TrangThaiLopHoc.DangTuyenSinh),  // sẽ đủ 3/3 -> test lớp đầy
                Lop("TOEIC 600+ - K02",       3, new DateTime(2026, 9, 14),  "Thứ 2-4-6 (17:30-19:30)", "P104", 25, 4500000, TrangThaiLopHoc.DangHoc),
                Lop("Tiếng Nhật N5 - K01",    4, new DateTime(2026, 11, 23), "Thứ 3-5-7 (18:00-20:00)", "P301", 18, 5500000, TrangThaiLopHoc.DangTuyenSinh),
                Lop("Tiếng Nhật N5 - K02",    4, new DateTime(2027, 2, 1),   "Thứ 2-4-6 (18:00-20:00)", "P302", 18, 5800000, TrangThaiLopHoc.SapMo),
                Lop("Tiếng Hàn Sơ cấp 1 - K01", 5, new DateTime(2026, 11, 30), "Thứ 7-CN (14:00-17:00)", "P303", 20, 4200000, TrangThaiLopHoc.DangTuyenSinh)
            };

            // ===== 5. ĐĂNG KÝ (19) =====  (số trong ngoặc: học viên thứ mấy, lớp thứ mấy)
            var dangKys = new List<DangKyHoc>
            {
                DK(1, 1, TrangThaiDangKyHoc.DaXacNhan,  new DateTime(2026, 8, 1)),
                DK(2, 1, TrangThaiDangKyHoc.DaXacNhan,  new DateTime(2026, 8, 3)),

                DK(3, 2, TrangThaiDangKyHoc.DaXacNhan,  new DateTime(2026, 9, 20)),
                DK(4, 2, TrangThaiDangKyHoc.DaXacNhan,  new DateTime(2026, 9, 22)),
                DK(5, 2, TrangThaiDangKyHoc.DaXacNhan,  new DateTime(2026, 9, 25)),

                DK(1, 3, TrangThaiDangKyHoc.ChoXacNhan, new DateTime(2026, 10, 18)),

                DK(2, 4, TrangThaiDangKyHoc.DaXacNhan,  new DateTime(2026, 10, 10)),
                DK(7, 4, TrangThaiDangKyHoc.DaXacNhan,  new DateTime(2026, 10, 12)),
                DK(8, 4, TrangThaiDangKyHoc.ChoXacNhan, new DateTime(2026, 10, 20)),
                DK(9, 4, TrangThaiDangKyHoc.DaHuy,      new DateTime(2026, 10, 14)),

                DK(10, 6, TrangThaiDangKyHoc.DaXacNhan,  new DateTime(2026, 10, 8)),
                DK(11, 6, TrangThaiDangKyHoc.DaXacNhan,  new DateTime(2026, 10, 9)),
                DK(12, 6, TrangThaiDangKyHoc.DaXacNhan,  new DateTime(2026, 10, 11)),
                DK(13, 6, TrangThaiDangKyHoc.ChoXacNhan, new DateTime(2026, 10, 21)),  // lớp đã đủ -> test không cho xác nhận

                DK(3, 7, TrangThaiDangKyHoc.DaXacNhan,  new DateTime(2026, 9, 1)),
                DK(14, 7, TrangThaiDangKyHoc.DaXacNhan, new DateTime(2026, 9, 3)),

                DK(4, 8, TrangThaiDangKyHoc.ChoXacNhan, new DateTime(2026, 10, 19)),
                DK(15, 8, TrangThaiDangKyHoc.DaHuy,     new DateTime(2026, 10, 15)),

                DK(5, 10, TrangThaiDangKyHoc.DaXacNhan, new DateTime(2026, 10, 16))
            };

            // ----- hàm cục bộ giúp viết gọn -----
            LopHoc Lop(string ten, int maKh, DateTime khaiGiang, string lich, string phong, int toiDa, decimal hocPhi, string trangThai)
                => new LopHoc
                {
                    TenLop = ten,
                    KhoaHoc = khoaHocs[maKh - 1],
                    NgayKhaiGiang = khaiGiang,
                    LichHoc = lich,
                    PhongHoc = phong,
                    SoLuongToiDa = toiDa,
                    HocPhi = hocPhi,
                    TrangThai = trangThai
                };

            DangKyHoc DK(int hv, int lop, string trangThai, DateTime ngay)
                => new DangKyHoc
                {
                    HocVien = hocViens[hv - 1],
                    LopHoc = lopHocs[lop - 1],
                    HocPhi = lopHocs[lop - 1].HocPhi,
                    NgayDangKy = ngay,
                    TrangThai = trangThai
                };

            // ===== LƯU =====
            db.TaiKhoans.AddRange(admins);
            db.HocViens.AddRange(hocViens);   // kéo theo TaiKhoan của học viên
            db.KhoaHocs.AddRange(khoaHocs);
            db.LopHocs.AddRange(lopHocs);
            db.DangKyHocs.AddRange(dangKys);
            db.SaveChanges();
        }
        // Thêm 15 lớp bổ sung để test phân trang (chạy được cả khi CSDL đã có dữ liệu cũ)
        public static void ThemLopHocMau(AppDbContext db)
        {
            // Đã thêm rồi thì thôi, tránh nhân đôi dữ liệu mỗi lần chạy
            if (db.LopHocs.Any(l => l.TenLop.Contains("Bổ sung"))) return;

            var khoaHocs = db.KhoaHocs.OrderBy(k => k.MaKhoaHoc).ToList();
            if (khoaHocs.Count == 0) return;

            string[] lich = { "Thứ 2-4-6 (18:00-20:00)", "Thứ 3-5-7 (18:30-20:30)", "Thứ 7-CN (08:00-11:00)" };

            var dsLop = new List<LopHoc>();
            for (int i = 0; i < 15; i++)
            {
                var kh = khoaHocs[i % khoaHocs.Count];
                dsLop.Add(new LopHoc
                {
                    TenLop = $"{kh.TenKhoaHoc} - Bổ sung {i + 1:00}",
                    KhoaHoc = kh,
                    NgayKhaiGiang = new DateTime(2026, 12, 1).AddDays(i * 7),
                    LichHoc = lich[i % 3],
                    PhongHoc = $"P{401 + i}",
                    SoLuongToiDa = 10 + (i % 4) * 5,                  // 10, 15, 20, 25
                    HocPhi = 3000000 + (i % 8) * 500000,              // từ 3.0 triệu đến 6.5 triệu
                    TrangThai = TrangThaiLopHoc.TatCa[i % 4]          // lần lượt đủ 4 trạng thái
                });
            }

            db.LopHocs.AddRange(dsLop);
            db.SaveChanges();
        }
    }
}