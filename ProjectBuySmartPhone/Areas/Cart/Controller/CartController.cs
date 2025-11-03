using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectBuySmartPhone.Dtos.Cart;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Domain.Session;
using ProjectBuySmartPhone.Models.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProjectBuySmartPhone.Areas.Cart.Controllers
{
    [Area("Cart")] 
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly MyDbContext _context;
        public CartController(ILogger<CartController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet]   
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }
        private ShoppingCart? GetCart()
        {
            var cartData = HttpContext.Session.GetString("Cart");
            return cartData != null ? JsonConvert.DeserializeObject<ShoppingCart>(cartData) : new ShoppingCart();
        }
        private void saveCart(ShoppingCart cart)
        {
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }
        [HttpPost]
        public IActionResult AddCart(CartItemDto cartItem)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductId == cartItem.ProductId);
            if (product == null) return NotFound("Can not found product");
            if (cartItem.Quantity <= 0)
                cartItem.Quantity = 1;
            if (cartItem.Quantity > product.Qty)
            {
                TempData["Error"] = $"Chỉ còn {product.Qty} sản phẩm trong kho!";
                
            }
            var cart = GetCart();
            var existing = cart.Items.FirstOrDefault(x => x.ProductId == product.ProductId);
            if (existing != null)
            {
                // nếu sản phẩm đã có trong giỏ
                if (existing.Quantity + cartItem.Quantity > product.Qty)
                {
                    TempData["Error"] = $"Không thể thêm quá {product.Qty} sản phẩm!";
                    return RedirectToAction("Index");
                }

                existing.Quantity += cartItem.Quantity;
            }
            else
            {
                // thêm sản phẩm mới vào giỏ
                var mainImage = product.ProductImages?.FirstOrDefault(p => p.IsMain)?.ImageUrl
                    ?? product.ProductImages?.FirstOrDefault()?.ImageUrl
                    ?? "/images/img_store/imgProduct/default.png";

                var priceAfterDiscount = product.Price - (product.Price * product.Discount / 100);

                cart.Items.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ImageUrl = mainImage,
                    Price = priceAfterDiscount,
                    Quantity = cartItem.Quantity
                });
            }

            saveCart(cart);
            TempData["Success"] = "Đã thêm sản phẩm vào giỏ hàng!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item == null)
                return NotFound();

            if (quantity <= 0)
                cart.Items.Remove(item);
            else
                item.Quantity = quantity;

            saveCart(cart);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveItem(int productId)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);

            if (item != null)
            {
                cart.Items.Remove(item);
                saveCart(cart); // ⚡ BẮT BUỘC: Lưu lại giỏ hàng sau khi xóa
                TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy sản phẩm trong giỏ hàng.";
            }

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart"); // ⚡ Xóa toàn bộ key Cart
            TempData["Success"] = "Đã xóa toàn bộ giỏ hàng!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateQuantityAjax(int productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item == null)
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });

            var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
            if (product == null)
                return Json(new { success = false, message = "Sản phẩm không tồn tại trong hệ thống." });

            if (quantity <= 0)
            {
                cart.Items.Remove(item);
                saveCart(cart);
                return Json(new { success = true, remove = true, totalAmount = cart.TotalAmount.ToString("N0") });
            }

            if (quantity > product.Qty)
            {
                return Json(new { success = false, message = $"Chỉ còn {product.Qty} sản phẩm trong kho!" });
            }

            item.Quantity = quantity;
            saveCart(cart);

            return Json(new
            {
                success = true,
                newQuantity = item.Quantity,
                itemTotal = (item.Quantity * item.Price).ToString("N0"),
                totalAmount = cart.TotalAmount.ToString("N0")
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckoutConfirm(Order order)
        {
            var userId = getCurrentUserId();
            if (userId == null)
            {
                TempData["Error"] = "Không thể xác định người dùng. Vui lòng đăng nhập lại.";
                return RedirectToAction("Index", "Login", new { area = "Identity" });
            }

            var cart = GetCart();
            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin giao hàng!";
                return RedirectToAction("Index");
            }

            // ✅ B1: Tạo đơn hàng
            order.UserId = userId;
            order.StatusOrderId = 1; // 1 = Chờ xác nhận
            order.TotalPrice = cart.TotalAmount;
            _context.Orders.Add(order);
            _context.SaveChanges(); // cần để có OrderId

            // ✅ B2: Tạo chi tiết đơn hàng & ProductDetail tương ứng
            foreach (var item in cart.Items)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    Qty = item.Quantity,
                    UnitPrice = item.Price,
                    LineTotal = item.Quantity * item.Price
                };

                _context.OrderDetails.Add(orderDetail);
                _context.SaveChanges(); // để có OrderDetailId

                // ✅ B3: Tạo ProductDetail gắn với OrderDetailId (nếu ProductDetail rỗng)
                var productDetail = new ProductDetail
                {
                    ProductId = item.ProductId,
                    Sku = Guid.NewGuid().ToString(), // SKU sinh tự động
                    OrderDetailId = orderDetail.OrderDetailId
                };
                _context.ProductDetails.Add(productDetail);

                // ✅ B4: Trừ tồn kho sản phẩm
                var product = _context.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (product != null)
                {
                    product.Qty -= item.Quantity;
                    if (product.Qty < 0) product.Qty = 0; // tránh âm kho
                }
            }

            _context.SaveChanges();

            // ✅ B5: Dọn giỏ hàng
            HttpContext.Session.Remove("Cart");

            TempData["Success"] = "Đặt hàng thành công!";
            return RedirectToAction("Success");
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCart();
            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng trống, vui lòng thêm sản phẩm trước khi thanh toán.";
                return RedirectToAction("Index", "Cart");
            }
            // 🔹 Lấy User từ token
            var userId = getCurrentUserId();
            if (userId == null)
            {
                TempData["Error"] = "Vui lòng đăng nhập để thanh toán.";
                return RedirectToAction("Index", "Login", new { area = "Identity" });
            }

            var user = _context.User.FirstOrDefault(u => u.UserId == userId);

            // 🔹 Gắn thông tin mặc định vào ViewBag để hiển thị trong form
            var fullName = $"{user?.FirstName} {user?.LastName}".Trim();
            ViewBag.RecipientName = fullName != "" ? fullName : user?.Username ?? "Khách hàng";
            ViewBag.RecipientPhone = user?.PhoneNumber ?? "";
            ViewBag.PostalCode = GeneratePostalCode();

            return View(cart);
        }
        private string GeneratePostalCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        // -------------------------
        // 🟢 TRANG THÀNH CÔNG
        // -------------------------
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Success()
        {
            return View();
        }
        private int? getCurrentUserId()
        {
            try
            {
                var accessToken = Request.Cookies["AccessToken"];
                if (string.IsNullOrEmpty(accessToken))
                    return null;

                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(accessToken);
                var idClaim = token.Claims.FirstOrDefault(c => c.Type == "idUser");

                if (idClaim != null && int.TryParse(idClaim.Value, out int userId))
                    return userId;

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
