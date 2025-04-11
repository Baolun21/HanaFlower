using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using BookShop.Data.Contexts;
using BookShop.Data.Dtos;
using BookShop.Data.Dtos.Cart;
using BookShop.Data.Models;
using VNPAY.NET;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;

namespace BookShop.Controllers
{
    public class CartController : Controller
    {
        private readonly IVnpay _vnpay;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _appContext;
        public CartController(IVnpay vnpay, IConfiguration configuration, AppDbContext appContext)
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

        public const string CARTKEY = "cart";
        public List<CartItem> GetCartItems()
        {

            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY)!;
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(jsoncart)!;
            }
            return new List<CartItem>();
        }
     
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        void SaveCartSession(List<CartItem> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }

        [Route("/cart", Name = "cart")]
        public IActionResult Index()
        {
            var session = HttpContext.Session;
            var role = HttpContext.Session.GetString("role");
            List<Category_requests> category_request = null!;

            if (role != null)
            {

                if (role == "Admin")
                {
                    category_request = _appContext.Category_Requests
                        .Where(x => x.is_approved == 0 && x.is_deleted_by_admin == false)
                        .OrderByDescending(x => x.created_at)
                        .ToList();
                }
                else if (role == "Store Owner")
                {

                    var user_id = int.Parse(HttpContext.Session.GetString("user_id")!);
                    category_request = _appContext.Category_Requests
                        .Where(x => x.is_deleted_by_owner == false && x.requester_id == user_id && (x.is_approved == -1 || x.is_approved == 1))
                        .OrderByDescending(x => x.created_at)
                        .ToList();
                }

                if (category_request != null)
                {
                    var jsonNotification = JsonConvert.SerializeObject(category_request);
                    session.SetString("notification", jsonNotification);
                }
            }

            return View(GetCartItems());
        }
        public IActionResult Succ()
        {
            return View();
        }
        [Route("addcart/{productid:int}", Name = "addcart")]
        public IActionResult AddToCart([FromRoute] int productid)
        {

            var product = _appContext.Products
                .Where(p => p.product_id == productid && p.quantity > 0)
                .FirstOrDefault();
            if (product == null)
                return NotFound("Không có sản phẩm");

            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.Product!.product_id == productid);
            if (cartitem != null)
            {
                cartitem.Quantity++;
            }
            else
            {
                cart.Add(new CartItem() { Quantity = 1, Product = product });
            }

            SaveCartSession(cart);
            return RedirectToAction(nameof(Index));
        }
        [Route("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart([FromForm] int productid, [FromForm] int quantity)
        {
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.Product!.product_id == productid);
            if (cartitem != null)
            {
                var product = _appContext.Products
                .Where(p => p.product_id == productid)
                .FirstOrDefault();
                if (product != null)
                {
                    if (product.quantity >= quantity)
                    {
                        cartitem.Quantity = quantity;
                    }
                    else
                    {
                        TempData["Message"] = "Sản phẩm không đủ số lượng";
                        TempData["Success"] = false;
                        return Ok();
                    }
                }
                else
                {
                    TempData["Message"] = "Sản phẩm không tồn tại";
                    TempData["Success"] = false;
                    return Ok();
                }
            }
            SaveCartSession(cart);
            return Ok();
        }
        [Route("/removecart/{productid:int}", Name = "removecart")]
        public IActionResult RemoveCart([FromRoute] int productid)
        {
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.Product!.product_id == productid);
            if (cartitem != null)
            {
                cart.Remove(cartitem);
            }

            SaveCartSession(cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(DetailOrder request)
        {
            if (HttpContext.Session.GetString("user_id") != null)
            {

                try
                {
                    request.CartItems = GetCartItems();
                    request.MaKH = int.Parse(HttpContext.Session.GetString("user_id")!);
                    var donHang = new Orders();
                    donHang.user_id = request.MaKH;
                    donHang.address = request.DiaChiGiao;
                    donHang.order_date = DateTime.Now;
                    donHang.status = "Chờ xử lý";
                    donHang.phone = request.SDT;
                    donHang.note = request.note;
                    donHang.email = request.email;
                    donHang.total_price = request.TongTien;
                    await _appContext.AddAsync(donHang);
                    await _appContext.SaveChangesAsync();

                    if (request.CartItems != null)
                    {
                        foreach (var item in request.CartItems)
                        {
                            var chiTietDonHang = new Order_items();
                            chiTietDonHang.order_id = donHang.order_id;
                            chiTietDonHang.product_id = item.Product!.product_id;
                            chiTietDonHang.quantity = item.Quantity;
                            chiTietDonHang.price = item.Product!.price;
                            await _appContext.AddAsync(chiTietDonHang);
                            await _appContext.SaveChangesAsync();

                            var product = _appContext.Products
                                .Where(p => p.product_id == item.Product!.product_id)
                                .FirstOrDefault();
                            if (product != null)
                            {
                                product.quantity -= item.Quantity;
                                await _appContext.SaveChangesAsync();
                            }
                        }

                        if (request.LoaiGiaoDich == 2)
                        {
                            TempData["Message"] = "Đặt hàng thành công";
                            TempData["Success"] = true;
                            ClearCart();
                            return RedirectToAction("Succ", "Cart");
                        }

                        if (request.LoaiGiaoDich == 1)
                        {
                            var dto = new TransactionDto();
                            dto.Id = donHang.order_id;
                            dto.Amount = donHang.total_price;
                            dto.Date = donHang.order_date;
                            dto.ClientName = donHang.user_id;
                            try
                            {
                                var ipAddress = NetworkHelper.GetIpAddress(HttpContext); // Lấy địa chỉ IP của thiết bị thực hiện giao dịch
                                var description = "Thanh toán đơn hàng cho tài khoản " + dto.ClientName + ".";
                                var request2 = new PaymentRequest
                                {
                                    PaymentId = DateTime.Now.Ticks,
                                    Money = (double)dto.Amount,
                                    Description = description,
                                    IpAddress = ipAddress,
                                    BankCode = BankCode.ANY,
                                    CreatedDate = DateTime.Now,
                                    Currency = Currency.VND,
                                    Language = DisplayLanguage.Vietnamese,
                                };

                                var currentOrder = _appContext.Orders.Find(dto.Id);
                                if (currentOrder == null) throw new Exception("Không tìm thấy đơn hàng.");

                                currentOrder.payment_id = request2.PaymentId;

                                var paymentUrl = _vnpay.GetPaymentUrl(request2);

                                _appContext.Orders.Update(currentOrder);
                                _appContext.SaveChanges();
                                ClearCart();

                                return Redirect(paymentUrl);
                            }
                            catch (Exception ex)
                            {
                                return BadRequest(ex.Message);
                            }
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Đặt hàng không thành công";
                        TempData["Success"] = false;
                    }
                    return RedirectToAction("", "Cart");
                }
                catch(Exception  ex)
                {
                    TempData["Message"] = "Lỗi";
                    TempData["Success"] = false;
                    return RedirectToAction("", "Cart");
                }
            }
            return RedirectToAction("Login", "Account");
        }
        
    }
}
