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


                //// 2️⃣ Seed dữ liệu mẫu cho User
                //var users = new User[]
                //{
                //    new User { FullName = "Admin", Email = "admin@gmail.com", Password = "123456", Role = "Admin", IsActive = Domain.Enums.ActiveStatus.Active },
                //    new User { FullName = "John Doe", Email = "john@gmail.com", Password = "123456", Role = "Customer", IsActive = Domain.Enums.ActiveStatus.Active },
                //    new User { FullName = "Jane Smith", Email = "jane@gmail.com", Password = "123456", Role = "Customer", IsActive = Domain.Enums.ActiveStatus.Active }
                //};
                //foreach (var u in users)
                //    context.Users.Add(u);
                //context.SaveChanges();

