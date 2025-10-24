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
//                    new User { FullName = "Admin", Email = "admin@gmail.com", PasswordHash = "123456", Role = "Admin", IsActive = Domain.Enums.ActiveStatus.Active },
//                    new User { FullName = "John Doe", Email = "john@gmail.com", PasswordHash = "123456", Role = "Customer", IsActive = Domain.Enums.ActiveStatus.Active },
//                    new User { FullName = "Jane Smith", Email = "jane@gmail.com", PasswordHash = "123456", Role = "Customer", IsActive = Domain.Enums.ActiveStatus.Active }
//                };
//                foreach (var u in users)
//                    context.Users.Add(u);
//                context.SaveChanges();

//                // 3️⃣ Seed dữ liệu cho ProductCategory
//                var categories = new ProductCategory[]
//                {
//                    new ProductCategory { CategoryName = "Smartphone" },
//                    new ProductCategory { CategoryName = "Tablet" },
//                    new ProductCategory { CategoryName = "Accessories" }
//                };
//                foreach (var c in categories)
//                    context.ProductCategories.Add(c);
//                context.SaveChanges();

//                // 4️⃣ Seed dữ liệu cho Product
//                var products = new Product[]
//                {
//                    new Product
//                    {
//                        ProductName = "iPhone 15 Pro",
//                        Description = "Flagship Apple 2025",
//                        Price = 35000,
//                        Qty = 10,
//                        Discount = 0,
//                        ProductCategoryId = 1,
//                        Status = Domain.Enums.ActiveStatus.Active
//                    },
//                    new Product
//                    {
//                        ProductName = "Samsung Galaxy S24 Ultra",
//                        Description = "Flagship Samsung 2025",
//                        Price = 32000,
//                        Qty = 8,
//                        Discount = 5,
//                        ProductCategoryId = 1,
//                        Status = Domain.Enums.ActiveStatus.Active
//                    },
//                    new Product
//                    {
//                        ProductName = "iPad Air M2",
//                        Description = "Tablet hiệu năng cao của Apple",
//                        Price = 22000,
//                        Qty = 5,
//                        Discount = 0,
//                        ProductCategoryId = 2,
//                        Status = Domain.Enums.ActiveStatus.Active
//                    }
//                };
//                foreach (var p in products)
//                    context.Products.Add(p);
//                context.SaveChanges();

//                // 5️⃣ Seed dữ liệu cho ProductDetail
//                var productDetails = new ProductDetail[]
//                {
//                    new ProductDetail { ProductId = 1, Color = "Silver", Size = "256GB", Sku = "IP15PRO256", Qty = 5, Price = 35000 },
//                    new ProductDetail { ProductId = 2, Color = "Black", Size = "512GB", Sku = "SSS24U512", Qty = 4, Price = 32000 },
//                    new ProductDetail { ProductId = 3, Color = "Blue", Size = "128GB", Sku = "IPADM2128", Qty = 3, Price = 22000 }
//                };
//                foreach (var d in productDetails)
//                    context.ProductDetails.Add(d);
//                context.SaveChanges();

//                // 6️⃣ Seed dữ liệu cho ProductImage
//                var productImages = new ProductImage[]
//                {
//                    new ProductImage { ProductId = 1, ImageUrl = "iphone15pro.jpg", IsMain = true },
//                    new ProductImage { ProductId = 2, ImageUrl = "s24ultra.jpg", IsMain = true },
//                    new ProductImage { ProductId = 3, ImageUrl = "ipadairm2.jpg", IsMain = true }
//                };
//                foreach (var img in productImages)
//                    context.ProductImages.Add(img);
//                context.SaveChanges();

//                // 7️⃣ Seed dữ liệu cho Blog
//                var blogs = new Blog[]
//                {
//                    new Blog { Title = "Đánh giá iPhone 15 Pro", Content = "Chiếc iPhone mới nhất từ Apple.", UserId = 1 },
//                    new Blog { Title = "So sánh Galaxy S24 và iPhone 15", Content = "Hai flagship mạnh nhất hiện nay.", UserId = 1 }
//                };
//                foreach (var b in blogs)
//                    context.Blogs.Add(b);
//                context.SaveChanges();

//                // 8️⃣ Seed dữ liệu cho Order
//                var orders = new Order[]
//                {
//                    new Order
//                    {
//                        UserId = 2,
//                        RecipientName = "John Doe",
//                        RecipientPhone = "0909000000",
//                        ShippingAddress = "Hà Nội",
//                        City = "Hà Nội",
//                        Status = Domain.Enums.OrderStatus.Paid,
//                        PaymentMethod = Domain.Enums.PaymentMethod.CreditCard
//                    },
//                    new Order
//                    {
//                        UserId = 3,
//                        RecipientName = "Jane Smith",
//                        RecipientPhone = "0909111111",
//                        ShippingAddress = "TP.HCM",
//                        City = "TP.HCM",
//                        Status = Domain.Enums.OrderStatus.Pending,
//                        PaymentMethod = Domain.Enums.PaymentMethod.Cod
//                    }
//                };
//                foreach (var o in orders)
//                    context.Orders.Add(o);
//                context.SaveChanges();

//                // 9️⃣ Seed dữ liệu cho OrderDetail
//                var orderDetails = new OrderDetail[]
//                {
//                    new OrderDetail { OrderId = 1, ProductDetailId = 1, Qty = 1, UnitPrice = 35000, LineTotal = 35000 },
//                    new OrderDetail { OrderId = 2, ProductDetailId = 2, Qty = 1, UnitPrice = 32000, LineTotal = 32000 }
//                };
//                foreach (var od in orderDetails)
//                    context.OrderDetails.Add(od);
//                context.SaveChanges();
//            }
//        }
//    }
//}

