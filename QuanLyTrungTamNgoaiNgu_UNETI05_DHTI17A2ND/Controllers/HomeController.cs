using Microsoft.AspNetCore.Mvc;
using QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Models;
using System.Diagnostics;

namespace QuanLyTrungTamNgoaiNgu_UNETI05_DHTI17A2ND.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
