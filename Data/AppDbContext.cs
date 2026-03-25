using Microsoft.EntityFrameworkCore;
using HSU.PTWeb.AnhPH.BookStore.Models;

namespace HSU.PTWeb.AnhPH.BookStore.Data
{
    // DbContext chứa các DbSet tương ứng với các Model
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Các DbSet tương ứng với bảng trong cơ sở dữ liệu
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Ward> Wards { get; set; }
        //public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Thiết lập tên bảng nếu cần
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Product>().ToTable("Products");
            modelBuilder.Entity<Order>().ToTable("Orders");
            modelBuilder.Entity<OrderDetail>().ToTable("OrderDetails");
            modelBuilder.Entity<City>().ToTable("Cities");
            modelBuilder.Entity<Ward>().ToTable("Wards");
            //modelBuilder.Entity<Supplier>().ToTable("Suppliers");

            // Thiết lập khóa chính
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<Category>().HasKey(c => c.CategoryId);
            modelBuilder.Entity<Product>().HasKey(p => p.ProductId);
            modelBuilder.Entity<Order>().HasKey(o => o.OrderId);
            modelBuilder.Entity<OrderDetail>().HasKey(od => od.OrderDetailId);
            modelBuilder.Entity<City>().HasKey(c => c.CityId);
            modelBuilder.Entity<Ward>().HasKey(w => w.WardId);
            //modelBuilder.Entity<Supplier>().HasKey(s => s.SupplierId);

            // Quan hệ: Category 1 - n Product
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ: Order 1 - n OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ: Product 1 - n OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ: User 1 - n Order
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.AppUser)
                .WithMany(u => u.AppOrders)
                .HasForeignKey(o => o.AppUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Orders_AppUser");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.CityNavigation)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CityId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Orders_Cities");

            modelBuilder.Entity<Order>()
                .HasOne(o => o.WardNavigation)
                .WithMany(w => w.Orders)
                .HasForeignKey(o => o.WardId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Orders_Wards");

            // Quan hệ: City 1 - n Ward
            modelBuilder.Entity<Ward>()
                .HasOne(w => w.City)
                .WithMany(c => c.Wards)
                .HasForeignKey(w => w.CityId)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ: City 1 - n User
            modelBuilder.Entity<User>()
                .HasOne(u => u.City)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ: Ward 1 - n User
            modelBuilder.Entity<User>()
                .HasOne(u => u.Ward)
                .WithMany(w => w.Users)
                .HasForeignKey(u => u.WardId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<City>()
                .HasIndex(c => c.CityName)
                .IsUnique();

            modelBuilder.Entity<Ward>()
                .HasIndex(w => new { w.CityId, w.WardName })
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.AppUserId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => o.UserId);

            modelBuilder.Entity<Order>()
                .HasIndex(o => new { o.Status, o.OrderDate })
                .IsDescending(false, true);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.CityId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.WardId);

            modelBuilder.Entity<Order>()
                .Property(o => o.Channel)
                .HasDefaultValue("Online");

            // Thiết lập giá trị mặc định cho CreatedDate
            modelBuilder.Entity<User>()
                .Property(u => u.CreatedDate)
                .HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<Product>()
                .Property(p => p.CreatedDate)
                .HasDefaultValueSql("getutcdate()");
        }
    }
}
