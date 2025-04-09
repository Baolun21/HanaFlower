using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace BookShop.Controllers
{
    public class ThanhToanController : Controller
    {
        [HttpPost]
        public IActionResult Index(string TenKH, string SDT, string Email, string DiaChiGiao, string note, decimal TongTien)
        {
            // Truyền dữ liệu đến view
            ViewBag.TenKH = TenKH ?? "";
            ViewBag.SDT = SDT ?? "";
            ViewBag.Email = Email ?? "";
            ViewBag.DiaChiGiao = DiaChiGiao ?? "";
            ViewBag.note = note ?? "";
            ViewBag.TongTien = TongTien;
            
        
            // Lưu tổng tiền dưới dạng string do Session chỉ lưu được string
            HttpContext.Session.SetString("TongTien", TongTien.ToString(CultureInfo.InvariantCulture));

            return View();
        }
    }
}
