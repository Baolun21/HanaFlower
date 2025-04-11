using System.Globalization;
using BookShop.Data.Contexts;
using BookShop.Data.Dtos;
using BookShop.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;

namespace BookShop.Controllers
{
    public class ThanhToanController : Controller
    {
        private readonly IVnpay _vnpay;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _appContext;

        public ThanhToanController(IVnpay vnpay, IConfiguration configuration, AppDbContext appContext)
        {
            _vnpay = vnpay;
            _configuration = configuration;
            _appContext = appContext;

            _vnpay.Initialize(
                _configuration["Vnpay:TmnCode"], 
                _configuration["Vnpay:HashSecret"], 
                _configuration["Vnpay:BaseUrl"], 
                _configuration["Vnpay:ReturnUrl"]);
        }
        
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
        
        [AllowAnonymous]
        [HttpGet("IpnAction")]
        public IActionResult IpnAction()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    if (paymentResult.IsSuccess)
                    {
                        // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
                        return Ok();
                    }

                    // Thực hiện hành động nếu thanh toán thất bại tại đây. Ví dụ: Hủy đơn hàng.
                    return BadRequest("Thanh toán thất bại");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
        
        public ActionResult<string> Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    var resultDescription = $"{paymentResult.PaymentResponse.Description}. {paymentResult.TransactionStatus.Description}.";
                    var model = _appContext.Orders.FirstOrDefault(t => t.payment_id == paymentResult.PaymentId);
                    
                    if (paymentResult.IsSuccess)
                    {
                        if (model != null)
                        {
                            TempData["Message"] = "Đặt hàng thành công";
                            TempData["Success"] = true;
                            return RedirectToAction("Succ", "Cart");
                        }
                    }
                    else
                    {
                        if (model != null)
                        {
                            model.status = "Huỷ đơn";
                            
                            _appContext.Orders.Update(model);
                            _appContext.SaveChanges();
                            TempData["Message"] = "Đặt hàng thất bại";
                            TempData["Success"] = true;
                            return RedirectToAction("ViewOrders", "Order");
                        }
                    }

                    return BadRequest(resultDescription);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
    }
}
