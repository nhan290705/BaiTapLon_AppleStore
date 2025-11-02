using Microsoft.AspNetCore.Mvc;
using ProjectBuySmartPhone.Models.Infrastructure;

namespace ProjectBuySmartPhone.Areas.Checkout
{
    public class CheckoutController : Controller
    {
        private readonly ILogger<CheckoutController> _logger;
        private readonly MyDbContext _context;
        public CheckoutController(ILogger<CheckoutController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

    }
}
