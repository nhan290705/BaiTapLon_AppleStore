using ProjectBuySmartPhone.Models.Domain.Entities;

namespace ProjectBuySmartPhone.Responsitory
{
    public interface IProductRepository
    {
        Product add(Product product);
        Product delete(string id);
        IQueryable<Product> GetAll();
        Product getProduct(string id);
        Product update(Product product);
    }
}