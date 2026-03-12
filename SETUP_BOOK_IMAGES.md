# 📚 Hướng dẫn Setup Hình ảnh Sản phẩm

## 🎯 Danh sách hình ảnh cần có

Tạo thư mục `wwwroot/images/products/` và thêm các file sau:

### Sách Văn học
1. **nha-gia-kim.jpg** - Nhà Giả Kim
2. **cay-cam-ngot.jpg** - Cây Cam Ngọt Của Tôi
3. **mat-biec.jpg** - Mắt Biếc
4. **hoa-vang-co-xanh.jpg** - Tôi Thấy Hoa Vàng Trên Cỏ Xanh

### Sách Kỹ năng sống
5. **dac-nhan-tam.jpg** - Đắc Nhân Tâm
6. **sapiens.jpg** - Sapiens
7. **tu-duy-nhanh-cham.jpg** - Tư Duy Nhanh Và Chậm
8. **atomic-habits.jpg** - Atomic Habits

### Sách Kinh doanh
9. **khoi-nghiep-tinh-gon.jpg** - Khởi Nghiệp Tinh Gọn
10. **7-thoi-quen.jpg** - 7 Thói Quen Của Người Thành Đạt
11. **lam-giau-chung-khoan.jpg** - Làm Giàu Từ Chứng Khoán

### Sách Thiếu nhi
12. **de-men.jpg** - Dế Mèn Phiêu Lưu Ký
13. **hoang-tu-be.jpg** - Hoàng Tử Bé

### Sách Lập trình
14. **clean-code.jpg** - Clean Code
15. **design-patterns.jpg** - Design Patterns
16. **aspnet-core.jpg** - ASP.NET Core

### Hình placeholder
17. **placeholder.svg** - Hình mặc định khi không có ảnh

## 📥 Cách 1: Tự tìm hình trên mạng

1. Google Images tìm với từ khóa: "[Tên sách] book cover"
2. Tải về và đặt tên đúng như trên
3. Copy vào `wwwroot/images/products/`

## 📥 Cách 2: Sử dụng PowerShell Script để tạo placeholder

Chạy script sau trong PowerShell:

```powershell
# Tạo thư mục
New-Item -Path "wwwroot\images\products" -ItemType Directory -Force

# Danh sách tên file
$books = @(
    "nha-gia-kim",
    "cay-cam-ngot",
    "mat-biec",
    "hoa-vang-co-xanh",
    "dac-nhan-tam",
    "sapiens",
    "tu-duy-nhanh-cham",
    "atomic-habits",
    "khoi-nghiep-tinh-gon",
    "7-thoi-quen",
    "lam-giau-chung-khoan",
    "de-men",
    "hoang-tu-be",
    "clean-code",
    "design-patterns",
    "aspnet-core"
)

# Tạo file SVG placeholder cho mỗi sách
foreach ($book in $books) {
    $svg = @"
<svg xmlns="http://www.w3.org/2000/svg" width="300" height="400">
  <rect width="300" height="400" fill="#e9ecef"/>
  <text x="50%" y="50%" font-family="Arial" font-size="20" fill="#6c757d" text-anchor="middle" dominant-baseline="middle">
    $book
  </text>
</svg>
"@
    $svg | Out-File -FilePath "wwwroot\images\products\$book.jpg" -Encoding UTF8
}

Write-Host "✅ Đã tạo placeholder cho các sách!" -ForegroundColor Green
```

## 📥 Cách 3: Download từ URLs (Recommended)

Tạo file `download-book-covers.ps1`:

```powershell
# Download book covers from online sources

$images = @{
    "nha-gia-kim.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/45/3b/fc/aa3c55b071c74fc37aee6fabe2c02f6e.jpg.webp"
    "cay-cam-ngot.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/5e/18/24/2a6154ba08df6ce6161c13f4303fa19e.jpg.webp"
    "mat-biec.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/d9/ce/36/5e4c0d0d0e6c6c6e6f7d7d7d7d7d7d7d.jpg.webp"
    "hoa-vang-co-xanh.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/ee/c4/d0/cddf34fe6fa9f6bb332d26d6f6f0e6b0.jpg.webp"
    "dac-nhan-tam.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/c7/8e/fa/09d6cddd5db3188aa2b1e463c2a8f18d.jpg.webp"
    "sapiens.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/b8/14/08/6f83c37e4f0b7879c1ce7d4c3a4d1c9f.jpg.webp"
    "atomic-habits.jpg" = "https://salt.tikicdn.com/cache/750x750/ts/product/4c/55/37/ad27d5ddbaa799f8e3c6f0a4c4d9c4c7.jpg.webp"
}

New-Item -Path "wwwroot\images\products" -ItemType Directory -Force

foreach ($image in $images.GetEnumerator()) {
    try {
        Invoke-WebRequest -Uri $image.Value -OutFile "wwwroot\images\products\$($image.Key)"
        Write-Host "✅ Downloaded: $($image.Key)" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ Failed: $($image.Key)" -ForegroundColor Red
    }
}
```

Chạy: `.\download-book-covers.ps1`

## 🖼️ Placeholder SVG

Tạo file `wwwroot/images/products/placeholder.svg`:

```xml
<svg xmlns="http://www.w3.org/2000/svg" width="300" height="400" viewBox="0 0 300 400">
  <rect width="300" height="400" fill="#e9ecef"/>
  <g transform="translate(150, 200)">
    <path d="M-40,-60 L-40,60 L0,60 L0,-60 M10,-60 L10,60 L50,60 L50,-60" fill="#6c757d" opacity="0.3"/>
    <text y="90" font-family="Arial" font-size="16" fill="#6c757d" text-anchor="middle">
      No Image
    </text>
  </g>
</svg>
```

## ✅ Kiểm tra

Sau khi setup xong, kiểm tra:

1. Browse to: `https://localhost:xxxx/images/products/nha-gia-kim.jpg`
2. Xem có hiển thị ảnh không
3. Run app và xem Product Index page

## 📝 Notes

- Kích thước đề nghị: **300x400px** đến **600x800px**
- Format: JPG hoặc PNG
- Dung lượng: < 500KB mỗi ảnh
- Nếu không có ảnh nào, app sẽ tự động dùng `placeholder.svg`

## 🎨 Design Tips

Để UI đẹp hơn:
- Sử dụng ảnh bìa sách thật
- Đồng nhất tỷ lệ khung hình (3:4)
- Nén ảnh để tối ưu tốc độ load
- Sử dụng lazy loading cho ảnh

## 🔗 Nguồn ảnh gợi ý

- Tiki.vn
- Fahasa.com
- Amazon.com (search sách tiếng Việt)
- Google Books
- Goodreads.com
