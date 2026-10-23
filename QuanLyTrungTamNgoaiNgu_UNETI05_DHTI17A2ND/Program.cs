// Họ và tên: Hoàng Thanh Hải
// Mã sinh viên: 23203100089
// Nội dung thực hiện: Cấu hình DbContext, Session, culture vi-VN, tự migrate và nạp dữ liệu mẫu
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Data;

var builder = WebApplication.CreateBuilder(args);

// Culture Việt Nam: tiền hiển thị 3.000.000, ngày dd/MM/yyyy
var vi = new CultureInfo("vi-VN");
CultureInfo.DefaultThreadCurrentCulture = vi;
CultureInfo.DefaultThreadCurrentUICulture = vi;

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session (cần cache bộ nhớ để lưu dữ liệu session)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Tự áp migration và nạp dữ liệu mẫu khi chạy lần đầu
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    DuLieuMau.KhoiTao(db);
    DuLieuMau.ThemLopHocMau(db);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();        // phải đặt trước UseAuthorization và MapControllerRoute
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();