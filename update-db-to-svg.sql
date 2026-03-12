-- Update ImageUrl in database to use .svg extension
USE BookStoreDb;
GO

UPDATE Products 
SET ImageUrl = REPLACE(ImageUrl, '.jpg', '.svg')
WHERE ImageUrl LIKE '%.jpg';

PRINT '✅ Updated ImageUrl to use .svg extension';

-- Verify
SELECT ProductId, ProductName, ImageUrl 
FROM Products
ORDER BY ProductId;
GO
