using ProjectBuySmartPhone.Models.Domain.Entities;

namespace ProjectBuySmartPhone.Responsitory
{
    public interface IOrderRepository
    {
        Order add(Order order);
        Order update(Order order);
        Order delete(int id);
        Order getOrder(int id);
        IQueryable<Order> GetAll(); // Dùng IQueryable để có thể filter sau
    }
}