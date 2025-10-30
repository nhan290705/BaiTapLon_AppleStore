using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace ProjectBuySmartPhone.Responsitory
{
    public class ProductDetailRepository : IProductDetailRepository
    {
        private readonly MyDbContext _db;

        public ProductDetailRepository(MyDbContext db)
        {
            _db = db;
        }

        public ProductDetail add(ProductDetail productDetail)
        {
            _db.ProductDetails.Add(productDetail);
            _db.SaveChanges();
            return productDetail;
        }

        public ProductDetail delete(string id)
        {
            var productDetail = _db.ProductDetails.Find(id);
            if (productDetail != null)
            {
                _db.ProductDetails.Remove(productDetail);
                _db.SaveChanges();
            }
            return productDetail;
        }

        public IQueryable<ProductDetail> GetAll()
        {
            return _db.ProductDetails.AsNoTracking();
        }

        public ProductDetail getProductDetail(string id)
        {
            return _db.ProductDetails.Find(id);
        }

        public ProductDetail update(ProductDetail productDetail)
        {
            _db.Update(productDetail);
            _db.SaveChanges();
            return productDetail;
        }
    }
}