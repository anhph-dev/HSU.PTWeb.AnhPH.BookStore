# QUICK INSTRUCTIONS - Seed Database

## 🚀 Làm ngay bây giờ:

### Step 1: Chạy App
```
Trong Visual Studio:
- Press F5 (Start Debugging)
```

HOẶC terminal:
```powershell
dotnet run
```

### Step 2: Xem Console Output
Bạn sẽ thấy:
```
============================================================
🔍 DbInitializer.Initialize() called
============================================================
📊 Current Products count: 0
✅ Starting database seeding...
✅ Created 3 users.
✅ Created 6 categories.
✅ Đã thêm 17 sản phẩm vào cơ sở dữ liệu.
✅ Database seeding completed!
============================================================
```

### Step 3: Verify Data
Sau khi app chạy (khoảng 5 giây), chạy:
```powershell
.\check-database.ps1
```

Hoặc truy cập browser:
```
https://localhost:xxxx/Product
```

## ✅ Expected Result

Sau khi F5 và app start, bạn sẽ thấy:
- Users: 3
- Categories: 6  
- Products: 17

## 🔧 Nếu vẫn không có data

Check Output window trong Visual Studio:
1. View → Output
2. Show output from: **Debug**
3. Look for console messages từ DbInitializer

Nếu KHÔNG thấy message → DbInitializer không chạy
Nếu CÓ message nhưng vẫn không có data → Check connection string

## 📝 Connection String hiện tại
```
Server=.\SQLEXPRESS;Database=BookStoreDb;Trusted_Connection=True;TrustServerCertificate=True
```

---
**TL;DR**: Press F5 → Wait 5s → Run `.\check-database.ps1`
