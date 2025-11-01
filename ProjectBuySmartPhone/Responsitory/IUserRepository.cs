using ProjectBuySmartPhone.Models.Domain.Entities;

namespace ProjectBuySmartPhone.Responsitory
{
    public interface IUserRepository
    {
        void add(User user);
        void update(User user);
        void delete(User user);
        IQueryable<User> GetAll();
        User GetById(int id);
    }
}
