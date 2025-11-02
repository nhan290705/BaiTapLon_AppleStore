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
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(Order order)
        {
            var userId = getCurrentUserId();
            if (userId == null)
            {
                Console.WriteLine("khong tim thay token");
                TempData["Error"] = "Không thể xác định người dùng. Vui lòng đăng nhập lại.";
                return RedirectToAction("Index", "Login", new { area = "Identity" });
            }
            var cart = GetCart();

            if (!cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng trống!";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Vui lòng nhập đầy đủ thông tin giao hàng!";
                return RedirectToAction("Index");
            }

            order.UserId = userId;
            order.StatusOrderId = 1; 
            order.TotalPrice = cart.TotalAmount;

            _context.Orders.Add(order);
            _context.SaveChanges();

            // ✅ Lưu OrderDetail cho từng sản phẩm trong giỏ
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
                _context.SaveChanges(); // để có OrderDetailId ngay

                // ✅ Nếu ProductDetail tồn tại thì gắn FK
                var productDetail = _context.ProductDetails
                    .FirstOrDefault(pd => pd.ProductId == item.ProductId && pd.OrderDetailId == null);

                if (productDetail != null)
                {
                    productDetail.OrderDetailId = orderDetail.OrderDetailId;
                    _context.ProductDetails.Update(productDetail);
                }

                // ✅ Giảm số lượng tồn kho của sản phẩm
                var product = _context.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (product != null)
                {
                    product.Qty -= item.Quantity;
                }
            }

            _context.SaveChanges();

            HttpContext.Session.Remove("Cart");
            TempData["Success"] = "Đặt hàng thành công!";
            return RedirectToAction("Success");
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
