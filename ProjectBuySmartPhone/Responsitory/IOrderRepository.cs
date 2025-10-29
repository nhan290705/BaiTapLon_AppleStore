using ProjectBuySmartPhone.Models.Domain.Entities;

namespace ProjectBuySmartPhone.Responsitory
{
    public interface IOrderRepository
    {
        Order add(Order order);
        Order update(Order order);
        Order delete(string id);
        Order getOrder(string id);
        IQueryable<Order> GetAll(); // Dùng IQueryable để có thể filter sau
    }
}