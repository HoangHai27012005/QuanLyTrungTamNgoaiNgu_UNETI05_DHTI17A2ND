// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: Filter bắt buộc đăng nhập (đọc Session, chưa đăng nhập thì chuyển về trang đăng nhập)
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Helpers;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Filters
{
    public class YeuCauDangNhapAttribute : ActionFilterAttribute
    {
        // Chạy TRƯỚC khi action trong Controller được thực thi
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Đã đăng nhập thì cho đi tiếp
            if (PhienDangNhap.DaDangNhap(context.HttpContext.Session)) return;

            // Gán thông báo hiển thị ở _Layout
            if (context.Controller is Controller controller)
                controller.TempData["Loi"] = "Vui lòng đăng nhập để tiếp tục.";

            // Chỉ nhớ địa chỉ quay lại với request GET (POST không thể "quay lại" bằng GET)
            var request = context.HttpContext.Request;
            string? returnUrl = HttpMethods.IsGet(request.Method)
                ? request.Path + request.QueryString
                : null;

            // Gán Result => action KHÔNG được chạy nữa (ngắt luồng)
            context.Result = new RedirectToActionResult("DangNhap", "TaiKhoan", new { returnUrl });
        }
    }
}