using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;
using Microsoft.EntityFrameworkCore;
namespace ProjectBuySmartPhone.Responsitory
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly MyDbContext _db;

        public OrderDetailRepository(MyDbContext db)
        {
            _db = db;
        }

        public OrderDetail add(OrderDetail orderDetail)
        {
            _db.OrderDetails.Add(orderDetail);
            _db.SaveChanges();
            return orderDetail;
        }

        public OrderDetail delete(int id)
        {
            var orderDetail = _db.OrderDetails.Find(id);
            if (orderDetail != null)
            {
                _db.OrderDetails.Remove(orderDetail);
                _db.SaveChanges();
            }
            return orderDetail;
        }

        public IQueryable<OrderDetail> GetAll()
        {
            return _db.OrderDetails.AsNoTracking();
        }

        public OrderDetail getOrderDetail(int id)
        {
            return _db.OrderDetails.Find(id);
        }

        public OrderDetail update(OrderDetail orderDetail)
        {
            _db.Update(orderDetail);
            _db.SaveChanges();
            return orderDetail;
        }

        public IQueryable<OrderDetail> GetByOrderId(int orderId)
        {
            return _db.OrderDetails
                .AsNoTracking()
                .Where(od => od.OrderId == orderId);
        }
    }
}