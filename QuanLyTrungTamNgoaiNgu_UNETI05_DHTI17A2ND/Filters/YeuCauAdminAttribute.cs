// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: Filter chỉ cho Admin truy cập (kiểm tra vai trò trong Session ở phía Server)
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Helpers;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Filters
{
    // Kế thừa YeuCauDangNhap: gọi lại phần kiểm tra đăng nhập rồi kiểm tra thêm vai trò
    public class YeuCauAdminAttribute : YeuCauDangNhapAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);          // 1. chưa đăng nhập -> đã bị chuyển về đăng nhập
            if (context.Result != null) return;

            // 2. đã đăng nhập nhưng không phải Admin
            if (!PhienDangNhap.LaAdmin(context.HttpContext.Session))
            {
                if (context.Controller is Controller controller)
                    controller.TempData["Loi"] = "Bạn không có quyền truy cập chức năng này.";

                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}