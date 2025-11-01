using ProjectBuySmartPhone.Models.Domain.Entities;

namespace ProjectBuySmartPhone.Responsitory
{
    public interface IOrderDetailRepository
    {
        OrderDetail add(OrderDetail orderDetail);
        OrderDetail delete(int id);
        IQueryable<OrderDetail> GetAll();
        OrderDetail getOrderDetail(int id);
        OrderDetail update(OrderDetail orderDetail);
        IQueryable<OrderDetail> GetByOrderId(int orderId); // Lấy chi tiết theo OrderId
    }
}