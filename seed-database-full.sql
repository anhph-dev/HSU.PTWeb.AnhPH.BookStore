-- ============================================================
-- COMPREHENSIVE SEED DATA: 30 Products + Demo Orders for Reports
-- ============================================================

USE BookStoreDb;
GO

-- Clear existing data
DELETE FROM OrderDetails;
DELETE FROM Orders;
DELETE FROM Products;
DELETE FROM Categories;
DELETE FROM Users;

-- Reset identity
DBCC CHECKIDENT ('Users', RESEED, 0);
DBCC CHECKIDENT ('Categories', RESEED, 0);
DBCC CHECKIDENT ('Products', RESEED, 0);
DBCC CHECKIDENT ('Orders', RESEED, 0);
DBCC CHECKIDENT ('OrderDetails', RESEED, 0);
GO

PRINT '✅ Cleared existing data';
GO

-- Fix ISBN column length
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Products') AND name = 'ISBN')
BEGIN
    ALTER TABLE Products ALTER COLUMN ISBN NVARCHAR(20) NULL;
END
GO

-- ============================================================
-- INSERT USERS (with hashed passwords using BCrypt)
-- ============================================================
SET IDENTITY_INSERT Users ON;

INSERT INTO Users (UserId, Email, FullName, Password, PasswordHash, Role, PhoneNumber, CreatedDate)
VALUES 
-- Admin account: admin@bookstore.com / Admin@123
(1, 'admin@bookstore.com', N'Quản trị viên', 'admin123', 
 '$2a$11$kMWXLN.Kz8jCxOqO5S9gS.hK8pQrZ8VYF9FQM4LG0vHhx6K8KXQwG', 
 'Admin', '0901234567', GETDATE()),

-- Customer accounts
(2, 'customer1@gmail.com', N'Nguyễn Văn A', '123456',
 '$2a$11$V/7wGPVP8XPSq4QYYzgJ5eJ9L2mX6EhZ8KQQYDx7LmXP8YZG4GE7G',
 'Customer', '0912345678', DATEADD(day, -30, GETDATE())),

(3, 'customer2@gmail.com', N'Trần Thị B', '123456',
 '$2a$11$V/7wGPVP8XPSq4QYYzgJ5eJ9L2mX6EhZ8KQQYDx7LmXP8YZG4GE7G',
 'Customer', '0923456789', DATEADD(day, -25, GETDATE())),

(4, 'customer3@gmail.com', N'Lê Văn C', '123456',
 '$2a$11$V/7wGPVP8XPSq4QYYzgJ5eJ9L2mX6EhZ8KQQYDx7LmXP8YZG4GE7G',
 'Customer', '0934567890', DATEADD(day, -20, GETDATE())),

(5, 'customer4@gmail.com', N'Phạm Thị D', '123456',
 '$2a$11$V/7wGPVP8XPSq4QYYzgJ5eJ9L2mX6EhZ8KQQYDx7LmXP8YZG4GE7G',
 'Customer', '0945678901', DATEADD(day, -15, GETDATE()));

SET IDENTITY_INSERT Users OFF;
GO

PRINT '✅ Inserted 5 users (1 admin + 4 customers)';
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
(6, N'Công nghệ - Lập trình', N'Sách về công nghệ thông tin và lập trình'),
(7, N'Tâm lý - Tình cảm', N'Sách về tâm lý học và tình cảm'),
(8, N'Lịch sử - Chính trị', N'Sách về lịch sử và chính trị');

SET IDENTITY_INSERT Categories OFF;
GO

PRINT '✅ Inserted 8 categories';
GO

-- ============================================================
-- INSERT 30+ PRODUCTS
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
-- Văn học nước ngoài (10 sách)
(1, N'Nhà Giả Kim', 'Paulo Coelho', '9786042148393', N'NXB Hội Nhà Văn', 2020,
 N'Tiếng Việt', 256, N'Bìa mềm', '20.5 x 14 x 1.5', 300,
 150000, 120000, 20, 50, 234,
 N'Tác phẩm kinh điển về hành trình tìm kiếm ước mơ',
 N'Nhà Giả Kim là một trong những cuốn sách bán chạy nhất mọi thời đại.',
 2, 'products/nha-gia-kim.jpg', 1, 0, 1, 1, 0, 4.8, 1250, GETDATE()),

(2, N'Cây Cam Ngọt Của Tôi', N'José Mauro de Vasconcelos', '9786042271667', N'NXB Hội Nhà Văn', 2019,
 N'Tiếng Việt', 244, N'Bìa mềm', '20 x 13 x 1.5', 280,
 140000, 108000, 23, 35, 456,
 N'Câu chuyện cảm động về tuổi thơ nghèo khó',
 N'Cây Cam Ngọt Của Tôi là một câu chuyện cảm động về cậu bé Zezé.',
 2, 'products/cay-cam-ngot.jpg', 1, 0, 1, 1, 0, 4.9, 2340, GETDATE()),

(3, N'Harry Potter và Hòn Đá Phù Thủy', 'J.K. Rowling', '9786041234567', N'NXB Trẻ', 2021,
 N'Tiếng Việt', 352, N'Bìa mềm', '21 x 14 x 2', 380,
 165000, 145000, 12, 60, 890,
 N'Tập 1 của bộ truyện Harry Potter huyền thoại',
 N'Câu chuyện về cậu bé phù thủy Harry Potter.',
 2, 'products/placeholder.svg', 1, 1, 1, 1, 0, 4.9, 3456, GETDATE()),

(4, N'Đắc Nhân Tâm', 'Dale Carnegie', '9786042013458', N'NXB Tổng Hợp TPHCM', 2021,
 N'Tiếng Việt', 320, N'Bìa mềm', '20.5 x 14 x 2', 340,
 180000, 135000, 25, 60, 1234,
 N'Nghệ thuật giao tiếp và ứng xử',
 N'Cuốn sách kinh điển về nghệ thuật giao tiếp.',
 3, 'products/dac-nhan-tam.svg', 1, 0, 1, 1, 0, 4.9, 3450, GETDATE()),

(5, N'Sapiens: Lược Sử Loài Người', 'Yuval Noah Harari', '9786042265772', N'NXB Thế Giới', 2020,
 N'Tiếng Việt', 544, N'Bìa cứng', '23 x 15.5 x 3.5', 700,
 280000, 229000, 18, 25, 890,
 N'Lịch sử loài người từ thời tiền sử đến hiện đại',
 N'Sapiens là cuốn sách nghiên cứu lịch sử loài người.',
 3, 'products/sapiens.svg', 1, 1, 0, 1, 0, 4.7, 675, GETDATE()),

(6, N'Tư Duy Nhanh Và Chậm', 'Daniel Kahneman', '9786042016879', N'NXB Thế Giới', 2021,
 N'Tiếng Việt', 575, N'Bìa mềm', '23 x 15.5 x 3', 650,
 250000, 200000, 20, 30, 456,
 N'Hai hệ thống tư duy trong bộ não',
 N'Cuốn sách của nhà tâm lý học đoạt giải Nobel.',
 3, 'products/tu-duy-nhanh-cham.svg', 0, 0, 0, 1, 0, 4.6, 342, GETDATE()),

(7, N'Atomic Habits', 'James Clear', '9786042278895', N'NXB Thế Giới', 2021,
 N'Tiếng Việt', 375, N'Bìa mềm', '21 x 14 x 2', 380,
 190000, 152000, 20, 55, 789,
 N'Cách tạo thói quen tốt',
 N'Hướng dẫn xây dựng thói quen tốt.',
 3, 'products/atomic-habits.jpg', 1, 0, 1, 1, 0, 4.8, 987, GETDATE()),

(8, N'Khởi Nghiệp Tinh Gọn', 'Eric Ries', '9786042214580', N'NXB Thế Giới', 2020,
 N'Tiếng Việt', 320, N'Bìa mềm', '21 x 14 x 2', 340,
 NULL, 165000, NULL, 40, 234,
 N'Phương pháp khởi nghiệp hiệu quả',
 N'The Lean Startup giới thiệu phương pháp khởi nghiệp linh hoạt.',
 4, 'products/khoi-nghiep-tinh-gon.jpg', 0, 0, 0, 1, 0, 4.5, 234, GETDATE()),

(9, N'7 Thói Quen Hiệu Quả', 'Stephen R. Covey', '9786042198764', N'NXB Tổng Hợp TPHCM', 2020,
 N'Tiếng Việt', 448, N'Bìa mềm', '21 x 14 x 3', 480,
 200000, 160000, 20, 45, 567,
 N'Bảy nguyên tắc thành công bền vững',
 N'Cuốn sách kinh điển về phát triển cá nhân.',
 4, 'products/7-thoi-quen.svg', 0, 0, 1, 1, 0, 4.7, 678, GETDATE()),

(10, N'Tôi Tài Giỏi Bạn Cũng Thế', 'Adam Khoo', '9786041234890', N'NXB Trẻ', 2019,
 N'Tiếng Việt', 296, N'Bìa mềm', '20 x 13 x 1.8', 310,
 125000, 98000, 22, 48, 345,
 N'Bí quyết học tập hiệu quả',
 N'Chia sẻ bí quyết học tập và phát triển bản thân.',
 3, 'products/placeholder.svg', 0, 0, 0, 1, 0, 4.5, 234, GETDATE()),

-- Văn học Việt Nam (5 sách)
(11, N'Mắt Biếc', N'Nguyễn Nhật Ánh', '9786041157298', N'NXB Trẻ', 2021,
 N'Tiếng Việt', 308, N'Bìa mềm', '20 x 13 x 2', 320,
 NULL, 110000, NULL, 45, 678,
 N'Chuyện tình đầu dang dở',
 N'Tác phẩm nổi bật nhất của Nguyễn Nhật Ánh.',
 1, 'products/mat-biec.svg', 0, 1, 1, 1, 0, 4.7, 890, GETDATE()),

(12, N'Tôi Thấy Hoa Vàng Trên Cỏ Xanh', N'Nguyễn Nhật Ánh', '9786041157274', N'NXB Trẻ', 2020,
 N'Tiếng Việt', 368, N'Bìa mềm', '20 x 13 x 2.5', 350,
 NULL, 125000, NULL, 40, 543,
 N'Tuổi thơ dữ dội',
 N'Câu chuyện về tuổi thơ ở vùng quê.',
 1, 'products/hoa-vang-co-xanh.svg', 0, 0, 1, 1, 0, 4.8, 1120, GETDATE()),

(13, N'Cho Tôi Xin Một Vé Đi Tuổi Thơ', N'Nguyễn Nhật Ánh', '9786041157300', N'NXB Trẻ', 2019,
 N'Tiếng Việt', 288, N'Bìa mềm', '20 x 13 x 1.8', 290,
 95000, 85000, 11, 52, 432,
 N'Hồi ức tuổi thơ đẹp đẽ',
 N'Những kỷ niệm tuổi thơ trong sáng.',
 1, 'products/placeholder.svg', 0, 0, 0, 1, 0, 4.6, 567, GETDATE()),

(14, N'Cô Gái Đến Từ Hôm Qua', N'Nguyễn Nhật Ánh', '9786041234999', N'NXB Trẻ', 2021,
 N'Tiếng Việt', 336, N'Bìa mềm', '20 x 13 x 2.1', 340,
 118000, 105000, 11, 38, 289,
 N'Câu chuyện tình cảm lãng mạn',
 N'Chuyện tình lãng mạn và cảm động.',
 1, 'products/placeholder.svg', 0, 1, 0, 1, 0, 4.5, 456, GETDATE()),

(15, N'Totto-chan - Cô Bé Bên Cửa Sổ', 'Tetsuko Kuroyanagi', '9786042345001', N'NXB Hội Nhà Văn', 2018,
 N'Tiếng Việt', 276, N'Bìa mềm', '19 x 13 x 1.6', 270,
 92000, 78000, 15, 55, 678,
 N'Hồi ức tuổi thơ Nhật Bản',
 N'Câu chuyện về cô bé Totto-chan và ngôi trường đặc biệt.',
 5, 'products/placeholder.svg', 0, 0, 0, 1, 0, 4.8, 789, GETDATE()),

-- Thiếu nhi (5 sách)
(16, N'Dế Mèn Phiêu Lưu Ký', N'Tô Hoài', '9786041098762', N'NXB Kim Đồng', 2020,
 N'Tiếng Việt', 200, N'Bìa mềm', '19 x 13 x 1.5', 220,
 NULL, 75000, NULL, 80, 456,
 N'Văn học thiếu nhi kinh điển Việt Nam',
 N'Chú dế mèn dũng cảm.',
 5, 'products/de-men.svg', 0, 0, 1, 1, 0, 5.0, 890, GETDATE()),

(17, N'Hoàng Tử Bé', N'Antoine de Saint-Exupéry', '9786042234567', N'NXB Trẻ', 2021,
 N'Tiếng Việt', 128, N'Bìa cứng', '21 x 15 x 1', 250,
 120000, 96000, 20, 60, 678,
 N'Tác phẩm kinh điển thế giới',
 N'Câu chuyện thiếu nhi ý nghĩa sâu sắc.',
 5, 'products/hoang-tu-be.svg', 1, 0, 1, 1, 0, 4.9, 1234, GETDATE()),

(18, N'Đảo Giấu Vàng', 'Robert Louis Stevenson', '9786041345678', N'NXB Kim Đồng', 2020,
 N'Tiếng Việt', 312, N'Bìa mềm', '20 x 13 x 1.9', 320,
 98000, 88000, 10, 45, 234,
 N'Phiêu lưu mạo hiểm trên biển',
 N'Chuyến phiêu lưu tìm kho báu huyền thoại.',
 5, 'products/placeholder.svg', 0, 0, 0, 1, 0, 4.6, 456, GETDATE()),

(19, N'Rừng Nauy', 'Haruki Murakami', '9786042456789', N'NXB Hội Nhà Văn', 2019,
 N'Tiếng Việt', 448, N'Bìa mềm', '21 x 14 x 2.5', 470,
 168000, 145000, 14, 28, 567,
 N'Tiểu thuyết tình cảm nổi tiếng',
 N'Câu chuyện tình yêu và tuổi trẻ.',
 7, 'products/placeholder.svg', 1, 0, 0, 1, 0, 4.7, 789, GETDATE()),

(20, N'Nhà Kinh Tế Học Siêu Đẳng', 'Steven D. Levitt', '9786042567890', N'NXB Thế Giới', 2020,
 N'Tiếng Việt', 384, N'Bìa mềm', '21 x 14 x 2.3', 400,
 175000, 149000, 15, 32, 345,
 N'Kinh tế học từ góc nhìn mới',
 N'Phân tích các hiện tượng xã hội qua kinh tế học.',
 4, 'products/placeholder.svg', 0, 0, 0, 1, 0, 4.5, 456, GETDATE()),

-- Lập trình (5 sách)
(21, N'Clean Code', 'Robert C. Martin', '9786042345678', N'NXB Thế Giới', 2020,
 N'Tiếng Việt', 464, N'Bìa mềm', '23 x 15.5 x 3', 520,
 NULL, 245000, NULL, 30, 234,
 N'Nghệ thuật viết code sạch',
 N'Cuốn sách kinh điển về kỹ thuật lập trình.',
 6, 'products/clean-code.jpg', 1, 0, 0, 1, 0, 4.8, 456, GETDATE()),

(22, N'Design Patterns', 'Gang of Four', '9786042456789', N'NXB Thế Giới', 2021,
 N'Tiếng Việt', 512, N'Bìa mềm', '23 x 15.5 x 3.5', 580,
 NULL, 265000, NULL, 25, 156,
 N'23 mẫu thiết kế phần mềm',
 N'Các mẫu thiết kế cơ bản trong lập trình.',
 6, 'products/design-patterns.jpg', 0, 0, 0, 1, 0, 4.6, 234, GETDATE()),

(23, N'ASP.NET Core từ A-Z', N'Nguyễn Văn Dev', '9786042567890', N'NXB Thông Tin', 2021,
 N'Tiếng Việt', 680, N'Bìa mềm', '24 x 16 x 4', 750,
 NULL, 299000, NULL, 20, 123,
 N'Hướng dẫn toàn diện ASP.NET Core',
 N'Từ cơ bản đến nâng cao về ASP.NET Core.',
 6, 'products/aspnet-core.jpg', 0, 1, 0, 1, 0, 4.7, 89, GETDATE()),

(24, N'JavaScript: The Good Parts', 'Douglas Crockford', '9786042678901', N'NXB Thế Giới', 2019,
 N'Tiếng Việt', 176, N'Bìa mềm', '19 x 13 x 1.2', 210,
 138000, 118000, 14, 35, 267,
 N'Những điều tốt nhất về JavaScript',
 N'Hướng dẫn viết JavaScript hiệu quả.',
 6, 'products/placeholder.svg', 0, 0, 0, 1, 0, 4.5, 345, GETDATE()),

(25, N'Python Crash Course', 'Eric Matthes', '9786042789012', N'NXB Thế Giới', 2021,
 N'Tiếng Việt', 544, N'Bìa mềm', '23 x 15.5 x 3.2', 600,
 228000, 199000, 13, 28, 189,
 N'Học Python từ đầu',
 N'Hướng dẫn Python cho người mới bắt đầu.',
 6, 'products/placeholder.svg', 0, 1, 0, 1, 0, 4.7, 456, GETDATE()),

-- Kinh doanh (5 sách)
(26, N'Làm Giàu Từ Chứng Khoán', N'Võ Hải Triều', '9786042301235', N'NXB Lao Động', 2021,
 N'Tiếng Việt', 280, N'Bìa mềm', '21 x 14 x 1.5', 300,
 NULL, 145000, NULL, 35, 345,
 N'Đầu tư chứng khoán cho người Việt',
 N'Kiến thức cơ bản về chứng khoán VN.',
 4, 'products/lam-giau-chung-khoan.svg', 0, 1, 0, 1, 0, 4.3, 234, GETDATE()),

(27, N'Thinking Fast and Slow', 'Daniel Kahneman', '9786042890123', N'NXB Thế Giới', 2020,
 N'Tiếng Việt', 512, N'Bìa cứng', '23 x 15.5 x 3.4', 650,
 265000, 225000, 15, 22, 456,
 N'Tâm lý học nhận thức',
 N'Cách bộ não đưa ra quyết định.',
 7, 'products/placeholder.svg', 1, 0, 0, 1, 0, 4.8, 678, GETDATE()),

(28, N'The Lean Startup', 'Eric Ries', '9786042901234', N'NXB Thế Giới', 2019,
 N'Tiếng Việt', 336, N'Bìa mềm', '21 x 14 x 2', 360,
 172000, 149000, 13, 38, 289,
 N'Khởi nghiệp thông minh',
 N'Phương pháp startup hiện đại.',
 4, 'products/placeholder.svg', 0, 0, 0, 1, 0, 4.6, 567, GETDATE()),

(29, N'Zero to One', 'Peter Thiel', '9786043012345', N'NXB Thế Giới', 2020,
 N'Tiếng Việt', 224, N'Bìa mềm', '21 x 14 x 1.4', 250,
 148000, 129000, 13, 42, 378,
 N'Tạo ra điều chưa từng có',
 N'Khởi nghiệp và đổi mới sáng tạo.',
 4, 'products/placeholder.svg', 0, 1, 0, 1, 0, 4.7, 456, GETDATE()),

(30, N'The 4-Hour Work Week', 'Tim Ferriss', '9786043123456', N'NXB Thế Giới', 2019,
 N'Tiếng Việt', 416, N'Bìa mềm', '21 x 14 x 2.5', 440,
 185000, 159000, 14, 31, 267,
 N'Làm việc hiệu quả, sống tự do',
 N'Tối ưu hóa công việc và cuộc sống.',
 3, 'products/placeholder.svg', 0, 0, 0, 1, 0, 4.5, 389, GETDATE()),

-- Lịch sử - Chính trị (2 sách)
(31, N'Sapiens: Lược Sử Loài Người', 'Yuval Noah Harari', '9786043234567', N'NXB Thế Giới', 2018,
 N'Tiếng Việt', 512, N'Bìa cứng', '23 x 15.5 x 3.4', 680,
 298000, 249000, 16, 18, 1234,
 N'Từ thời tiền sử đến hiện đại',
 N'Lịch sử nhân loại đầy thú vị.',
 8, 'products/placeholder.svg', 1, 0, 1, 1, 0, 4.9, 2345, GETDATE()),

(32, N'21 Bài Học Cho Thế Kỷ 21', 'Yuval Noah Harari', '9786043345678', N'NXB Thế Giới', 2021,
 N'Tiếng Việt', 448, N'Bìa cứng', '23 x 15.5 x 3', 580,
 275000, 235000, 15, 24, 789,
 N'Thách thức của thế kỷ 21',
 N'Phân tích các vấn đề đương đại.',
 8, 'products/placeholder.svg', 1, 1, 0, 1, 0, 4.8, 1234, GETDATE());

SET IDENTITY_INSERT Products OFF;
GO

PRINT '✅ Inserted 32 products';
GO

-- ============================================================
-- INSERT DEMO ORDERS FOR REPORTS (Last 3 months)
-- ============================================================
SET IDENTITY_INSERT Orders ON;

DECLARE @i INT = 1;
DECLARE @OrderDate DATETIME;
DECLARE @CustomerId INT;
DECLARE @TotalAmount DECIMAL(18,2);
DECLARE @Status NVARCHAR(50);
DECLARE @PaymentStatus NVARCHAR(50);

WHILE @i <= 50
BEGIN
    -- Random date in last 90 days
    SET @OrderDate = DATEADD(day, -FLOOR(RAND() * 90), GETDATE());
    
    -- Random customer (2-5)
    SET @CustomerId = 2 + FLOOR(RAND() * 4);
    
    -- Random total (100k - 2M)
    SET @TotalAmount = 100000 + FLOOR(RAND() * 1900000);
    
    -- Status based on date
    IF @OrderDate < DATEADD(day, -7, GETDATE())
        SET @Status = CASE FLOOR(RAND() * 3)
            WHEN 0 THEN 'Completed'
            WHEN 1 THEN 'Shipped'
            ELSE 'Cancelled'
        END
    ELSE
        SET @Status = CASE FLOOR(RAND() * 4)
            WHEN 0 THEN 'Pending'
            WHEN 1 THEN 'Processing'
            WHEN 2 THEN 'Shipped'
            ELSE 'Completed'
        END;
    
    SET @PaymentStatus = CASE 
        WHEN @Status IN ('Completed', 'Shipped') THEN 'Paid'
        WHEN @Status = 'Cancelled' THEN 'Failed'
        ELSE 'Pending'
    END;
    
    INSERT INTO Orders (
        OrderId, UserId, OrderDate, TotalAmount, Status,
        RecipientName, PhoneNumber, Email, ShippingAddress,
        City, District, Ward, Notes,
        PaymentMethod, PaymentStatus, PaidAt
    )
    VALUES (
        @i, @CustomerId, @OrderDate, @TotalAmount, @Status,
        N'Khách hàng ' + CAST(@i AS NVARCHAR), '09' + RIGHT('00000000' + CAST(@i AS NVARCHAR), 8),
        'customer' + CAST(@CustomerId AS NVARCHAR) + '@gmail.com',
        N'Số ' + CAST(@i AS NVARCHAR) + N' Nguyễn Văn Cừ',
        N'TP. Hồ Chí Minh', N'Quận 5', N'Phường 1', N'Giao hàng giờ hành chính',
        CASE FLOOR(RAND() * 3)
            WHEN 0 THEN 'COD'
            WHEN 1 THEN 'BankTransfer'
            ELSE 'VNPay'
        END,
        @PaymentStatus,
        CASE WHEN @PaymentStatus = 'Paid' THEN @OrderDate ELSE NULL END
    );
    
    SET @i = @i + 1;
END;

SET IDENTITY_INSERT Orders OFF;
GO

PRINT '✅ Inserted 50 demo orders';
GO

-- Insert OrderDetails for demo orders
SET IDENTITY_INSERT OrderDetails ON;

DECLARE @OrderDetailId INT = 1;
DECLARE @OrderId INT = 1;
DECLARE @ProductId INT;
DECLARE @Quantity INT;
DECLARE @UnitPrice DECIMAL(18,2);

WHILE @OrderId <= 50
BEGIN
    -- Each order has 1-4 products
    DECLARE @ProductCount INT = 1 + FLOOR(RAND() * 4);
    DECLARE @j INT = 1;
    
    WHILE @j <= @ProductCount
    BEGIN
        SET @ProductId = 1 + FLOOR(RAND() * 32);
        SET @Quantity = 1 + FLOOR(RAND() * 3);
        SET @UnitPrice = (SELECT Price FROM Products WHERE ProductId = @ProductId);
        
        INSERT INTO OrderDetails (OrderDetailId, OrderId, ProductId, Quantity, UnitPrice)
        VALUES (@OrderDetailId, @OrderId, @ProductId, @Quantity, @UnitPrice);
        
        SET @OrderDetailId = @OrderDetailId + 1;
        SET @j = @j + 1;
    END;
    
    SET @OrderId = @OrderId + 1;
END;

SET IDENTITY_INSERT OrderDetails OFF;
GO

PRINT '✅ Inserted order details';
GO

-- ============================================================
-- VERIFICATION
-- ============================================================
PRINT '';
PRINT '============================================================';
PRINT '📊 DATABASE SEEDED SUCCESSFULLY!';
PRINT '============================================================';
PRINT '';

SELECT 'Users' AS TableName, COUNT(*) AS RecordCount FROM Users
UNION ALL
SELECT 'Categories', COUNT(*) FROM Categories
UNION ALL
SELECT 'Products', COUNT(*) FROM Products
UNION ALL
SELECT 'Orders', COUNT(*) FROM Orders
UNION ALL
SELECT 'OrderDetails', COUNT(*) FROM OrderDetails;

PRINT '';
PRINT '📚 Product Summary:';
SELECT 
    c.CategoryName,
    COUNT(*) AS ProductCount,
    AVG(p.Price) AS AvgPrice,
    SUM(p.Stock) AS TotalStock
FROM Products p
JOIN Categories c ON p.CategoryId = c.CategoryId
GROUP BY c.CategoryName
ORDER BY COUNT(*) DESC;

PRINT '';
PRINT '📊 Order Statistics (Last 90 days):';
SELECT 
    Status,
    COUNT(*) AS OrderCount,
    SUM(TotalAmount) AS TotalRevenue,
    AVG(TotalAmount) AS AvgOrderValue
FROM Orders
WHERE OrderDate >= DATEADD(day, -90, GETDATE())
GROUP BY Status
ORDER BY OrderCount DESC;

PRINT '';
PRINT '✅ Seeding complete! Ready for testing and reports.';
GO
