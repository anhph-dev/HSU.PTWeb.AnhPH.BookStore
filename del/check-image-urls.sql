-- Check ImageUrl in database
USE BookStoreDb;
GO

SELECT 
    ProductId,
    ProductName,
    ImageUrl,
    '~/images/' + ImageUrl AS FullPath
FROM Products
ORDER BY ProductId;
GO
