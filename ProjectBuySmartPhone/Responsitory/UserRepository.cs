using ProjectBuySmartPhone.Models.Domain.Entities;
using ProjectBuySmartPhone.Models.Infrastructure;

namespace ProjectBuySmartPhone.Responsitory
{
    public class UserRepository : IUserRepository
    {
        private readonly MyDbContext _context;

        public UserRepository(MyDbContext db)
        {
            _context = db;
        }

        public void add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public void update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void delete(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public IQueryable<User> GetAll()
        {
            return _context.Users;
        }

        public User GetById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.UserId == id);
        }
    }
}
