using HSU.PTWeb.AnhPH.BookStore.Models;

namespace HSU.PTWeb.AnhPH.BookStore.Data
{
    // Lớp khởi tạo dữ liệu mặc định (tạo một admin và vài danh mục/sản phẩm mẫu)
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // nếu đã có dữ liệu thì thôi
            if (context.Users.Any()) return;

            // Tạo admin và vài user mẫu
            var admin = new User
            {
                Email = "admin@bookstore.com",
                FullName = "Quản trị viên",
                Password = "123456", // Demo: lưu plain-text. Trong thực tế phải hash mật khẩu
                Role = "Admin"
            };

            var user1 = new User
            {
                Email = "user1@gmail.com",
                FullName = "Người dùng 1",
                Password = "123456",
                Role = "Customer"
            };

            var user2 = new User
            {
                Email = "user2@gmail.com",
                FullName = "Người dùng 2",
                Password = "123456",
                Role = "Customer"
            };

            context.Users.AddRange(admin, user1, user2);

            // Tạo 5 danh mục theo yêu cầu
            var catProgramming = new Category { CategoryName = "Programming", Description = "Sách lập trình" };
            var catBusiness = new Category { CategoryName = "Business", Description = "Sách kinh doanh" };
            var catScience = new Category { CategoryName = "Science", Description = "Sách khoa học" };
            var catNovel = new Category { CategoryName = "Novel", Description = "Tiểu thuyết" };
            var catChildren = new Category { CategoryName = "Children", Description = "Sách thiếu nhi" };

            context.Categories.AddRange(catProgramming, catBusiness, catScience, catNovel, catChildren);
            context.SaveChanges();

            // Tạo ít nhất 30 sản phẩm mẫu
            var products = new List<Product>();
            int idCounter = 1;
            // Helper để tạo sản phẩm nhanh
            void AddProduct(string name, decimal price, Category cat)
            {
                var p = new Product
                {
                    ProductName = name,
                    Description = name + " - Mô tả mẫu",
                    Price = price,
                    CategoryId = cat.CategoryId,
                    ImageUrl = $"products/sample{(idCounter % 6) + 1}.jpg",
                    Stock = 100,
                    CreatedDate = DateTime.UtcNow
                };
                products.Add(p);
                idCounter++;
            }

            // Thêm 30 sản phẩm, phân bổ vào các category
            for (int i = 1; i <= 6; i++) AddProduct($"Learn C# Volume {i}", 150000 + i * 10000, catProgramming);
            for (int i = 1; i <= 6; i++) AddProduct($"Business Strategy {i}", 200000 + i * 12000, catBusiness);
            for (int i = 1; i <= 6; i++) AddProduct($"Science Book {i}", 180000 + i * 9000, catScience);
            for (int i = 1; i <= 6; i++) AddProduct($"Novel Story {i}", 120000 + i * 5000, catNovel);
            for (int i = 1; i <= 6; i++) AddProduct($"Kids Book {i}", 80000 + i * 2000, catChildren);

            context.Products.AddRange(products);
            context.SaveChanges();
        }
    }
}
