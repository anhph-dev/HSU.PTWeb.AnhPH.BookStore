-- Update ImageUrl to ensure correct filenames
USE BookStoreDb;
GO

-- Check current ImageUrl values
SELECT ProductId, ProductName, ImageUrl 
FROM Products
ORDER BY ProductId;

-- If ImageUrl contains GUID, update them:
/*
UPDATE Products SET ImageUrl = 'products/nha-gia-kim.jpg' WHERE ProductId = 1;
UPDATE Products SET ImageUrl = 'products/cay-cam-ngot.jpg' WHERE ProductId = 2;
UPDATE Products SET ImageUrl = 'products/mat-biec.jpg' WHERE ProductId = 3;
UPDATE Products SET ImageUrl = 'products/hoa-vang-co-xanh.jpg' WHERE ProductId = 4;
UPDATE Products SET ImageUrl = 'products/dac-nhan-tam.jpg' WHERE ProductId = 5;
UPDATE Products SET ImageUrl = 'products/sapiens.jpg' WHERE ProductId = 6;
UPDATE Products SET ImageUrl = 'products/tu-duy-nhanh-cham.jpg' WHERE ProductId = 7;
UPDATE Products SET ImageUrl = 'products/atomic-habits.jpg' WHERE ProductId = 8;
UPDATE Products SET ImageUrl = 'products/khoi-nghiep-tinh-gon.jpg' WHERE ProductId = 9;
UPDATE Products SET ImageUrl = 'products/7-thoi-quen.jpg' WHERE ProductId = 10;
UPDATE Products SET ImageUrl = 'products/lam-giau-chung-khoan.jpg' WHERE ProductId = 11;
UPDATE Products SET ImageUrl = 'products/de-men.jpg' WHERE ProductId = 12;
UPDATE Products SET ImageUrl = 'products/hoang-tu-be.jpg' WHERE ProductId = 13;
UPDATE Products SET ImageUrl = 'products/clean-code.jpg' WHERE ProductId = 14;
UPDATE Products SET ImageUrl = 'products/design-patterns.jpg' WHERE ProductId = 15;
UPDATE Products SET ImageUrl = 'products/aspnet-core.jpg' WHERE ProductId = 16;

PRINT '✅ Updated all ImageUrl values';
*/
GO
