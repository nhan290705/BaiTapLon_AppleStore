using ProjectBuySmartPhone.Models;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ProjectBuySmartPhone.Responsitory
{
    public class OrderRepository : IOrderRepository
    {
        private readonly MyDbContext _db;

        public OrderRepository(MyDbContext db)
        {
            _db = db;
        }

        public Order add(Order order)
        {
            _db.Orders.Add(order);
            _db.SaveChanges();
            return order;
        }

        public Order delete(string id)
        {
            var order = _db.Orders.Find(id);
            if (order != null)
            {
                _db.Orders.Remove(order);
                _db.SaveChanges();
            }
            return order;
        }

        public IQueryable<Order> GetAll()
        {
            return _db.Orders.AsNoTracking();
        }

        public Order getOrder(string id)
        {
            return _db.Orders.Find(id);
        }

        public Order update(Order order)
        {
            _db.Update(order);
            _db.SaveChanges();
            return order;
        }
    }
}