# 🔍 Kiểm tra Database có dữ liệu

## Cách 1: Dùng SQL Server Management Studio (SSMS)

1. Mở SSMS
2. Connect tới `.\SQLEXPRESS`
3. Tìm database `BookStoreDb`
4. Run các query sau:

```sql
-- Kiểm tra Users
SELECT COUNT(*) AS UserCount FROM Users;
SELECT * FROM Users;

-- Kiểm tra Categories
SELECT COUNT(*) AS CategoryCount FROM Categories;
SELECT * FROM Categories;

-- Kiểm tra Products
SELECT COUNT(*) AS ProductCount FROM Products;
SELECT TOP 10 ProductId, ProductName, Author, Price, Stock FROM Products;

-- Kiểm tra chi tiết 1 sản phẩm
SELECT * FROM Products WHERE ProductId = 1;
```

## Cách 2: Dùng Visual Studio SQL Server Object Explorer

1. View -> SQL Server Object Explorer
2. Connect to: `(localdb)\MSSQLLocalDB` hoặc `.\SQLEXPRESS`
3. Databases -> BookStoreDb -> Tables
4. Right-click table -> View Data

## Cách 3: Dùng .NET CLI query

Tạo file `check-db.ps1`:

```powershell
# Check database from PowerShell
$connectionString = "Server=.\SQLEXPRESS;Database=BookStoreDb;Trusted_Connection=True;MultipleActiveResultSets=true"

Add-Type -AssemblyName "System.Data"

$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)

try {
    $connection.Open()
    Write-Host "✅ Connected to database!" -ForegroundColor Green
    
    # Check Users
    $cmd = $connection.CreateCommand()
    $cmd.CommandText = "SELECT COUNT(*) FROM Users"
    $userCount = $cmd.ExecuteScalar()
    Write-Host "Users: $userCount" -ForegroundColor Cyan
    
    # Check Categories
    $cmd.CommandText = "SELECT COUNT(*) FROM Categories"
    $catCount = $cmd.ExecuteScalar()
    Write-Host "Categories: $catCount" -ForegroundColor Cyan
    
    # Check Products
    $cmd.CommandText = "SELECT COUNT(*) FROM Products"
    $productCount = $cmd.ExecuteScalar()
    Write-Host "Products: $productCount" -ForegroundColor Cyan
    
    if ($productCount -gt 0) {
        Write-Host "`n📚 Sample Products:" -ForegroundColor Yellow
        $cmd.CommandText = "SELECT TOP 5 ProductName, Author, Price FROM Products"
        $reader = $cmd.ExecuteReader()
        while ($reader.Read()) {
            Write-Host "  - $($reader['ProductName']) by $($reader['Author']) - $($reader['Price']) VND"
        }
        $reader.Close()
    }
}
catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}
finally {
    $connection.Close()
}
```

Chạy: `.\check-db.ps1`

## Cách 4: Thêm logging vào DbInitializer

Sửa file `Data/DbInitializer.cs`, thêm console output:

```csharp
public static void Initialize(AppDbContext context)
{
    Console.WriteLine("🔍 Checking if database needs seeding...");
    
    // Nếu đã có dữ liệu thì thôi
    if (context.Users.Any())
    {
        Console.WriteLine("⚠️ Database already has data. Skipping seed.");
        return;
    }

    Console.WriteLine("✅ Starting database seeding...");
    
    // ... rest of code ...
    
    context.Products.AddRange(products);
    context.SaveChanges();

    Console.WriteLine($"✅ Database seeded successfully!");
    Console.WriteLine($"   - Users: 3");
    Console.WriteLine($"   - Categories: 6");
    Console.WriteLine($"   - Products: {products.Count}");
}
```

## 🔧 Troubleshooting

### Vấn đề 1: Database có data cũ
**Giải pháp**: Xóa và tạo lại database

```bash
dotnet ef database drop --force
dotnet ef database update
dotnet run
```

### Vấn đề 2: DbInitializer không chạy
**Nguyên nhân**: `context.Users.Any()` return `true` (đã có data)

**Giải pháp**: Xóa tất cả data hoặc sửa logic:

```csharp
// Thay vì:
if (context.Users.Any()) return;

// Dùng:
if (context.Products.Any()) return; // Chỉ check Products
```

### Vấn đề 3: Connection String sai
**Check**: `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=BookStoreDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

### Vấn đề 4: Migration chưa apply
**Giải pháp**:

```bash
dotnet ef migrations list
dotnet ef database update
```

## ✅ Expected Results

Nếu seed thành công, bạn sẽ thấy:

- **Users**: 3 (admin, user1, user2)
- **Categories**: 6
- **Products**: 17

### Sample Product Data:

| ProductId | ProductName | Author | Price | Stock |
|-----------|-------------|--------|-------|-------|
| 1 | Nhà Giả Kim | Paulo Coelho | 120,000 | 50 |
| 2 | Cây Cam Ngọt Của Tôi | José Mauro de Vasconcelos | 108,000 | 35 |
| 3 | Mắt Biếc | Nguyễn Nhật Ánh | 110,000 | 45 |

## 🚀 Quick Test via Browser

1. Run app: `F5` in Visual Studio
2. Navigate to: `https://localhost:xxxx/Product`
3. Should see 17 books displayed

If you see books → Database has data ✅  
If you see "Không có sản phẩm" → Database is empty ❌

## 📧 Admin Login Test

1. Go to: `/Account/Login`
2. Email: `admin@bookstore.com`
3. Password: `admin123`
4. Go to: `/Admin/Products`
5. Should see 17 products

## 🔄 Force Re-seed

Nếu muốn tạo lại data hoàn toàn:

```bash
# 1. Drop database
dotnet ef database drop --force

# 2. Re-create and migrate
dotnet ef database update

# 3. Run app (will auto-seed)
dotnet run
```

Hoặc trong Visual Studio: **Debug > Start Debugging (F5)**

---

Nếu vẫn không có data, check Console output khi app startup. Bạn sẽ thấy message từ DbInitializer.
