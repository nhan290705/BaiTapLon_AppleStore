namespace ProjectBuySmartPhone.Models.Domain.Entities
{
    public class DashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ProcessingOrders { get; set; }
        public int CompletedOrders { get; set; }

        public int TotalCustomers { get; set; }
        public int NewCustomersToday { get; set; }
        public int NewCustomersYesterday { get; set; }
        public int NewCustomersLastWeek { get; set; }

        public int TotalProducts { get; set; }
        public int InStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }
        public int TotalProductDetails { get; set; }

        public decimal TotalRevenue { get; set; }
        public decimal RevenueToday { get; set; }
        public decimal RevenueYesterday { get; set; }
        public decimal RevenueLastWeek { get; set; }
        public decimal RevenueLastMonth { get; set; }

        // Danh sách chi tiết
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public List<User> RecentCustomers { get; set; } = new List<User>();
        public List<TopProductViewModel> TopSellingProducts { get; set; } = new List<TopProductViewModel>();
        public List<OrderStatusViewModel> OrdersByStatus { get; set; } = new List<OrderStatusViewModel>();
        public List<DailyRevenueViewModel> Last7DaysRevenue { get; set; } = new List<DailyRevenueViewModel>();
        public List<CategoryStatViewModel> ProductsByCategory { get; set; } = new List<CategoryStatViewModel>();
    }

    // Top sản phẩm bán chạy
    public class TopProductViewModel
    {
        public Product Product { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    // Thống kê theo trạng thái đơn hàng
    public class OrderStatusViewModel
    {
        public int StatusId { get; set; }
        public string StatusName { get; set; }
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
    }

    // Doanh thu theo ngày
    public class DailyRevenueViewModel
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class CategoryStatViewModel
    {
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public int TotalStock { get; set; }
    }
}

