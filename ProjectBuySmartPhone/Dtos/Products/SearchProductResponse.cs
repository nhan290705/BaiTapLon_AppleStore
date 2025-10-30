using ProjectBuySmartPhone.Dtos.Common;
using ProjectBuySmartPhone.Models.Domain.Entities;

namespace ProjectBuySmartPhone.Dtos.Products
{
    public class SearchProductResponse : SearchPageRequest<Product>
    {
        public int? TotalRecords { get; set; }

        public List<ProductDto>? Products { get; set; }
    }
}
