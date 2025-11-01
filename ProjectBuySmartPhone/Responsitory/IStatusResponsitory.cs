using ProjectBuySmartPhone.Models;
using ProjectBuySmartPhone.Models.Domain.Entities;
namespace ProjectBuySmartPhone.Responsitory
{
    public interface IStatusResponsitory
    {
        StatusOrder add(StatusOrder statusOrder);
        StatusOrder update(StatusOrder statusOrder);
        StatusOrder delete(int  id);
        StatusOrder getStatusOrder(int id);
        IEnumerable<StatusOrder> GetAll();
    }
}
