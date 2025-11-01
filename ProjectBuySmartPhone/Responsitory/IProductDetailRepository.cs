using ProjectBuySmartPhone.Models.Domain.Entities;

namespace ProjectBuySmartPhone.Responsitory
{
    public interface IProductDetailRepository
    {
        ProductDetail add(ProductDetail productDetail);
        ProductDetail delete(int id);
        IQueryable<ProductDetail> GetAll();
        ProductDetail getProductDetail(int id);
        ProductDetail update(ProductDetail productDetail);
    }
}