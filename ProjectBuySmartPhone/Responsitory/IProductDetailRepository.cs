using ProjectBuySmartPhone.Models.Domain.Entities;

namespace ProjectBuySmartPhone.Responsitory
{
    public interface IProductDetailRepository
    {
        ProductDetail add(ProductDetail productDetail);
        ProductDetail delete(string id);
        IQueryable<ProductDetail> GetAll();
        ProductDetail getProductDetail(string id);
        ProductDetail update(ProductDetail productDetail);
    }
}