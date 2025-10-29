using ProjectBuySmartPhone.Models;
using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;

namespace ProjectBuySmartPhone.Responsitory
{
    public class StatusResponsitory : IStatusResponsitory
    {
        private readonly MyDbContext _db;
        public StatusResponsitory(MyDbContext db)
        {
            _db = db;
        }
        public StatusOrder add(StatusOrder statusOrder)
        {
            _db.StatusOrders.Add(statusOrder);
            _db.SaveChanges();
            return statusOrder;
        }

        public StatusOrder delete(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StatusOrder> GetAll()
        {
            return _db.StatusOrders;
        }

        public StatusOrder getStatusOrder(int id)
        {
            return _db.StatusOrders.Find(id);
        }

        public StatusOrder update(StatusOrder statusOrder)
        {
            _db.Update(statusOrder);
            _db.SaveChanges();
            return statusOrder;

        }
    }
}
