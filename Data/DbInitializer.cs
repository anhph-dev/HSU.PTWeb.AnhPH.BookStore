using HSU.PTWeb.AnhPH.BookStore.Models;
using HSU.PTWeb.AnhPH.BookStore.Services;
using Microsoft.EntityFrameworkCore;

namespace HSU.PTWeb.AnhPH.BookStore.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            var wardByCity = new Dictionary<string, string[]>
            {
                ["TP. Hồ Chí Minh"] = new[] { "Phường Bến Nghé", "Phường Bến Thành", "Phường Tân Định", "Phường Cầu Ông Lãnh" },
                ["Hà Nội"] = new[] { "Phường Hàng Bạc", "Phường Cửa Nam", "Phường Láng", "Phường Việt Hưng" },
                ["Đà Nẵng"] = new[] { "Phường Thạch Thang", "Phường Hải Châu", "Phường An Hải", "Phường Hòa Cường" },
                ["Cần Thơ"] = new[] { "Phường An Cư", "Phường Tân An", "Phường Xuân Khánh", "Phường Hưng Lợi" },
                ["Bình Dương"] = new[] { "Phường Phú Cường", "Phường Chánh Nghĩa", "Phường Hiệp Thành", "Phường Lái Thiêu" }
            };

            if (!context.Cities.Any())
            {
                var cityMasters = wardByCity.Keys.Select(name => new City { CityName = name, IsActive = true }).ToList();
                context.Cities.AddRange(cityMasters);
                context.SaveChanges();
            }

            if (!context.Wards.Any())
            {
                var cityMap = context.Cities.ToDictionary(c => c.CityName, c => c.CityId);
                var wards = new List<Ward>();
                foreach (var pair in wardByCity)
                {
                    if (!cityMap.TryGetValue(pair.Key, out var cityId)) continue;
                    wards.AddRange(pair.Value.Select(w => new Ward { CityId = cityId, WardName = w, IsActive = true }));
                }
                context.Wards.AddRange(wards);
                context.SaveChanges();
            }

            if (context.Users.Any())
            {
                var cityMap = context.Cities.ToDictionary(c => c.CityName, c => c.CityId);
                var wardMap = context.Wards
                    .GroupBy(w => w.CityId)
                    .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.WardName, x => x.WardId));

                var existingUsers = context.Users.ToList();
                foreach (var u in existingUsers)
                {
                    if (!u.CityId.HasValue)
                    {
                        var defaultCityName = wardByCity.Keys.First();
                        var cityName = defaultCityName;

                        if (cityMap.TryGetValue(cityName, out var cid))
                        {
                            u.CityId = cid;
                            if (!u.WardId.HasValue && wardMap.TryGetValue(cid, out var wardByName))
                            {
                                var defaultWard = wardByName.Values.FirstOrDefault();
                                if (defaultWard > 0)
                                    u.WardId = defaultWard;
                            }
                        }
                    }
                    else if (!u.WardId.HasValue && u.CityId.HasValue && wardMap.TryGetValue(u.CityId.Value, out var wardsOfCity))
                    {
                        var defaultWard = wardsOfCity.Values.FirstOrDefault();
                        if (defaultWard > 0)
                            u.WardId = defaultWard;
                    }
                }

                var existingOrders = context.Orders.ToList();
                foreach (var o in existingOrders)
                {
                    if (string.IsNullOrWhiteSpace(o.Ward) && !string.IsNullOrWhiteSpace(o.City) && wardByCity.TryGetValue(o.City, out var wards) && wards.Length > 0)
                    {
                        o.Ward = wards[0];
                    }
                }

                context.SaveChanges();
                return;
            }

            var hasher = new PasswordHasher();

            var cityMapSeed = context.Cities.ToDictionary(c => c.CityName, c => c.CityId);
            var wardMapSeed = context.Wards
                .Include(w => w.City)
                .ToList()
                .GroupBy(w => w.City.CityName)
                .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.WardName, x => x.WardId));

            // ===== USERS =====
            var users = new List<User>
            {
                new User { Email = "admin@bookstore.com", FullName = "Quản trị viên", PasswordHash = hasher.HashPassword("Admin@123"), Role = "Admin", PhoneNumber = "0901234567", Address = "123 Nguyễn Huệ", CityId = cityMapSeed["TP. Hồ Chí Minh"], WardId = wardMapSeed["TP. Hồ Chí Minh"]["Phường Bến Nghé"], CreatedDate = DateTime.Now },
                new User { Email = "customer1@gmail.com", FullName = "Nguyễn Văn A", PasswordHash = hasher.HashPassword("Customer@123"), Role = "Customer", PhoneNumber = "0912345678", Address = "45 Lê Lợi", CityId = cityMapSeed["TP. Hồ Chí Minh"], WardId = wardMapSeed["TP. Hồ Chí Minh"]["Phường Bến Thành"], CreatedDate = DateTime.Now.AddDays(-30) },
                new User { Email = "customer2@gmail.com", FullName = "Trần Thị B", PasswordHash = hasher.HashPassword("Customer@123"), Role = "Customer", PhoneNumber = "0923456789", Address = "78 Trần Hưng Đạo", CityId = cityMapSeed["Hà Nội"], WardId = wardMapSeed["Hà Nội"]["Phường Hàng Bạc"], CreatedDate = DateTime.Now.AddDays(-25) },
                new User { Email = "customer3@gmail.com", FullName = "Lê Văn C", PasswordHash = hasher.HashPassword("Customer@123"), Role = "Customer", PhoneNumber = "0934567890", Address = "12 Pasteur", CityId = cityMapSeed["Đà Nẵng"], WardId = wardMapSeed["Đà Nẵng"]["Phường Thạch Thang"], CreatedDate = DateTime.Now.AddDays(-20) },
                new User { Email = "customer4@gmail.com", FullName = "Phạm Thị D", PasswordHash = hasher.HashPassword("Customer@123"), Role = "Customer", PhoneNumber = "0945678901", Address = "56 Điện Biên Phủ", CityId = cityMapSeed["Cần Thơ"], WardId = wardMapSeed["Cần Thơ"]["Phường An Cư"], CreatedDate = DateTime.Now.AddDays(-15) },
            };
            context.Users.AddRange(users);

            // ===== CATEGORIES =====
            var categories = new List<Category>
            {
                new Category { CategoryName = "Văn học Việt Nam",      Description = "Tiểu thuyết, truyện ngắn của các tác giả Việt Nam" },
                new Category { CategoryName = "Văn học nước ngoài",    Description = "Tiểu thuyết dịch từ nước ngoài" },
                new Category { CategoryName = "Kỹ năng sống",          Description = "Sách phát triển bản thân, tư duy" },
                new Category { CategoryName = "Kinh tế - Kinh doanh",  Description = "Sách về kinh tế, khởi nghiệp, quản lý" },
                new Category { CategoryName = "Thiếu nhi",             Description = "Sách dành cho trẻ em" },
                new Category { CategoryName = "Công nghệ - Lập trình", Description = "Sách về công nghệ thông tin và lập trình" },
                new Category { CategoryName = "Tâm lý - Tình cảm",    Description = "Sách về tâm lý học và tình cảm" },
                new Category { CategoryName = "Lịch sử - Chính trị",  Description = "Sách về lịch sử và chính trị" },
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            var vn      = categories[0];
            var foreign = categories[1];
            var skill   = categories[2];
            var biz     = categories[3];
            var child   = categories[4];
            var tech    = categories[5];
            var psych   = categories[6];
            var history = categories[7];

            // ===== PRODUCTS (32 books) =====
            var products = new List<Product>
            {
                // -- Văn học nước ngoài --
                new Product { ProductName = "Nhà Giả Kim",                    Author = "Paulo Coelho",               ISBN = "9786042148393", Publisher = "NXB Hội Nhà Văn",       PublicationYear = 2020, Language = "Tiếng Việt", PageCount = 256, CoverType = "Bìa mềm", Weight = 300, Price = 120000, OriginalPrice = 150000, DiscountPercent = 20, Stock = 50, SoldCount = 234, ShortDescription = "Tác phẩm kinh điển về hành trình tìm kiếm ước mơ",         Category = foreign, ImageUrl = "products/nha-gia-kim.jpg",          IsFeatured = true,  IsBestSeller = true,  IsAvailable = true, AverageRating = 4.8m, ReviewCount = 1250 },
                new Product { ProductName = "Cây Cam Ngọt Của Tôi",           Author = "José Mauro de Vasconcelos",  ISBN = "9786042271667", Publisher = "NXB Hội Nhà Văn",       PublicationYear = 2019, Language = "Tiếng Việt", PageCount = 244, CoverType = "Bìa mềm", Weight = 280, Price = 108000, OriginalPrice = 140000, DiscountPercent = 23, Stock = 35, SoldCount = 456, ShortDescription = "Câu chuyện cảm động về tuổi thơ nghèo khó",                Category = foreign, ImageUrl = "products/cay-cam-ngot.jpg",         IsFeatured = true,  IsBestSeller = true,  IsAvailable = true, AverageRating = 4.9m, ReviewCount = 2340 },
                new Product { ProductName = "Harry Potter và Hòn Đá Phù Thủy",Author = "J.K. Rowling",               ISBN = "9786041234567", Publisher = "NXB Trẻ",               PublicationYear = 2021, Language = "Tiếng Việt", PageCount = 352, CoverType = "Bìa mềm", Weight = 380, Price = 145000, OriginalPrice = 165000, DiscountPercent = 12, Stock = 60, SoldCount = 890, ShortDescription = "Tập 1 của bộ truyện Harry Potter huyền thoại",          Category = foreign, ImageUrl = "products/placeholder.svg",          IsFeatured = true,  IsBestSeller = true,  IsNewArrival = true, IsAvailable = true, AverageRating = 4.9m, ReviewCount = 3456 },
                new Product { ProductName = "Rừng Nauy",                       Author = "Haruki Murakami",            ISBN = "9786042456789", Publisher = "NXB Hội Nhà Văn",       PublicationYear = 2019, Language = "Tiếng Việt", PageCount = 448, CoverType = "Bìa mềm", Weight = 470, Price = 145000, OriginalPrice = 168000, DiscountPercent = 14, Stock = 28, SoldCount = 567, ShortDescription = "Tiểu thuyết tình cảm nổi tiếng của Murakami",          Category = foreign, ImageUrl = "products/placeholder.svg",          IsFeatured = true,  IsAvailable = true, AverageRating = 4.7m, ReviewCount = 789 },
                new Product { ProductName = "Totto-chan - Cô Bé Bên Cửa Sổ",  Author = "Tetsuko Kuroyanagi",         ISBN = "9786042345001", Publisher = "NXB Hội Nhà Văn",       PublicationYear = 2018, Language = "Tiếng Việt", PageCount = 276, CoverType = "Bìa mềm", Weight = 270, Price = 78000,  OriginalPrice = 92000,  DiscountPercent = 15, Stock = 55, SoldCount = 678, ShortDescription = "Hồi ức tuổi thơ Nhật Bản",                              Category = foreign, ImageUrl = "products/placeholder.svg",          IsAvailable = true, AverageRating = 4.8m, ReviewCount = 789 },

                // -- Văn học Việt Nam --
                new Product { ProductName = "Mắt Biếc",                       Author = "Nguyễn Nhật Ánh",           ISBN = "9786041157298", Publisher = "NXB Trẻ",               PublicationYear = 2021, Language = "Tiếng Việt", PageCount = 308, CoverType = "Bìa mềm", Weight = 320, Price = 110000, OriginalPrice = null,   DiscountPercent = null, Stock = 45, SoldCount = 678, ShortDescription = "Chuyện tình đầu dang dở đẹp buồn",                      Category = vn,      ImageUrl = "products/mat-biec.svg",             IsNewArrival = true, IsBestSeller = true, IsAvailable = true, AverageRating = 4.7m, ReviewCount = 890 },
                new Product { ProductName = "Tôi Thấy Hoa Vàng Trên Cỏ Xanh",Author = "Nguyễn Nhật Ánh",           ISBN = "9786041157274", Publisher = "NXB Trẻ",               PublicationYear = 2020, Language = "Tiếng Việt", PageCount = 368, CoverType = "Bìa mềm", Weight = 350, Price = 125000, OriginalPrice = null,   DiscountPercent = null, Stock = 40, SoldCount = 543, ShortDescription = "Câu chuyện tuổi thơ ở vùng quê",                        Category = vn,      ImageUrl = "products/hoa-vang-co-xanh.svg",     IsBestSeller = true,  IsAvailable = true, AverageRating = 4.8m, ReviewCount = 1120 },
                new Product { ProductName = "Cho Tôi Xin Một Vé Đi Tuổi Thơ",Author = "Nguyễn Nhật Ánh",           ISBN = "9786041157300", Publisher = "NXB Trẻ",               PublicationYear = 2019, Language = "Tiếng Việt", PageCount = 288, CoverType = "Bìa mềm", Weight = 290, Price = 85000,  OriginalPrice = 95000,  DiscountPercent = 11, Stock = 52, SoldCount = 432, ShortDescription = "Hồi ức tuổi thơ trong sáng",                           Category = vn,      ImageUrl = "products/placeholder.svg",          IsAvailable = true, AverageRating = 4.6m, ReviewCount = 567 },
                new Product { ProductName = "Cô Gái Đến Từ Hôm Qua",         Author = "Nguyễn Nhật Ánh",           ISBN = "9786041234999", Publisher = "NXB Trẻ",               PublicationYear = 2021, Language = "Tiếng Việt", PageCount = 336, CoverType = "Bìa mềm", Weight = 340, Price = 105000, OriginalPrice = 118000, DiscountPercent = 11, Stock = 38, SoldCount = 289, ShortDescription = "Câu chuyện tình cảm lãng mạn",                          Category = vn,      ImageUrl = "products/placeholder.svg",          IsNewArrival = true, IsAvailable = true, AverageRating = 4.5m, ReviewCount = 456 },

                // -- Kỹ năng sống --
                new Product { ProductName = "Đắc Nhân Tâm",                   Author = "Dale Carnegie",              ISBN = "9786042013458", Publisher = "NXB Tổng Hợp TPHCM",   PublicationYear = 2021, Language = "Tiếng Việt", PageCount = 320, CoverType = "Bìa mềm", Weight = 340, Price = 135000, OriginalPrice = 180000, DiscountPercent = 25, Stock = 60, SoldCount = 1234, ShortDescription = "Nghệ thuật giao tiếp và ứng xử kinh điển",             Category = skill,   ImageUrl = "products/dac-nhan-tam.svg",         IsFeatured = true,  IsBestSeller = true,  IsAvailable = true, AverageRating = 4.9m, ReviewCount = 3450 },
                new Product { ProductName = "Sapiens: Lược Sử Loài Người",    Author = "Yuval Noah Harari",          ISBN = "9786042265772", Publisher = "NXB Thế Giới",          PublicationYear = 2020, Language = "Tiếng Việt", PageCount = 544, CoverType = "Bìa cứng", Weight = 700, Price = 229000, OriginalPrice = 280000, DiscountPercent = 18, Stock = 25, SoldCount = 890,  ShortDescription = "Lịch sử loài người từ thời tiền sử đến hiện đại",     Category = skill,   ImageUrl = "products/sapiens.svg",              IsFeatured = true,  IsNewArrival = true,  IsAvailable = true, AverageRating = 4.7m, ReviewCount = 675 },
                new Product { ProductName = "Tư Duy Nhanh Và Chậm",           Author = "Daniel Kahneman",            ISBN = "9786042016879", Publisher = "NXB Thế Giới",          PublicationYear = 2021, Language = "Tiếng Việt", PageCount = 575, CoverType = "Bìa mềm", Weight = 650, Price = 200000, OriginalPrice = 250000, DiscountPercent = 20, Stock = 30, SoldCount = 456,  ShortDescription = "Hai hệ thống tư duy trong bộ não",                     Category = skill,   ImageUrl = "products/tu-duy-nhanh-cham.svg",    IsAvailable = true, AverageRating = 4.6m, ReviewCount = 342 },
                new Product { ProductName = "Atomic Habits",                   Author = "James Clear",                ISBN = "9786042278895", Publisher = "NXB Thế Giới",          PublicationYear = 2021, Language = "Tiếng Việt", PageCount = 375, CoverType = "Bìa mềm", Weight = 380, Price = 152000, OriginalPrice = 190000, DiscountPercent = 20, Stock = 55, SoldCount = 789,  ShortDescription = "Cách tạo thói quen tốt và phá bỏ thói quen xấu",     Category = skill,   ImageUrl = "products/atomic-habits.jpg",        IsFeatured = true,  IsBestSeller = true,  IsAvailable = true, AverageRating = 4.8m, ReviewCount = 987 },
                new Product { ProductName = "Tôi Tài Giỏi Bạn Cũng Thế",     Author = "Adam Khoo",                  ISBN = "9786041234890", Publisher = "NXB Trẻ",               PublicationYear = 2019, Language = "Tiếng Việt", PageCount = 296, CoverType = "Bìa mềm", Weight = 310, Price = 98000,  OriginalPrice = 125000, DiscountPercent = 22, Stock = 48, SoldCount = 345,  ShortDescription = "Bí quyết học tập hiệu quả",                           Category = skill,   ImageUrl = "products/placeholder.svg",          IsAvailable = true, AverageRating = 4.5m, ReviewCount = 234 },

                // -- Thiếu nhi --
                new Product { ProductName = "Dế Mèn Phiêu Lưu Ký",           Author = "Tô Hoài",                   ISBN = "9786041098762", Publisher = "NXB Kim Đồng",           PublicationYear = 2020, Language = "Tiếng Việt", PageCount = 200, CoverType = "Bìa mềm", Weight = 220, Price = 75000,  OriginalPrice = null,   DiscountPercent = null, Stock = 80, SoldCount = 456,  ShortDescription = "Văn học thiếu nhi kinh điển Việt Nam",                Category = child,   ImageUrl = "products/de-men.svg",               IsBestSeller = true, IsAvailable = true, AverageRating = 5.0m, ReviewCount = 890 },
                new Product { ProductName = "Hoàng Tử Bé",                    Author = "Antoine de Saint-Exupéry",  ISBN = "9786042234567", Publisher = "NXB Trẻ",               PublicationYear = 2021, Language = "Tiếng Việt", PageCount = 128, CoverType = "Bìa cứng", Weight = 250, Price = 96000,  OriginalPrice = 120000, DiscountPercent = 20, Stock = 60, SoldCount = 678,  ShortDescription = "Tác phẩm thiếu nhi kinh điển thế giới",               Category = child,   ImageUrl = "products/hoang-tu-be.svg",          IsFeatured = true,  IsBestSeller = true,  IsAvailable = true, AverageRating = 4.9m, ReviewCount = 1234 },
                new Product { ProductName = "Đảo Giấu Vàng",                  Author = "Robert Louis Stevenson",    ISBN = "9786041345678", Publisher = "NXB Kim Đồng",           PublicationYear = 2020, Language = "Tiếng Việt", PageCount = 312, CoverType = "Bìa mềm", Weight = 320, Price = 88000,  OriginalPrice = 98000,  DiscountPercent = 10, Stock = 45, SoldCount = 234,  ShortDescription = "Phiêu lưu mạo hiểm tìm kho báu",                      Category = child,   ImageUrl = "products/placeholder.svg",          IsAvailable = true, AverageRating = 4.6m, ReviewCount = 456 },

                // -- Kinh tế - Kinh doanh --
                new Product { ProductName = "Khởi Nghiệp Tinh Gọn",          Author = "Eric Ries",                  ISBN = "9786042214580", Publisher = "NXB Thế Giới",          PublicationYear = 2020, Language = "Tiếng Việt", PageCount = 320, CoverType = "Bìa mềm", Weight = 340, Price = 165000, OriginalPrice = null,   DiscountPercent = null, Stock = 40, SoldCount = 234,  ShortDescription = "Phương pháp khởi nghiệp hiệu quả hiện đại",           Category = biz,     ImageUrl = "products/khoi-nghiep-tinh-gon.jpg", IsAvailable = true, AverageRating = 4.5m, ReviewCount = 234 },
                new Product { ProductName = "7 Thói Quen Hiệu Quả",           Author = "Stephen R. Covey",           ISBN = "9786042198764", Publisher = "NXB Tổng Hợp TPHCM",   PublicationYear = 2020, Language = "Tiếng Việt", PageCount = 448, CoverType = "Bìa mềm", Weight = 480, Price = 160000, OriginalPrice = 200000, DiscountPercent = 20, Stock = 45, SoldCount = 567,  ShortDescription = "Bảy nguyên tắc thành công bền vững",                  Category = biz,     ImageUrl = "products/7-thoi-quen.svg",          IsBestSeller = true, IsAvailable = true, AverageRating = 4.7m, ReviewCount = 678 },
                new Product { ProductName = "Làm Giàu Từ Chứng Khoán",        Author = "Võ Hải Triều",              ISBN = "9786042301235", Publisher = "NXB Lao Động",           PublicationYear = 2021, Language = "Tiếng Việt", PageCount = 280, CoverType = "Bìa mềm", Weight = 300, Price = 145000, OriginalPrice = null,   DiscountPercent = null, Stock = 35, SoldCount = 345,  ShortDescription = "Đầu tư chứng khoán cho người Việt",                   Category = biz,     ImageUrl = "products/lam-giau-chung-khoan.svg", IsNewArrival = true, IsAvailable = true, AverageRating = 4.3m, ReviewCount = 234 },
                new Product { ProductName = "Zero to One",                     Author = "Peter Thiel",                ISBN = "9786043012345", Publisher = "NXB Thế Giới",          PublicationYear = 2020, Language = "Tiếng Việt", PageCount = 224, CoverType = "Bìa mềm", Weight = 250, Price = 129000, OriginalPrice = 148000, DiscountPercent = 13, Stock = 42, SoldCount = 378,  ShortDescription = "Tạo ra điều chưa từng có",                            Category = biz,     ImageUrl = "products/placeholder.svg",          IsNewArrival = true, IsAvailable = true, AverageRating = 4.7m, ReviewCount = 456 },
                new Product { ProductName = "The 4-Hour Work Week",            Author = "Tim Ferriss",                ISBN = "9786043123456", Publisher = "NXB Thế Giới",          PublicationYear = 2019, Language = "Tiếng Việt", PageCount = 416, CoverType = "Bìa mềm", Weight = 440, Price = 159000, OriginalPrice = 185000, DiscountPercent = 14, Stock = 31, SoldCount = 267,  ShortDescription = "Làm việc hiệu quả, sống tự do",                       Category = biz,     ImageUrl = "products/placeholder.svg",          IsAvailable = true, AverageRating = 4.5m, ReviewCount = 389 },
                new Product { ProductName = "Nhà Kinh Tế Học Siêu Đẳng",      Author = "Steven D. Levitt",           ISBN = "9786042567890", Publisher = "NXB Thế Giới",          PublicationYear = 2020, Language = "Tiếng Việt", PageCount = 384, CoverType = "Bìa mềm", Weight = 400, Price = 149000, OriginalPrice = 175000, DiscountPercent = 15, Stock = 32, SoldCount = 345,  ShortDescription = "Kinh tế học từ góc nhìn bất ngờ",                     Category = biz,     ImageUrl = "products/placeholder.svg",          IsAvailable = true, AverageRating = 4.5m, ReviewCount = 456 },

                // -- Công nghệ - Lập trình --
                new Product { ProductName = "Clean Code",                      Author = "Robert C. Martin",           ISBN = "9786042345678", Publisher = "NXB Thế Giới",          PublicationYear = 2020, Language = "Tiếng Việt", PageCount = 464, CoverType = "Bìa mềm", Weight = 520, Price = 245000, OriginalPrice = null,   DiscountPercent = null, Stock = 30, SoldCount = 234,  ShortDescription = "Nghệ thuật viết code sạch",                           Category = tech,    ImageUrl = "products/clean-code.jpg",           IsFeatured = true,  IsAvailable = true, AverageRating = 4.8m, ReviewCount = 456 },
                new Product { ProductName = "Design Patterns",                 Author = "Gang of Four",               ISBN = "9786042456781", Publisher = "NXB Thế Giới",          PublicationYear = 2021, Language = "Tiếng Việt", PageCount = 512, CoverType = "Bìa mềm", Weight = 580, Price = 265000, OriginalPrice = null,   DiscountPercent = null, Stock = 25, SoldCount = 156,  ShortDescription = "23 mẫu thiết kế phần mềm kinh điển",                 Category = tech,    ImageUrl = "products/design-patterns.jpg",      IsAvailable = true, AverageRating = 4.6m, ReviewCount = 234 },
                new Product { ProductName = "ASP.NET Core từ A-Z",             Author = "Nguyễn Văn Dev",            ISBN = "9786042567891", Publisher = "NXB Thông Tin",          PublicationYear = 2021, Language = "Tiếng Việt", PageCount = 680, CoverType = "Bìa mềm", Weight = 750, Price = 299000, OriginalPrice = null,   DiscountPercent = null, Stock = 20, SoldCount = 123,  ShortDescription = "Hướng dẫn toàn diện ASP.NET Core 8",                 Category = tech,    ImageUrl = "products/aspnet-core.jpg",          IsNewArrival = true, IsAvailable = true, AverageRating = 4.7m, ReviewCount = 89 },
                new Product { ProductName = "JavaScript: The Good Parts",      Author = "Douglas Crockford",          ISBN = "9786042678901", Publisher = "NXB Thế Giới",          PublicationYear = 2019, Language = "Tiếng Việt", PageCount = 176, CoverType = "Bìa mềm", Weight = 210, Price = 118000, OriginalPrice = 138000, DiscountPercent = 14, Stock = 35, SoldCount = 267,  ShortDescription = "Những điều tốt nhất về JavaScript",                  Category = tech,    ImageUrl = "products/placeholder.svg",          IsAvailable = true, AverageRating = 4.5m, ReviewCount = 345 },
                new Product { ProductName = "Python Crash Course",             Author = "Eric Matthes",               ISBN = "9786042789012", Publisher = "NXB Thế Giới",          PublicationYear = 2021, Language = "Tiếng Việt", PageCount = 544, CoverType = "Bìa mềm", Weight = 600, Price = 199000, OriginalPrice = 228000, DiscountPercent = 13, Stock = 28, SoldCount = 189,  ShortDescription = "Học Python từ đầu với dự án thực tế",                Category = tech,    ImageUrl = "products/placeholder.svg",          IsNewArrival = true, IsAvailable = true, AverageRating = 4.7m, ReviewCount = 456 },

                // -- Tâm lý - Tình cảm --
                new Product { ProductName = "Thinking Fast and Slow",          Author = "Daniel Kahneman",            ISBN = "9786042890123", Publisher = "NXB Thế Giới",          PublicationYear = 2020, Language = "Tiếng Việt", PageCount = 512, CoverType = "Bìa cứng", Weight = 650, Price = 225000, OriginalPrice = 265000, DiscountPercent = 15, Stock = 22, SoldCount = 456,  ShortDescription = "Cách bộ não đưa ra quyết định",                       Category = psych,   ImageUrl = "products/placeholder.svg",          IsFeatured = true,  IsAvailable = true, AverageRating = 4.8m, ReviewCount = 678 },

                // -- Lịch sử - Chính trị --
                new Product { ProductName = "Sapiens: Lịch Sử Tóm Lược",      Author = "Yuval Noah Harari",          ISBN = "9786043234567", Publisher = "NXB Thế Giới",          PublicationYear = 2018, Language = "Tiếng Việt", PageCount = 512, CoverType = "Bìa cứng", Weight = 680, Price = 249000, OriginalPrice = 298000, DiscountPercent = 16, Stock = 18, SoldCount = 1234, ShortDescription = "Từ thời tiền sử đến hiện đại",                        Category = history, ImageUrl = "products/placeholder.svg",          IsFeatured = true,  IsBestSeller = true,  IsAvailable = true, AverageRating = 4.9m, ReviewCount = 2345 },
                new Product { ProductName = "21 Bài Học Cho Thế Kỷ 21",        Author = "Yuval Noah Harari",          ISBN = "9786043345678", Publisher = "NXB Thế Giới",          PublicationYear = 2021, Language = "Tiếng Việt", PageCount = 448, CoverType = "Bìa cứng", Weight = 580, Price = 235000, OriginalPrice = 275000, DiscountPercent = 15, Stock = 24, SoldCount = 789,  ShortDescription = "Thách thức và cơ hội của thế kỷ 21",                 Category = history, ImageUrl = "products/placeholder.svg",          IsFeatured = true,  IsNewArrival = true,  IsAvailable = true, AverageRating = 4.8m, ReviewCount = 1234 },
            };
            context.Products.AddRange(products);
            context.SaveChanges();

            // ===== DEMO ORDERS (50 orders - last 90 days) =====
            var rnd       = new Random(42); // fixed seed for reproducible data
            var customers = users.Where(u => u.Role == "Customer").ToList();
            var allProds  = products;
            var statuses  = new[] { "Pending", "Processing", "Shipped", "Completed", "Cancelled" };
            var payments  = new[] { "COD", "BankTransfer", "VNPay" };
            var cities = wardByCity.Keys.ToArray();

            for (int i = 0; i < 50; i++)
            {
                var customer  = customers[rnd.Next(customers.Count)];
                var orderDate = DateTime.Now.AddDays(-rnd.Next(0, 90)).AddHours(-rnd.Next(0, 24));
                var daysAgo   = (DateTime.Now - orderDate).TotalDays;
                
                var status = daysAgo > 14 ? statuses[rnd.Next(2, 5)]  // older → more likely completed/cancelled
                           : daysAgo > 3  ? statuses[rnd.Next(0, 4)]
                           :                statuses[rnd.Next(0, 3)];

                var paymentStatus = status == "Completed" || status == "Shipped" ? "Paid"
                                  : status == "Cancelled" ? "Failed"
                                  : "Pending";

                var payMethod = payments[rnd.Next(payments.Length)];
                var city      = cities[rnd.Next(cities.Length)];
                var wards     = wardByCity[city];

                var order = new Order
                {
                    UserId        = customer.UserId,
                    OrderDate     = orderDate,
                    Status        = status,
                    RecipientName = customer.FullName,
                    PhoneNumber   = customer.PhoneNumber,
                    Email         = customer.Email,
                    ShippingAddress = $"{rnd.Next(1, 200)} Đường Nguyễn Văn Cừ",
                    City          = city,
                    Ward          = wards[rnd.Next(wards.Length)],
                    Notes         = rnd.Next(2) == 0 ? "Giao giờ hành chính" : "",
                    PaymentMethod = payMethod,
                    PaymentStatus = paymentStatus,
                    PaidAt        = paymentStatus == "Paid" ? orderDate.AddHours(rnd.Next(1, 24)) : null,
                };

                // 1–4 products per order
                var itemCount   = rnd.Next(1, 5);
                var usedIndexes = new HashSet<int>();
                for (int j = 0; j < itemCount; j++)
                {
                    int idx;
                    do { idx = rnd.Next(allProds.Count); } while (!usedIndexes.Add(idx));
                    var prod = allProds[idx];
                    var qty  = rnd.Next(1, 4);
                    order.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = prod.ProductId,
                        Quantity  = qty,
                        UnitPrice = prod.Price,
                    });
                }

                order.TotalAmount = order.OrderDetails.Sum(d => d.Quantity * d.UnitPrice);
                context.Orders.Add(order);
            }

            context.SaveChanges();
        }
    }
}
