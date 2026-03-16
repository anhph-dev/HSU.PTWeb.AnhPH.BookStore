-- ============================================================
-- SQL Script to Seed BookStore Database
-- Run this directly in SSMS or SQL Server Management Studio
-- ============================================================

USE BookStoreDb;
GO

-- Clear existing data (if any)
DELETE FROM OrderDetails;
DELETE FROM Orders;
DELETE FROM Products;
DELETE FROM Categories;
DELETE FROM Users;

-- Reset identity seeds
DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('Categories', RESEED, 0);
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
DBCC CHECKIDENT ('OrderDetails', RESEED, 0);
GO

PRINT '🗑️ Cleared existing data';
GO

-- ============================================================
-- FIX: Increase ISBN column length to 13 characters
-- ============================================================
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Products') AND name = 'ISBN')
BEGIN
    ALTER TABLE Products ALTER COLUMN ISBN NVARCHAR(20) NULL;
    PRINT '✅ Fixed ISBN column length';
END
GO

-- ============================================================
-- INSERT USERS
-- ============================================================
SET IDENTITY_INSERT Users ON;

INSERT INTO Users (UserId, Email, FullName, Password, Role)
VALUES 
(1, 'admin@bookstore.com', N'Quản trị viên', 'admin123', 'Admin'),
(2, 'user1@gmail.com', N'Nguyễn Văn A', '123456', 'Customer'),
(3, 'user2@gmail.com', N'Trần Thị B', '123456', 'Customer');

SET IDENTITY_INSERT Users OFF;
GO

PRINT '✅ Inserted 3 users';
GO

-- ============================================================
-- INSERT CATEGORIES
-- ============================================================
SET IDENTITY_INSERT Categories ON;

INSERT INTO Categories (CategoryId, CategoryName, Description)
VALUES 
(1, N'Văn học Việt Nam', N'Tiểu thuyết, truyện ngắn của các tác giả Việt Nam'),
(2, N'Văn học nước ngoài', N'Tiểu thuyết dịch từ nước ngoài'),
(3, N'Kỹ năng sống', N'Sách phát triển bản thân, tư duy'),
(4, N'Kinh tế - Kinh doanh', N'Sách về kinh tế, khởi nghiệp, quản lý'),
(5, N'Thiếu nhi', N'Sách dành cho trẻ em'),
(6, N'Công nghệ - Lập trình', N'Sách về công nghệ thông tin và lập trình');

SET IDENTITY_INSERT Categories OFF;
GO

PRINT '✅ Inserted 6 categories';
GO

-- ============================================================
-- INSERT PRODUCTS (16 books)
-- ============================================================
SET IDENTITY_INSERT Products ON;

INSERT INTO Products (
    ProductId, ProductName, Author, ISBN, Publisher, PublicationYear, 
    Language, PageCount, CoverType, Dimensions, Weight,
    OriginalPrice, Price, DiscountPercent, Stock, SoldCount,
    ShortDescription, Description, CategoryId, ImageUrl,
    IsFeatured, IsNewArrival, IsBestSeller, IsAvailable, IsDiscontinued,
    AverageRating, ReviewCount, CreatedDate
)
VALUES 
-- Văn học nước ngoài
(1, N'Nhà Giả Kim', 'Paulo Coelho', '9786042148393', N'NXB Hội Nhà Văn', 2020,
 N'Tiếng Việt', 256, N'Bìa mềm', '20.5 x 14 x 1.5', 300,
 150000, 120000, 20, 50, 234,
 N'Tác phẩm kinh điển về hành trình tìm kiếm ước mơ',
 N'Nhà Giả Kim là một trong những cuốn sách bán chạy nhất mọi thời đại. Câu chuyện kể về chàng chăn cừu Santiago trong hành trình tìm kiếm kho báu và khám phá chính mình.',
 2, 'products/nha-gia-kim.jpg',
 1, 0, 1, 1, 0,
 4.8, 1250, GETDATE()),

(2, N'Cây Cam Ngọt Của Tôi', N'José Mauro de Vasconcelos', '9786042271667', N'NXB Hội Nhà Văn', 2019,
 N'Tiếng Việt', 244, N'Bìa mềm', '20 x 13 x 1.5', 280,
 140000, 108000, 23, 35, 456,
 N'Câu chuyện cảm động về tuổi thơ nghèo khó nhưng giàu yêu thương',
 N'Cây Cam Ngọt Của Tôi là một câu chuyện cảm động về cậu bé Zezé và cây cam ngọt duy nhất của cậu. Cuốn sách là hồi ức tuổi thơ đầy xúc động.',
 2, 'products/cay-cam-ngot.jpg',
 1, 0, 1, 1, 0,
 4.9, 2340, GETDATE()),

-- Văn học Việt Nam
(3, N'Mắt Biếc', N'Nguyễn Nhật Ánh', '9786041157298', N'NXB Trẻ', 2021,
 N'Tiếng Việt', 308, N'Bìa mềm', '20 x 13 x 2', 320,
 NULL, 110000, NULL, 45, 678,
 N'Chuyện tình đầu dang dở của Ngạn và Hà Lan',
 N'Mắt Biếc là một trong những tác phẩm nổi bật nhất của nhà văn Nguyễn Nhật Ánh. Câu chuyện tình đầu trong sáng nhưng đầy bi kịch.',
 1, 'products/mat-biec.jpg',
 0, 1, 1, 1, 0,
 4.7, 890, GETDATE()),

(4, N'Tôi Thấy Hoa Vàng Trên Cỏ Xanh', N'Nguyễn Nhật Ánh', '9786041157274', N'NXB Trẻ', 2020,
 N'Tiếng Việt', 368, N'Bìa mềm', '20 x 13 x 2.5', 350,
 NULL, 125000, NULL, 40, 543,
 N'Tuổi thơ dữ dội trong hồi ức',
 N'Tác phẩm về tuổi thơ ở vùng quê Việt Nam với những câu chuyện vừa buồn vừa vui về tình anh em, tình bạn, và những nổi đau tuổi nhỏ.',
 1, 'products/hoa-vang-co-xanh.jpg',
 0, 0, 1, 1, 0,
 4.8, 1120, GETDATE()),

-- Kỹ năng sống
(5, N'Đắc Nhân Tâm', 'Dale Carnegie', '9786042013458', N'NXB Tổng Hợp TPHCM', 2021,
 N'Tiếng Việt', 320, N'Bìa mềm', '20.5 x 14 x 2', 340,
 180000, 135000, 25, 60, 1234,
 N'Nghệ thuật giao tiếp và ứng xử với con người',
 N'Đắc Nhân Tâm là cuốn sách kinh điển về nghệ thuật giao tiếp và kỹ năng ứng xử, giúp bạn thành công trong cuộc sống và công việc.',
 3, 'products/dac-nhan-tam.jpg',
 1, 0, 1, 1, 0,
 4.9, 3450, GETDATE()),

(6, N'Sapiens: Lược Sử Loài Người', 'Yuval Noah Harari', '9786042265772', N'NXB Thế Giới', 2020,
 N'Tiếng Việt', 544, N'Bìa cứng', '23 x 15.5 x 3.5', 700,
 280000, 229000, 18, 25, 890,
 N'Câu chuyện về sự tiến hóa của loài người từ thời tiền sử đến hiện đại',
 N'Sapiens là cuốn sách nghiên cứu lịch sử loài người từ góc nhìn khoa học, triết học và xã hội học, giúp chúng ta hiểu rõ hơn về bản thân và tương lai.',
 3, 'products/sapiens.jpg',
 1, 1, 0, 1, 0,
 4.7, 675, GETDATE()),

(7, N'Tư Duy Nhanh Và Chậm', 'Daniel Kahneman', '9786042016879', N'NXB Thế Giới', 2021,
 N'Tiếng Việt', 575, N'Bìa mềm', '23 x 15.5 x 3', 650,
 250000, 200000, 20, 30, 456,
 N'Khám phá hai hệ thống tư duy trong bộ não con người',
 N'Cuốn sách của nhà tâm lý học đoạt giải Nobel, giải thích cách bộ não xử lý thông tin và đưa ra quyết định.',
 3, 'products/tu-duy-nhanh-cham.jpg',
 0, 0, 0, 1, 0,
 4.6, 342, GETDATE()),

(8, N'Atomic Habits - Thói Quen Nguyên Tử', 'James Clear', '9786042278895', N'NXB Thế Giới', 2021,
 N'Tiếng Việt', 375, N'Bìa mềm', '21 x 14 x 2', 380,
 190000, 152000, 20, 55, 789,
 N'Cách tạo thói quen tốt, bỏ thói quen xấu',
 N'Atomic Habits hướng dẫn cách xây dựng và duy trì thói quen tốt thông qua những thay đổi nhỏ hàng ngày.',
 3, 'products/atomic-habits.jpg',
 1, 0, 1, 1, 0,
 4.8, 987, GETDATE()),

-- Kinh doanh
(9, N'Khởi Nghiệp Tinh Gọn', 'Eric Ries', '9786042214580', N'NXB Thế Giới', 2020,
 N'Tiếng Việt', 320, N'Bìa mềm', '21 x 14 x 2', 340,
 NULL, 165000, NULL, 40, 234,
 N'Phương pháp khởi nghiệp hiệu quả cho thời đại số',
 N'The Lean Startup giới thiệu phương pháp khởi nghiệp linh hoạt, tiết kiệm chi phí và tối ưu hóa sản phẩm dựa trên phản hồi thị trường.',
 4, 'products/khoi-nghiep-tinh-gon.jpg',
 0, 0, 0, 1, 0,
 4.5, 234, GETDATE()),

(10, N'7 Thói Quen Của Người Thành Đạt', 'Stephen R. Covey', '9786042198764', N'NXB Tổng Hợp TPHCM', 2020,
 N'Tiếng Việt', 448, N'Bìa mềm', '21 x 14 x 3', 480,
 200000, 160000, 20, 45, 567,
 N'Bảy nguyên tắc cơ bản để đạt được thành công bền vững',
 N'Cuốn sách kinh điển về phát triển cá nhân và lãnh đạo, giúp bạn xây dựng nền tảng vững chắc cho sự nghiệp.',
 4, 'products/7-thoi-quen.jpg',
 0, 0, 1, 1, 0,
 4.7, 678, GETDATE()),

(11, N'Làm Giàu Từ Chứng Khoán', N'Võ Hải Triều', '9786042301235', N'NXB Lao Động', 2021,
 N'Tiếng Việt', 280, N'Bìa mềm', '21 x 14 x 1.5', 300,
 NULL, 145000, NULL, 35, 345,
 N'Hướng dẫn đầu tư chứng khoán cho người Việt',
 N'Cuốn sách cung cấp kiến thức cơ bản về thị trường chứng khoán Việt Nam, phương pháp phân tích và chiến lược đầu tư hiệu quả.',
 4, 'products/lam-giau-chung-khoan.jpg',
 0, 1, 0, 1, 0,
 4.3, 234, GETDATE()),

-- Thiếu nhi
(12, N'Dế Mèn Phiêu Lưu Ký', N'Tô Hoài', '9786041098762', N'NXB Kim Đồng', 2020,
 N'Tiếng Việt', 200, N'Bìa mềm', '19 x 13 x 1.5', 220,
 NULL, 75000, NULL, 80, 456,
 N'Tác phẩm kinh điển văn học thiếu nhi Việt Nam',
 N'Câu chuyện về chú dế mèn dũng cảm trong cuộc phiêu lưu đầy bất ngờ, bài học về tình bạn và lòng dũng cảm.',
 5, 'products/de-men.jpg',
 0, 0, 1, 1, 0,
 5.0, 890, GETDATE()),

(13, N'Hoàng Tử Bé', N'Antoine de Saint-Exupéry', '9786042234567', N'NXB Trẻ', 2021,
 N'Tiếng Việt', 128, N'Bìa cứng', '21 x 15 x 1', 250,
 120000, 96000, 20, 60, 678,
 N'Câu chuyện thiếu nhi mang ý nghĩa sâu sắc về tình yêu và trưởng thành',
 N'Hoàng Tử Bé là một tác phẩm kinh điển của văn học thế giới, phù hợp cho mọi lứa tuổi với những bài học về tình yêu và cuộc sống.',
 5, 'products/hoang-tu-be.jpg',
 1, 0, 1, 1, 0,
 4.9, 1234, GETDATE()),

-- Lập trình
(14, N'Clean Code - Mã Sạch', 'Robert C. Martin', '9786042345678', N'NXB Thế Giới', 2020,
 N'Tiếng Việt', 464, N'Bìa mềm', '23 x 15.5 x 3', 520,
 NULL, 245000, NULL, 30, 234,
 N'Nghệ thuật viết code sạch, dễ đọc, dễ bảo trì',
 N'Clean Code là cuốn sách kinh điển về kỹ thuật lập trình, hướng dẫn cách viết code chất lượng cao, dễ đọc và bảo trì.',
 6, 'products/clean-code.jpg',
 1, 0, 0, 1, 0,
 4.8, 456, GETDATE()),

(15, N'Design Patterns - Mẫu Thiết Kế', 'Gang of Four', '9786042456789', N'NXB Thế Giới', 2021,
 N'Tiếng Việt', 512, N'Bìa mềm', '23 x 15.5 x 3.5', 580,
 NULL, 265000, NULL, 25, 156,
 N'23 mẫu thiết kế phần mềm kinh điển',
 N'Cuốn sách giới thiệu 23 mẫu thiết kế (design patterns) cơ bản trong lập trình hướng đối tượng, giúp giải quyết các vấn đề thường gặp.',
 6, 'products/design-patterns.jpg',
 0, 0, 0, 1, 0,
 4.6, 234, GETDATE()),

(16, N'ASP.NET Core từ A đến Z', N'Nguyễn Văn Dev', '9786042567890', N'NXB Thông Tin và Truyền Thông', 2021,
 N'Tiếng Việt', 680, N'Bìa mềm', '24 x 16 x 4', 750,
 NULL, 299000, NULL, 20, 123,
 N'Hướng dẫn toàn diện về ASP.NET Core Web Development',
 N'Cuốn sách hướng dẫn chi tiết về ASP.NET Core, từ cơ bản đến nâng cao, bao gồm MVC, Razor Pages, Web API, Entity Framework Core.',
 6, 'products/aspnet-core.jpg',
 0, 1, 0, 1, 0,
 4.7, 89, GETDATE());

SET IDENTITY_INSERT Products OFF;
GO

PRINT '✅ Inserted 16 products';
GO

-- ============================================================
-- VERIFICATION
-- ============================================================
PRINT '';
PRINT '============================================================';
PRINT '📊 Database Seeding Completed!';
PRINT '============================================================';
PRINT '';

SELECT 
    'Users' AS TableName, 
    COUNT(*) AS RecordCount 
FROM Users
UNION ALL
SELECT 
    'Categories' AS TableName, 
    COUNT(*) AS RecordCount 
FROM Categories
UNION ALL
SELECT 
    'Products' AS TableName, 
    COUNT(*) AS RecordCount 
FROM Products;

PRINT '';
PRINT '📚 Sample Products:';
SELECT TOP 5 
    ProductId,
    ProductName,
    Author,
    Price,
    Stock
FROM Products
ORDER BY ProductId;

PRINT '';
PRINT '✅ All done! Database is ready to use.';
PRINT '';
GO
