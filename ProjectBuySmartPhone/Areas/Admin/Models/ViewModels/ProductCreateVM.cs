using Microsoft.AspNetCore.Http;

namespace ProjectBuySmartPhone.Areas.Admin.Models.ViewModels
{
    // ProductId là IDENTITY tự tăng => không đưa vào VM
    public class ProductCreateVM
    {
        // Product
        public string ProductName { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public decimal Discount { get; set; }
        public string? Color { get; set; }
        public string Storage { get; set; }
        public string Ram { get; set; }
        public string? Port { get; set; }
        public byte Status { get; set; } = 1;
        public int ProductCategoryId { get; set; }   // nếu bạn muốn chỉ 1..5, view sẽ render radio 5 lựa chọn

        // ProductDetail
        public string Sku { get; set; } = string.Empty;

        // ProductImage (chỉ 1 ảnh)
        //public IFormFile? ImageFile { get; set; }
<<<<<<< HEAD
=======
        //[Required(ErrorMessage = "Phải chọn ít nhất một hình ảnh")]
>>>>>>> 4b77db63057788e4d2cc2fa7282cbe8bf46d3315
        public List<IFormFile>? ImageFiles { get; set; }

        // (tuỳ chọn) index ảnh chính người dùng chọn trên form; -1 = tự lấy file đầu tiên làm ảnh chính
        public int MainImageIndex { get; set; } = -1;
    }
}
