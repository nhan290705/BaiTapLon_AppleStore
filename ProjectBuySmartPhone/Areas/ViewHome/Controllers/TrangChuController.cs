using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;
using X.PagedList;

namespace ProjectBuySmartPhone.Areas.ViewHome.Controllers
{
    [Area("ViewHome")]
    public class TrangChuController : Controller
    {
        private readonly MyDbContext _db;
        private readonly IWebHostEnvironment _env;

        public TrangChuController(MyDbContext db)
        {
            this._db = db;
        }
        public IActionResult Index()
        {
            List<Product> listIphone = _db.Products
                                       .Include(p => p.ProductCategory)
                                       .Include(p => p.ProductImages)
                                       .Where(p => p.ProductCategory.CategoryName == "Iphone")
                                       .ToList();
            return View("TrangChu",listIphone);
        }
        public async Task<IActionResult> ListIphone(string keyWord, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Iphone"
                           && (String.IsNullOrEmpty(keyWord) || p.ProductName.ToLower().Contains(keyWord.Trim().ToLower())))
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);

          

            return View("ProductIphone", pagedList);
        }
        public async Task<IActionResult> InforIphone(string keyWord,int productId, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Iphone" && (String.IsNullOrEmpty(keyWord) || p.ProductName.ToLower().Contains(keyWord.Trim().ToLower())))
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);

            foreach (var a in query)
            {
                if (a.ProductId == productId)
                {
                    ViewBag.PP = a;
                    break;
                }
            }

            return View("ProductIphone", pagedList);
        }

        public async Task<IActionResult> ListMac(string keyWord,int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Mac" && (String.IsNullOrEmpty(keyWord) || p.ProductName.ToLower().Contains(keyWord.Trim().ToLower())))
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);



            return View("ProductMac", pagedList);
        }
        public async Task<IActionResult> InforMac(string keyWord, int productId, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Mac" && (String.IsNullOrEmpty(keyWord) || p.ProductName.ToLower().Contains(keyWord.Trim().ToLower())))
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);

            foreach (var a in query)
            {
                if (a.ProductId == productId)
                {
                    ViewBag.PP = a;
                    break;
                }
            }

            return View("ProductMac", pagedList);
        }

        public async Task<IActionResult> ListIpad(string keyWord, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Ipad" && (String.IsNullOrEmpty(keyWord) || p.ProductName.ToLower().Contains(keyWord.Trim().ToLower())))
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);



            return View("ProductIpad", pagedList);
        }
        public async Task<IActionResult> InforIpad(string keyWord, int productId, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Ipad" && (String.IsNullOrEmpty(keyWord) || p.ProductName.ToLower().Contains(keyWord.Trim().ToLower())))
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);

            foreach (var a in query)
            {
                if (a.ProductId == productId)
                {
                    ViewBag.PP = a;
                    break;
                }
            }

            return View("ProductIpad", pagedList);
        }



        public async Task<IActionResult> ListAppleWatch(string keyWord, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "AppleWatch" && (String.IsNullOrEmpty(keyWord) || p.ProductName.ToLower().Contains(keyWord.Trim().ToLower())))
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);



            return View("ProductAppleWatch", pagedList);
        }
        public async Task<IActionResult> InforAppleWatch(string keyWord, int productId, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "AppleWatch" && (String.IsNullOrEmpty(keyWord) || p.ProductName.ToLower().Contains(keyWord.Trim().ToLower())))
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);

            foreach (var a in query)
            {
                if (a.ProductId == productId)
                {
                    ViewBag.PP = a;
                    break;
                }
            }

            return View("ProductAppleWatch", pagedList);
        }

        public async Task<IActionResult> ListAccessory(string keyWord, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Accessory" && (String.IsNullOrEmpty(keyWord) || p.ProductName.ToLower().Contains(keyWord.Trim().ToLower())))
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);



            return View("ProductAccessory", pagedList);
        }
        public async Task<IActionResult> InforAccessory(string keyWord, int productId, int page = 1)
        {
            const int pageSize = 8;

            var query = _db.Products
                           .AsNoTracking()
                           .Include(p => p.ProductCategory)
                           .Include(p => p.ProductImages)
                           .Where(p => p.ProductCategory.CategoryName == "Accessory" && (String.IsNullOrEmpty(keyWord) || p.ProductName.ToLower().Contains(keyWord.Trim().ToLower())))
                           .OrderBy(p => p.ProductName); // ổn định thứ tự

            var pagedList = await query.ToPagedListAsync(page, pageSize);

            foreach (var a in query)
            {
                if (a.ProductId == productId)
                {
                    ViewBag.PP = a;
                    break;
                }
            }

            return View("ProductAccessory", pagedList);
        }
    }
}
