using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBuySmartPhone.Responsitory;
using ProjectBuySmartPhone.Models.Domain.Entities;

namespace ProjectBuySmartPhone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class DashboardController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductDetailRepository _productDetailRepository;

        public DashboardController(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IProductRepository productRepository,
            IProductDetailRepository productDetailRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
            _productDetailRepository = productDetailRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = new DashboardViewModel();

            try
            {
                var today = DateTime.Today;
                var yesterday = today.AddDays(-1);
                var lastWeek = today.AddDays(-7);
                var lastMonth = today.AddMonths(-1);
                var lastYear = today.AddYears(-1);

                // 📊 Thống kê đơn hàng
                var allOrders = _orderRepository.GetAll()
                    .Include(o => o.StatusOrder)   // ✅ thêm dòng này
                    .ToList();

                viewModel.TotalOrders = allOrders.Count;
                viewModel.PendingOrders = allOrders.Count(o => o.StatusOrderId == 1);
                viewModel.ProcessingOrders = allOrders.Count(o => o.StatusOrderId == 2 || o.StatusOrderId == 3);
                viewModel.CompletedOrders = allOrders.Count(o => o.StatusOrderId == 4);

                // 📊 Thống kê khách hàng
                var allUsers = _userRepository.GetAll().Where(u => u.Role == "USER").ToList();

                viewModel.TotalCustomers = allUsers.Count;
                viewModel.NewCustomersToday = allUsers.Count(u => u.CreatedAt >= today);
                viewModel.NewCustomersYesterday = allUsers.Count(u => u.CreatedAt >= yesterday && u.CreatedAt < today);
                viewModel.NewCustomersLastWeek = allUsers.Count(u => u.CreatedAt >= lastWeek);

                // 📊 Thống kê sản phẩm
                var allProducts = _productRepository.GetAll().ToList();
                var allProductDetails = _productDetailRepository.GetAll().ToList();

                viewModel.TotalProducts = allProducts.Count;
                viewModel.InStockProducts = allProducts.Count(p => p.Qty > 0);
                viewModel.OutOfStockProducts = allProducts.Count(p => p.Qty == 0);
                viewModel.TotalProductDetails = allProductDetails.Count;

                // 💰 Thống kê doanh thu
                viewModel.TotalRevenue = allOrders.Sum(o => o.TotalPrice ?? 0);
                viewModel.RevenueToday = allOrders.Where(o => o.CreatedAt >= today).Sum(o => o.TotalPrice ?? 0);
                viewModel.RevenueYesterday = allOrders.Where(o => o.CreatedAt >= yesterday && o.CreatedAt < today).Sum(o => o.TotalPrice ?? 0);
                viewModel.RevenueLastWeek = allOrders.Where(o => o.CreatedAt >= lastWeek).Sum(o => o.TotalPrice ?? 0);
                viewModel.RevenueLastMonth = allOrders.Where(o => o.CreatedAt >= lastMonth).Sum(o => o.TotalPrice ?? 0);

                // 📈 Đơn hàng gần đây (5 đơn mới nhất)
                viewModel.RecentOrders = _orderRepository.GetAll()
                    .Include(o => o.StatusOrder)
                    .Include(o => o.User)
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(5)
                    .ToList();

                // 👥 Khách hàng mới nhất (5 người)
                viewModel.RecentCustomers = _userRepository.GetAll()
                    .Where(u => u.Role == "USER")
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .ToList();

                // 📦 Top 5 sản phẩm bán chạy
                viewModel.TopSellingProducts = _productDetailRepository.GetAll()
                    .Include(pd => pd.Product)
                        .ThenInclude(p => p.ProductCategory)
                    .Where(pd => pd.OrderDetailId != null)
                    .GroupBy(pd => pd.ProductId)
                    .Select(g => new TopProductViewModel
                    {
                        Product = g.First().Product,
                        TotalSold = g.Count(),
                        TotalRevenue = g.Sum(pd => pd.Product.Price)
                    })
                    .OrderByDescending(tp => tp.TotalSold)
                    .Take(5)
                    .ToList();

                viewModel.OrdersByStatus = allOrders
                    .GroupBy(o => o.StatusOrderId)
                    .Select(g => new OrderStatusViewModel
                    {
                        StatusId = g.Key,
                        StatusName = g.First().StatusOrder?.Name ?? "N/A",
                        Count = g.Count(),
                        TotalAmount = g.Sum(o => o.TotalPrice ?? 0)
                    })
                    .OrderBy(os => os.StatusId)
                    .ToList();

                // 📈 Doanh thu 7 ngày gần đây
                viewModel.Last7DaysRevenue = Enumerable.Range(0, 7)
                    .Select(i => today.AddDays(-i))
                    .Select(date => new DailyRevenueViewModel
                    {
                        Date = date,
                        Revenue = allOrders
                            .Where(o => o.CreatedAt >= date && o.CreatedAt < date.AddDays(1))
                            .Sum(o => o.TotalPrice ?? 0),
                        OrderCount = allOrders
                            .Count(o => o.CreatedAt >= date && o.CreatedAt < date.AddDays(1))
                    })
                    .OrderBy(dr => dr.Date)
                    .ToList();

                // 📊 Thống kê sản phẩm theo danh mục
                viewModel.ProductsByCategory = _productRepository.GetAll()
                    .Include(p => p.ProductCategory)
                    .GroupBy(p => p.ProductCategory.CategoryName)
                    .Select(g => new CategoryStatViewModel
                    {
                        CategoryName = g.Key,
                        ProductCount = g.Count(),
                        TotalStock = g.Sum(p => p.Qty)
                    })
                    .OrderByDescending(cs => cs.ProductCount)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"✅ Dashboard loaded successfully");
                System.Diagnostics.Debug.WriteLine($"📊 Total Orders: {viewModel.TotalOrders}");
                System.Diagnostics.Debug.WriteLine($"👥 Total Customers: {viewModel.TotalCustomers}");
                System.Diagnostics.Debug.WriteLine($"💰 Total Revenue: {viewModel.TotalRevenue:N0} VNĐ");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading dashboard: {ex.Message}");
            }

            return View(viewModel);
        }
    }
}
