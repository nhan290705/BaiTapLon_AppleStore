using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProjectBuySmartPhone.Dtos.Cart;
using ProjectBuySmartPhone.Models.Domain.Session;
using ProjectBuySmartPhone.Models.Infrastructure;

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
                return RedirectToAction("Index", "Store", new { area = "" });
            }
            return null;
        }

    }
}
