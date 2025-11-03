using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBuySmartPhone.Models.Infrastructure;

namespace ProjectBuySmartPhone.Controllers
{
    public class StoreController : Controller
    {
        private readonly MyDbContext _context;
        public StoreController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductCategory)
                .Take(12)
                .ToList();

            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductCategory)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        public IActionResult Category(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            var products = _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductCategory)
                .Where(p => p.ProductCategory.CategoryName.ToLower() == id.ToLower())
                .ToList();

            ViewData["Title"] = id;
            return View("Index", products);
        }

    }
}
