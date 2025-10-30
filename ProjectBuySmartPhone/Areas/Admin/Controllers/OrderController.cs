using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBuySmartPhone.Models;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;
using ProjectBuySmartPhone.Responsitory;
using X.PagedList;
namespace ProjectBuySmartPhone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class OrderController : Controller
    { 
        private readonly IOrderRepository _orderRepository;
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public IActionResult Index(string searchTerm, int? status, int? page)
        {
            int pageSize = 10;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            // Lấy query từ repository
            var query = _orderRepository.GetAll();

            // Tìm kiếm theo searchTerm
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var searchLower = searchTerm?.ToLower() ?? "";

                query = query.Where(o =>
                    o.OrderId.ToString().Contains(searchLower) ||
                    (o.RecipientName != null && o.RecipientName.ToLower().Contains(searchLower)) ||
                    (o.RecipientPhone != null && o.RecipientPhone.ToLower().Contains(searchLower))
                );

            }

            // Lọc theo status
            if (status.HasValue && status.Value > 0)
            {
                query = query.Where(o => o.StatusOrderId == status.Value);
            }

            // Sắp xếp
            query = query.OrderBy(x => x.OrderId);

            // Truyền giá trị vào ViewBag để giữ lại trong form
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Status = status;

            // Phân trang
            var pagedList = query.ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }
        public IActionResult Details(string id)
        {
            var order = _orderRepository.getOrder(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }
        public IActionResult Edit(string id)
        {
            var order = _orderRepository.getOrder(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        public IActionResult Edit(Order order)
        {
            if (ModelState.IsValid)
            {
                _orderRepository.update(order);
                return RedirectToAction("Index");
            }
            return View(order);
        }

        [Route("AddOrder")]
        [HttpGet]
        public IActionResult AddOrder()
        {
            return View();
        }

        [Route("AddOrder")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                // Tạo OrderId tự động
                order.OrderId = GenerateOrderId();

                order.CreatedAt = DateTime.Now;
                order.StatusOrderId = 1;

                _orderRepository.add(order);

                TempData["SuccessMessage"] = "Thêm đơn hàng thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        private int GenerateOrderId()
        {
            // Lấy số lượng order từ repository
            var count = _orderRepository.GetAll().Count(o => o.CreatedAt.Date == DateTime.Today) + 1;
            
            return count;
        }
    }
}
