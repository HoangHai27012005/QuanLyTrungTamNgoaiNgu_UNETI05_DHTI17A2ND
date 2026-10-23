// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: Quản lý lớp học - CRUD (Admin), tìm kiếm, lọc, sắp xếp, phân trang bằng EF Core + LINQ
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Filters;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models.ViewModels;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Controllers
{
    [YeuCauDangNhap]   // Cả controller: phải đăng nhập. Create/Edit/Delete có thêm [YeuCauAdmin]
    public class LopHocController : Controller
    {
        private readonly AppDbContext _db;

        public LopHocController(AppDbContext db)
        {
            _db = db;
        }

        // Các kiểu sắp xếp hợp lệ: (giá trị trên URL, chữ hiển thị)
        private static readonly (string Gia, string Ten)[] KieuSapXep =
        {
            ("ngay_desc",   "Ngày khai giảng: mới nhất trước"),
            ("ngay_asc",    "Ngày khai giảng: cũ nhất trước"),
            ("ten_asc",     "Tên lớp: A → Z"),
            ("ten_desc",    "Tên lớp: Z → A"),
            ("hocphi_asc",  "Học phí: thấp → cao"),
            ("hocphi_desc", "Học phí: cao → thấp")
        };

        // Cách chuyển LopHoc (entity) sang LopHocItemViewModel. EF dịch biểu thức này thành SQL:
        // JOIN lấy tên khóa học + subquery đếm đăng ký "Đã xác nhận" trong CÙNG một câu truy vấn.
        private static readonly Expression<Func<LopHoc, LopHocItemViewModel>> ChuyenSangItem =
            l => new LopHocItemViewModel
            {
                MaLop = l.MaLop,
                TenLop = l.TenLop,
                MaKhoaHoc = l.MaKhoaHoc,
                TenKhoaHoc = l.KhoaHoc!.TenKhoaHoc,
                NgoaiNgu = l.KhoaHoc!.NgoaiNgu,
                CapDo = l.KhoaHoc!.CapDo,
                NgayKhaiGiang = l.NgayKhaiGiang,
                LichHoc = l.LichHoc,
                PhongHoc = l.PhongHoc,
                SoLuongToiDa = l.SoLuongToiDa,
                HocPhi = l.HocPhi,
                TrangThai = l.TrangThai,
                SoDaXacNhan = l.DanhSachDangKy.Count(d => d.TrangThai == TrangThaiDangKyHoc.DaXacNhan)
            };

        // ===================== DANH SÁCH: TÌM KIẾM + LỌC + SẮP XẾP + PHÂN TRANG =====================
        public async Task<IActionResult> Index(
            string? tuKhoa, int? maKhoaHoc, string? ngoaiNgu, string? trangThai,
            decimal? hocPhiTu, decimal? hocPhiDen, string? sapXep,
            int trang = 1, int kichThuocTrang = 5)
        {
            // ----- Chuẩn hóa tham số đầu vào -----
            tuKhoa = tuKhoa?.Trim();
            string? canhBao = null;

            // Học phí "từ" > "đến": tự hoán đổi và báo cho người dùng biết
            if (hocPhiTu.HasValue && hocPhiDen.HasValue && hocPhiTu > hocPhiDen)
            {
                (hocPhiTu, hocPhiDen) = (hocPhiDen, hocPhiTu);
                canhBao = "Học phí \"từ\" lớn hơn \"đến\" nên hệ thống đã tự hoán đổi hai giá trị.";
            }

            // Kiểu sắp xếp lạ trên URL thì dùng mặc định
            if (!KieuSapXep.Any(x => x.Gia == sapXep)) sapXep = "ngay_desc";

            // Kích thước trang ngoài khoảng 1..50 thì dùng 5
            if (kichThuocTrang < 1 || kichThuocTrang > 50) kichThuocTrang = 5;

            // ----- BƯỚC 1: TRUY VẤN GỐC (chưa chạy SQL, mới là IQueryable) -----
            IQueryable<LopHoc> query = _db.LopHocs.AsNoTracking();

            // ----- BƯỚC 2: TÌM KIẾM theo tên lớp HOẶC tên khóa học -----
            if (!string.IsNullOrEmpty(tuKhoa))
            {
                query = query.Where(l => l.TenLop.Contains(tuKhoa) ||
                                         l.KhoaHoc!.TenKhoaHoc.Contains(tuKhoa));
            }

            // ----- BƯỚC 3: LỌC (điều kiện nào null/rỗng thì bỏ qua, các điều kiện nối bằng AND) -----
            if (maKhoaHoc.HasValue)
                query = query.Where(l => l.MaKhoaHoc == maKhoaHoc.Value);

            if (!string.IsNullOrEmpty(ngoaiNgu))
                query = query.Where(l => l.KhoaHoc!.NgoaiNgu == ngoaiNgu);

            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(l => l.TrangThai == trangThai);

            if (hocPhiTu.HasValue)
                query = query.Where(l => l.HocPhi >= hocPhiTu.Value);

            if (hocPhiDen.HasValue)
                query = query.Where(l => l.HocPhi <= hocPhiDen.Value);

            // Đếm tổng số bản ghi SAU KHI tìm kiếm + lọc (kết quả đếm không phụ thuộc sắp xếp).
            // CountAsync chạy SELECT COUNT(*), không tải dữ liệu về.
            int tongSoBanGhi = await query.CountAsync();
            int tongSoTrang = Math.Max(1, (int)Math.Ceiling(tongSoBanGhi / (double)kichThuocTrang));

            // Trang < 1 thì về 1, trang > tổng số trang thì về trang cuối
            trang = Math.Clamp(trang, 1, tongSoTrang);

            // ----- BƯỚC 4: SẮP XẾP -----
            IOrderedQueryable<LopHoc> daSapXep = sapXep switch
            {
                "ten_asc" => query.OrderBy(l => l.TenLop),
                "ten_desc" => query.OrderByDescending(l => l.TenLop),
                "hocphi_asc" => query.OrderBy(l => l.HocPhi),
                "hocphi_desc" => query.OrderByDescending(l => l.HocPhi),
                "ngay_asc" => query.OrderBy(l => l.NgayKhaiGiang),
                _ => query.OrderByDescending(l => l.NgayKhaiGiang)   // ngay_desc (mặc định)
            };
            // Thêm khóa phụ MaLop để thứ tự ổn định: nếu 2 lớp trùng ngày/học phí thì không bị
            // nhảy qua lại giữa các trang
            daSapXep = daSapXep.ThenBy(l => l.MaLop);

            // ----- BƯỚC 5: PHÂN TRANG rồi mới chuyển sang ViewModel. ToListAsync() là lúc DUY NHẤT gửi SQL lấy dữ liệu -----
            var danhSach = await daSapXep
                .Skip((trang - 1) * kichThuocTrang)
                .Take(kichThuocTrang)
                .Select(ChuyenSangItem)
                .ToListAsync();

            // ----- BƯỚC 6: ĐÓNG GÓI VIEWMODEL ĐỂ HIỂN THỊ -----
            var vm = new LopHocIndexViewModel
            {
                DanhSach = danhSach,
                TuKhoa = tuKhoa,
                MaKhoaHoc = maKhoaHoc,
                NgoaiNgu = ngoaiNgu,
                TrangThai = trangThai,
                HocPhiTu = hocPhiTu,
                HocPhiDen = hocPhiDen,
                SapXep = sapXep!,
                Trang = trang,
                KichThuocTrang = kichThuocTrang,
                TongSoBanGhi = tongSoBanGhi,
                TongSoTrang = tongSoTrang,
                CanhBao = canhBao
            };

            // Dữ liệu cho các ô select của form lọc (lấy từ CSDL)
            vm.DanhSachKhoaHoc = await LayDanhSachKhoaHoc();
            vm.DanhSachNgoaiNgu = (await _db.KhoaHocs.AsNoTracking()
                    .Select(k => k.NgoaiNgu)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToListAsync())
                .Select(n => new SelectListItem(n, n)).ToList();
            vm.DanhSachTrangThai = TrangThaiLopHoc.TatCa.Select(t => new SelectListItem(t, t)).ToList();
            vm.DanhSachSapXep = KieuSapXep.Select(x => new SelectListItem(x.Ten, x.Gia)).ToList();
            vm.DanhSachKichThuoc = new[] { 5, 10, 20 }
                .Select(n => new SelectListItem($"{n} dòng / trang", n.ToString())).ToList();

            return View(vm);
        }

        // ===================== CHI TIẾT =====================
        public async Task<IActionResult> Details(int id)
        {
            var lop = await _db.LopHocs.AsNoTracking()
                .Where(l => l.MaLop == id)
                .Select(ChuyenSangItem)
                .FirstOrDefaultAsync();

            if (lop == null) return NotFound();
            return View(lop);
        }

        // ===================== THÊM =====================
        [YeuCauAdmin]
        public async Task<IActionResult> Create()
        {
            var vm = new LopHocFormViewModel
            {
                NgayKhaiGiang = DateTime.Today.AddDays(14),
                TrangThai = TrangThaiLopHoc.SapMo
            };
            await NapDanhSach(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [YeuCauAdmin]
        public async Task<IActionResult> Create(LopHocFormViewModel vm)
        {
            await KiemTraNghiepVu(vm);

            if (!ModelState.IsValid)
            {
                await NapDanhSach(vm);
                return View(vm);
            }

            var lop = new LopHoc
            {
                TenLop = vm.TenLop.Trim(),
                MaKhoaHoc = vm.MaKhoaHoc!.Value,
                NgayKhaiGiang = vm.NgayKhaiGiang!.Value.Date,
                LichHoc = vm.LichHoc.Trim(),
                PhongHoc = vm.PhongHoc.Trim(),
                SoLuongToiDa = vm.SoLuongToiDa!.Value,
                HocPhi = vm.HocPhi!.Value,
                TrangThai = vm.TrangThai
            };

            _db.LopHocs.Add(lop);
            await _db.SaveChangesAsync();

            TempData["ThanhCong"] = $"Đã thêm lớp học \"{lop.TenLop}\".";
            return RedirectToAction(nameof(Index));
        }

        // ===================== SỬA =====================
        [YeuCauAdmin]
        public async Task<IActionResult> Edit(int id)
        {
            var lop = await _db.LopHocs.AsNoTracking().FirstOrDefaultAsync(l => l.MaLop == id);
            if (lop == null) return NotFound();

            var vm = new LopHocFormViewModel
            {
                MaLop = lop.MaLop,
                TenLop = lop.TenLop,
                MaKhoaHoc = lop.MaKhoaHoc,
                NgayKhaiGiang = lop.NgayKhaiGiang,
                LichHoc = lop.LichHoc,
                PhongHoc = lop.PhongHoc,
                SoLuongToiDa = lop.SoLuongToiDa,
                HocPhi = lop.HocPhi,
                TrangThai = lop.TrangThai,
                SoDaXacNhan = await DemDaXacNhan(id)
            };
            await NapDanhSach(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [YeuCauAdmin]
        public async Task<IActionResult> Edit(int id, LopHocFormViewModel vm)
        {
            if (id != vm.MaLop) return NotFound();

            // Lấy entity thật từ CSDL rồi chỉ gán các trường cho phép sửa (tránh over-posting)
            var lop = await _db.LopHocs.FindAsync(id);
            if (lop == null) return NotFound();

            await KiemTraNghiepVu(vm);

            // Quy tắc: không giảm sĩ số tối đa xuống thấp hơn số học viên ĐÃ XÁC NHẬN
            vm.SoDaXacNhan = await DemDaXacNhan(id);
            if (vm.SoLuongToiDa.HasValue && vm.SoLuongToiDa.Value < vm.SoDaXacNhan)
            {
                ModelState.AddModelError(nameof(vm.SoLuongToiDa),
                    $"Lớp đã có {vm.SoDaXacNhan} học viên được xác nhận, số lượng tối đa không được nhỏ hơn con số này.");
            }

            if (!ModelState.IsValid)
            {
                await NapDanhSach(vm);
                return View(vm);
            }

            lop.TenLop = vm.TenLop.Trim();
            lop.MaKhoaHoc = vm.MaKhoaHoc!.Value;
            lop.NgayKhaiGiang = vm.NgayKhaiGiang!.Value.Date;
            lop.LichHoc = vm.LichHoc.Trim();
            lop.PhongHoc = vm.PhongHoc.Trim();
            lop.SoLuongToiDa = vm.SoLuongToiDa!.Value;
            lop.HocPhi = vm.HocPhi!.Value;
            lop.TrangThai = vm.TrangThai;

            await _db.SaveChangesAsync();

            TempData["ThanhCong"] = $"Đã cập nhật lớp học \"{lop.TenLop}\".";
            return RedirectToAction(nameof(Index));
        }

        // ===================== XÓA =====================
        [YeuCauAdmin]
        public async Task<IActionResult> Delete(int id)
        {
            var lop = await _db.LopHocs.AsNoTracking()
                .Where(l => l.MaLop == id)
                .Select(ChuyenSangItem)
                .FirstOrDefaultAsync();
            if (lop == null) return NotFound();

            ViewBag.SoDangKy = await _db.DangKyHocs.CountAsync(d => d.MaLop == id);
            return View(lop);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [YeuCauAdmin]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lop = await _db.LopHocs.FindAsync(id);
            if (lop == null)
            {
                TempData["Loi"] = "Lớp học không tồn tại hoặc đã bị xóa.";
                return RedirectToAction(nameof(Index));
            }

            // Lớp đã có bất kỳ lượt đăng ký nào (kể cả đã hủy) thì không được xóa để giữ lịch sử
            int soDangKy = await _db.DangKyHocs.CountAsync(d => d.MaLop == id);
            if (soDangKy > 0)
            {
                TempData["Loi"] = $"Không thể xóa lớp \"{lop.TenLop}\" vì đã có {soDangKy} lượt đăng ký. " +
                                  "Bạn có thể chuyển trạng thái sang \"Đã kết thúc\".";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _db.LopHocs.Remove(lop);
                await _db.SaveChangesAsync();
                TempData["ThanhCong"] = $"Đã xóa lớp học \"{lop.TenLop}\".";
            }
            catch (DbUpdateException)
            {
                TempData["Loi"] = "Không thể xóa lớp học do còn dữ liệu liên quan.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ===================== HÀM HỖ TRỢ (private) =====================

        private async Task<List<SelectListItem>> LayDanhSachKhoaHoc()
        {
            var khoaHocs = await _db.KhoaHocs.AsNoTracking().OrderBy(k => k.TenKhoaHoc).ToListAsync();
            return khoaHocs
                .Select(k => new SelectListItem($"{k.TenKhoaHoc} ({k.NgoaiNgu})", k.MaKhoaHoc.ToString()))
                .ToList();
        }

        private async Task NapDanhSach(LopHocFormViewModel vm)
        {
            vm.DanhSachKhoaHoc = await LayDanhSachKhoaHoc();
            vm.DanhSachTrangThai = TrangThaiLopHoc.TatCa.Select(t => new SelectListItem(t, t)).ToList();
        }

        private Task<int> DemDaXacNhan(int maLop) =>
            _db.DangKyHocs.CountAsync(d => d.MaLop == maLop && d.TrangThai == TrangThaiDangKyHoc.DaXacNhan);

        // Các quy tắc Data Annotation không làm được vì phải hỏi CSDL / kiểm tra danh sách hợp lệ
        private async Task KiemTraNghiepVu(LopHocFormViewModel vm)
        {
            if (vm.MaKhoaHoc.HasValue &&
                !await _db.KhoaHocs.AnyAsync(k => k.MaKhoaHoc == vm.MaKhoaHoc.Value))
            {
                ModelState.AddModelError(nameof(vm.MaKhoaHoc), "Khóa học không tồn tại");
            }

            if (!string.IsNullOrEmpty(vm.TrangThai) && !TrangThaiLopHoc.TatCa.Contains(vm.TrangThai))
            {
                ModelState.AddModelError(nameof(vm.TrangThai), "Trạng thái không hợp lệ");
            }
        }
    }
}