using Microsoft.EntityFrameworkCore;
using ProjectBuySmartPhone.Models.Domain.Entities;

namespace ProjectBuySmartPhone.Models.Infrastructure
{
    public class MyDbContext : DbContext
    {
        public MyDbContext()
        {
        }

        public MyDbContext(DbContextOptions options)
        : base(options)
        {
        } 
        public DbSet<User> Users { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<StatusOrder> StatusOrders { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            modelBuilder.Entity<User>().ToTable(nameof(User));
            modelBuilder.Entity<Blog>().ToTable(nameof(Blog));
            modelBuilder.Entity<BlogComment>().ToTable(nameof(BlogComment));
            modelBuilder.Entity<ProductCategory>().ToTable(nameof(ProductCategory));
            modelBuilder.Entity<Product>().ToTable(nameof(Product));
            modelBuilder.Entity<ProductImage>().ToTable(nameof(ProductImage));
            modelBuilder.Entity<ProductDetail>().ToTable(nameof(ProductDetail));
            modelBuilder.Entity<ProductComment>().ToTable(nameof(ProductComment));
            modelBuilder.Entity<Order>().ToTable(nameof(Order));
            modelBuilder.Entity<OrderDetail>().ToTable(nameof(OrderDetail));
            modelBuilder.Entity<StatusOrder>().ToTable(nameof(StatusOrder));  

            modelBuilder.Entity<Blog>()
                .HasOne(b => b.User)
                .WithMany(u => u.Blogs)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BlogComment>()
                .HasOne(bc => bc.User)
                .WithMany(u => u.BlogComments)
                .HasForeignKey(bc => bc.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ProductComment>()
                .HasOne(pc => pc.User)
                .WithMany(u => u.ProductComments)
                .HasForeignKey(pc => pc.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.NoAction); 
            modelBuilder.Entity<Order>()
                .HasOne(o => o.StatusOrder)
                .WithMany(s => s.Orders)
                .HasForeignKey(o => o.StatusOrderId)
                .OnDelete(DeleteBehavior.Restrict); // Không cho xóa status nếu đang có order sử dụng

            // BLOG COMMENT ↔ BLOG (có thể cascade an toàn)
            modelBuilder.Entity<BlogComment>()
                .HasOne(bc => bc.Blog)
                .WithMany(b => b.BlogComments)
                .HasForeignKey(bc => bc.BlogId)
                .OnDelete(DeleteBehavior.Cascade);

            // PRODUCT ↔ CATEGORY (tránh cascade để không xoá cả cây)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductCategory)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // PRODUCT IMAGES/DETAILS (cascade OK vì phụ thuộc hoàn toàn vào Product)
            modelBuilder.Entity<ProductImage>()
                .HasOne(i => i.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductDetail>()
                .HasOne(d => d.Product)
                .WithMany(p => p.ProductDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // PRODUCT COMMENT ↔ PRODUCT (cascade OK)
            modelBuilder.Entity<ProductComment>()
                .HasOne(c => c.Product)
                .WithMany(p => p.ProductComments)
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // ORDER DETAIL (tránh cascade vòng về kho)
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.ProductDetail)
                .WithMany(pd => pd.OrderDetails)
                .HasForeignKey(od => od.ProductDetailId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}