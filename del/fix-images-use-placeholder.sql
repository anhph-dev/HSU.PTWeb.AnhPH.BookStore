-- Update all products to use placeholder image temporarily
USE BookStoreDb;
GO

UPDATE Products 
SET ImageUrl = 'products/placeholder.svg'
WHERE ImageUrl LIKE 'products/%.jpg';

PRINT '✅ Updated all products to use placeholder image';

SELECT 
    ProductId, 
    ProductName, 
    ImageUrl 
FROM Products;
GO
