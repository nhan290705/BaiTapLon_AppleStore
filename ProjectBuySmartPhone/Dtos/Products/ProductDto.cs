using ProjectBuySmartPhone.Models.Domain.Entities;
using System.Linq.Expressions;

namespace ProjectBuySmartPhone.Dtos.Products
{
    public class ProductDto
    {
        public int ProductId { get; init; }        // dùng làm "#"
        public string ProductName { get; init; } = "";
        public string Color { get; init; } = "";
        public string Port { get; init; } = "";
        public string Ram { get; init; }
        public int Qty { get; init; }
        public decimal Discount { get; init; }
        public decimal Price { get; init; }

        // Projector để EF Core có thể translate sang SQL
        public static readonly Expression<Func<Product, ProductDto>> Selector
            = p => new ProductDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Color = p.Color,
                Port = p.Port,
                Ram = p.Ram,
                Qty = p.Qty,
                Discount = p.Discount,
                Price = p.Price
            };
    }
}
