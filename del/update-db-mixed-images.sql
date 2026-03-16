-- Update ImageUrl to use correct extensions (JPG for real images, SVG for placeholders)
USE BookStoreDb;
GO

-- Update products that have real JPG images downloaded
UPDATE Products SET ImageUrl = 'products/nha-gia-kim.jpg' WHERE ProductId = 1;
UPDATE Products SET ImageUrl = 'products/cay-cam-ngot.jpg' WHERE ProductId = 2;
UPDATE Products SET ImageUrl = 'products/atomic-habits.jpg' WHERE ProductId = 8;
UPDATE Products SET ImageUrl = 'products/clean-code.jpg' WHERE ProductId = 14;
UPDATE Products SET ImageUrl = 'products/design-patterns.jpg' WHERE ProductId = 15;
UPDATE Products SET ImageUrl = 'products/khoi-nghiep-tinh-gon.jpg' WHERE ProductId = 9;
UPDATE Products SET ImageUrl = 'products/aspnet-core.jpg' WHERE ProductId = 16;

-- Keep SVG placeholders for products without real images
UPDATE Products SET ImageUrl = 'products/mat-biec.svg' WHERE ProductId = 3;
UPDATE Products SET ImageUrl = 'products/hoa-vang-co-xanh.svg' WHERE ProductId = 4;
UPDATE Products SET ImageUrl = 'products/dac-nhan-tam.svg' WHERE ProductId = 5;
UPDATE Products SET ImageUrl = 'products/sapiens.svg' WHERE ProductId = 6;
UPDATE Products SET ImageUrl = 'products/tu-duy-nhanh-cham.svg' WHERE ProductId = 7;
UPDATE Products SET ImageUrl = 'products/7-thoi-quen.svg' WHERE ProductId = 10;
UPDATE Products SET ImageUrl = 'products/lam-giau-chung-khoan.svg' WHERE ProductId = 11;
UPDATE Products SET ImageUrl = 'products/de-men.svg' WHERE ProductId = 12;
UPDATE Products SET ImageUrl = 'products/hoang-tu-be.svg' WHERE ProductId = 13;

PRINT '✅ Updated ImageUrl - 7 JPG real images + 9 SVG placeholders';

-- Verify
SELECT 
    ProductId,
    ProductName,
    ImageUrl,
    CASE 
        WHEN ImageUrl LIKE '%.jpg' THEN '📷 Real Image'
        WHEN ImageUrl LIKE '%.svg' THEN '🎨 Placeholder'
        ELSE '❓ Unknown'
    END AS ImageType
FROM Products
ORDER BY ProductId;
GO
