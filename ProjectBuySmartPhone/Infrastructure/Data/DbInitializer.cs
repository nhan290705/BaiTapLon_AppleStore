//using Microsoft.EntityFrameworkCore;
//using ProjectBuySmartPhone.Models.Domain.Entities;

//namespace ProjectBuySmartPhone.Models.Infrastructure
//{
//    public class DbInitializer
//    {
//        public static void Initialize(IServiceProvider serviceProvider)
//        {
//            using (var context = new MyDbContext(
//                serviceProvider.GetRequiredService<DbContextOptions<MyDbContext>>()))
//            {
//                // Tạo database nếu chưa tồn tại
//                context.Database.EnsureCreated();

//                // 1️⃣ Kiểm tra nếu đã có dữ liệu rồi thì bỏ qua
//                if (context.Users.Any())
//                {
//                    return;
//                }


//                // 2️⃣ Seed dữ liệu mẫu cho User
//                var users = new User[]
//                {
//                    new User { FirstName = "Admin",LastName = "Han", Email = "admin@gmail.com", Password = "admin", Role = "Admin", IsActive = Domain.Enums.ActiveStatus.Active },

//                };
//                foreach (var u in users)
//                    context.Users.Add(u);
//                context.SaveChanges();
//            }
//        }
//    }
//}
