# 📸 Hướng dẫn tải hình ảnh bìa sách thật

## 🎯 Mục tiêu
Thay thế colored placeholders bằng hình ảnh bìa sách thật để UI đẹp hơn.

## 📋 Danh sách cần tải (16 hình)

### Văn học (4 hình)
1. **nha-gia-kim.jpg** - Nhà Giả Kim (Paulo Coelho)
2. **cay-cam-ngot.jpg** - Cây Cam Ngọt Của Tôi
3. **mat-biec.jpg** - Mắt Biếc (Nguyễn Nhật Ánh)
4. **hoa-vang-co-xanh.jpg** - Tôi Thấy Hoa Vàng Trên Cỏ Xanh

### Kỹ năng sống (4 hình)
5. **dac-nhan-tam.jpg** - Đắc Nhân Tâm
6. **sapiens.jpg** - Sapiens
7. **tu-duy-nhanh-cham.jpg** - Tư Duy Nhanh Và Chậm
8. **atomic-habits.jpg** - Atomic Habits

### Kinh doanh (3 hình)
9. **khoi-nghiep-tinh-gon.jpg** - Khởi Nghiệp Tinh Gọn
10. **7-thoi-quen.jpg** - 7 Thói Quen Của Người Thành Đạt
11. **lam-giau-chung-khoan.jpg** - Làm Giàu Từ Chứng Khoán

### Thiếu nhi (2 hình)
12. **de-men.jpg** - Dế Mèn Phiêu Lưu Ký
13. **hoang-tu-be.jpg** - Hoàng Tử Bé

### Lập trình (3 hình)
14. **clean-code.jpg** - Clean Code
15. **design-patterns.jpg** - Design Patterns
16. **aspnet-core.jpg** - ASP.NET Core

## 🔍 Cách tìm hình ảnh

### Cách 1: Tiki.vn (Recommended)
1. Vào https://tiki.vn/
2. Search tên sách (vd: "Nhà Giả Kim")
3. Click vào sách → Chuột phải vào hình → "Save image as..."
4. Đổi tên file theo danh sách trên
5. Save vào `wwwroot\images\products\`

### Cách 2: Fahasa.com
1. Vào https://fahasa.com/
2. Search và download tương tự

### Cách 3: Google Images
1. Google: "[Tên sách] book cover vietnam"
2. Chọn hình có độ phân giải cao
3. Download và rename

### Cách 4: Amazon/Goodreads (sách nước ngoài)
1. Vào Amazon.com hoặc Goodreads.com
2. Search book title in English
3. Download cover image

## 📐 Yêu cầu kỹ thuật

- **Format**: JPG hoặc PNG
- **Kích thước đề nghị**: 300x400px đến 600x800px
- **Tỷ lệ**: 3:4 (portrait)
- **Dung lượng**: < 500KB mỗi file
- **Tên file**: Chính xác như danh sách (lowercase, no spaces)

## 📂 Thư mục lưu
```
wwwroot\images\products\
```

## ✅ Kiểm tra

Sau khi tải xong:
1. Refresh browser (Ctrl+F5)
2. Check từng product card
3. Nếu vẫn thấy placeholder → check tên file

## 🎨 Tips để UI đẹp hơn

1. **Crop ảnh đồng nhất** - Dùng tool như Photoshop/GIMP
2. **Nén ảnh** - Dùng TinyPNG.com để giảm dung lượng
3. **Chất lượng cao** - Chọn ảnh >300x400px
4. **Background trắng** - Tốt nhất cho book covers

## 🚀 Quick Start (5 phút)

Nếu không muốn tải 16 hình, chỉ cần tải **5 hình phổ biến nhất**:
1. Nhà Giả Kim ⭐
2. Đắc Nhân Tâm ⭐
3. Mắt Biếc ⭐
4. Sapiens ⭐
5. Atomic Habits ⭐

Các hình còn lại có thể để placeholder.

## 📝 Alternative: Sử dụng placeholder tạm

Nếu chưa tải được hình thật, colored placeholders hiện tại đã đủ đẹp để demo:
- ✅ 16 màu khác nhau
- ✅ Hiển thị tên sách
- ✅ Professional gradient design
- ✅ SVG format (vector, responsive)

## 🔗 Link tham khảo

- Tiki: https://tiki.vn/
- Fahasa: https://fahasa.com/
- Google Books: https://books.google.com/
- Amazon: https://amazon.com/
- Goodreads: https://goodreads.com/

---

**Kết luận**: App đang chạy tốt với placeholder. Bạn có thể:
- ✅ Demo ngay với placeholder (đủ đẹp)
- ⏰ Tải hình thật sau khi rảnh
- 🎨 Hoặc thuê designer tạo cover đẹp hơn

**Current Status**: 🟢 WORKING với colored placeholders
