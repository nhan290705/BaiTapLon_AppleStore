using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace ProjectBuySmartPhone.Responsitory
{
    public class ProductRepository : IProductRepository
    {
        private readonly MyDbContext _db;

        public ProductRepository(MyDbContext db)
        {
            _db = db;
        }

        public Product add(Product product)
        {
            _db.Products.Add(product);
            _db.SaveChanges();
            return product;
        }

        public Product delete(string id)
        {
            var product = _db.Products.Find(id);
            if (product != null)
            {
                _db.Products.Remove(product);
                _db.SaveChanges();
            }
            return product;
        }

        public IQueryable<Product> GetAll()
        {
            return _db.Products.AsNoTracking();
        }

        public Product getProduct(string id)
        {
            return _db.Products.Find(id);
        }

        public Product update(Product product)
        {
            _db.Update(product);
            _db.SaveChanges();
            return product;
        }
    }
}