# ✅ REFACTORING COMPLETED & NEXT ACTIONS

## 🎉 What's Been Done (100% Clean System)

### ✅ Phase 1: Complete System Reset
- ✅ Dropped old database completely
- ✅ Removed all old migrations
- ✅ Created fresh migration: `InitialCreateClean`
- ✅ New clean database schema with:
  - User with PasswordHash field
  - Order with shipping & payment fields
  - All validation attributes

### ✅ Phase 2: Security Infrastructure
- ✅ Installed BCrypt.Net-Next package
- ✅ Created IPasswordHasher interface
- ✅ Created PasswordHasher service
- ✅ Registered in DI container

### ✅ Phase 3: Data Seeding
- ✅ Created comprehensive SQL seed script with:
  - 5 users (1 admin + 4 customers) with BCrypt hashed passwords
  - 8 categories
  - **32 products** (real Vietnamese books)
  - **50 demo orders** (last 90 days for reports)
  - Order details with random products

### ✅ Phase 4: Code Cleanup
- ✅ Removed debug console logging
- ✅ Deleted unused/duplicate files
- ✅ Clean User model with validation
- ✅ Clean Order model with shipping fields

---

## 🚀 WHAT NEEDS TO BE DONE NEXT

### 🔐 Priority 1: Update AccountController (30 minutes)

**Current Issue**: Controller still uses plain-text passwords

**Fix Required**:
```csharp
// File: Controllers/AccountController.cs

// Inject IPasswordHasher
private readonly IPasswordHasher _passwordHasher;

public AccountController(AppDbContext context, IPasswordHasher passwordHasher)
{
    _context = context;
    _passwordHasher = passwordHasher;
}

// In Register action:
var user = new User
{
    Email = model.Email,
    FullName = model.FullName,
    PasswordHash = _passwordHasher.HashPassword(model.Password), // Use this
    Role = "Customer"
};

// In Login action:
var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
if (user != null && _passwordHasher.VerifyPassword(model.Password, user.PasswordHash))
{
    // Login success
}
```

---

### 💳 Priority 2: Update OrderController Checkout (45 minutes)

**Current Issue**: No shipping address collection, no validation

**Required Changes**:

1. **Create CheckoutViewModel**:
```csharp
// File: ViewModels/CheckoutViewModel.cs
public class CheckoutViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    public string RecipientName { get; set; }

    [Required, Phone]
    public string PhoneNumber { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string ShippingAddress { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string District { get; set; }

    public string Ward { get; set; }
    public string Notes { get; set; }

    [Required]
    public string PaymentMethod { get; set; } // COD, BankTransfer
}
```

2. **Update OrderController.Checkout**:
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Checkout(CheckoutViewModel model)
{
    if (!ModelState.IsValid)
        return View(model);

    var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");
    
    var order = new Order
    {
        UserId = GetCurrentUserId(),
        OrderDate = DateTime.Now,
        RecipientName = model.RecipientName,
        PhoneNumber = model.PhoneNumber,
        Email = model.Email,
        ShippingAddress = model.ShippingAddress,
        City = model.City,
        District = model.District,
        Ward = model.Ward,
        Notes = model.Notes,
        PaymentMethod = model.PaymentMethod,
        PaymentStatus = "Pending",
        Status = "Pending",
        TotalAmount = cart.Sum(c => c.Quantity * c.Price)
    };

    // Add order details
    foreach (var item in cart)
    {
        order.OrderDetails.Add(new OrderDetail
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            UnitPrice = item.Price
        });

        // Deduct stock
        var product = await _context.Products.FindAsync(item.ProductId);
        product.Stock -= item.Quantity;
    }

    _context.Orders.Add(order);
    await _context.SaveChangesAsync();

    // Clear cart
    HttpContext.Session.Remove("cart");

    TempData["SuccessMessage"] = "Đặt hàng thành công!";
    return RedirectToAction("Details", new { id = order.OrderId });
}
```

---

### 🎨 Priority 3: UI Improvements (1 hour)

#### 3.1 Add Toast Notifications to _Layout.cshtml

```html
<!-- Before </body> tag -->
<div class="toast-container position-fixed top-0 end-0 p-3" style="z-index: 9999;">
    @if (TempData["SuccessMessage"] != null)
    {
        <div class="toast show align-items-center text-white bg-success border-0" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="bi bi-check-circle-fill me-2"></i>
                    @TempData["SuccessMessage"]
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    }
    @if (TempData["ErrorMessage"] != null)
    {
        <div class="toast show align-items-center text-white bg-danger border-0" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="bi bi-exclamation-triangle-fill me-2"></i>
                    @TempData["ErrorMessage"]
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>
    }
</div>

<script>
    // Auto-hide toasts after 5 seconds
    setTimeout(() => {
        const toasts = document.querySelectorAll('.toast');
        toasts.forEach(toast => {
            const bsToast = new bootstrap.Toast(toast);
            bsToast.hide();
        });
    }, 5000);
</script>
```

#### 3.2 Add Cart Badge to Navbar

```html
<li class="nav-item">
    <a class="nav-link position-relative" asp-controller="Cart" asp-action="Index">
        <i class="bi bi-cart3 fs-5"></i>
        <span class="visually-hidden">Giỏ hàng</span>
        @{
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(Context.Session, "cart");
            var cartCount = cart?.Sum(c => c.Quantity) ?? 0;
            if (cartCount > 0)
            {
                <span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">
                    @cartCount
                    <span class="visually-hidden">items in cart</span>
                </span>
            }
        }
    </a>
</li>
```

#### 3.3 Improve Checkout View

```html
@model CheckoutViewModel

<div class="container my-5">
    <div class="row">
        <div class="col-lg-8">
            <div class="card shadow-sm">
                <div class="card-header bg-primary text-white">
                    <h4 class="mb-0"><i class="bi bi-truck"></i> Thông tin giao hàng</h4>
                </div>
                <div class="card-body">
                    <form asp-action="Checkout" method="post">
                        @Html.AntiForgeryToken()
                        
                        <div class="row g-3">
                            <div class="col-md-6">
                                <label class="form-label">Họ và tên</label>
                                <input asp-for="RecipientName" class="form-control" />
                                <span asp-validation-for="RecipientName" class="text-danger"></span>
                            </div>
                            <div class="col-md-6">
                                <label class="form-label">Số điện thoại</label>
                                <input asp-for="PhoneNumber" class="form-control" />
                                <span asp-validation-for="PhoneNumber" class="text-danger"></span>
                            </div>
                            <div class="col-12">
                                <label class="form-label">Email</label>
                                <input asp-for="Email" type="email" class="form-control" />
                                <span asp-validation-for="Email" class="text-danger"></span>
                            </div>
                            <div class="col-12">
                                <label class="form-label">Địa chỉ</label>
                                <input asp-for="ShippingAddress" class="form-control" />
                                <span asp-validation-for="ShippingAddress" class="text-danger"></span>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">Tỉnh/Thành phố</label>
                                <select asp-for="City" class="form-select">
                                    <option value="">-- Chọn --</option>
                                    <option>TP. Hồ Chí Minh</option>
                                    <option>Hà Nội</option>
                                    <option>Đà Nẵng</option>
                                </select>
                                <span asp-validation-for="City" class="text-danger"></span>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">Quận/Huyện</label>
                                <input asp-for="District" class="form-control" />
                                <span asp-validation-for="District" class="text-danger"></span>
                            </div>
                            <div class="col-md-4">
                                <label class="form-label">Phường/Xã</label>
                                <input asp-for="Ward" class="form-control" />
                            </div>
                            <div class="col-12">
                                <label class="form-label">Ghi chú</label>
                                <textarea asp-for="Notes" class="form-control" rows="2"></textarea>
                            </div>
                        </div>

                        <hr class="my-4">

                        <h5>Phương thức thanh toán</h5>
                        <div class="form-check">
                            <input class="form-check-input" type="radio" asp-for="PaymentMethod" value="COD" id="cod" checked>
                            <label class="form-check-label" for="cod">
                                <i class="bi bi-cash-coin text-success"></i> Thanh toán khi nhận hàng (COD)
                            </label>
                        </div>
                        <div class="form-check">
                            <input class="form-check-input" type="radio" asp-for="PaymentMethod" value="BankTransfer" id="bank">
                            <label class="form-check-label" for="bank">
                                <i class="bi bi-bank text-primary"></i> Chuyển khoản ngân hàng
                            </label>
                        </div>

                        <div class="d-grid gap-2 mt-4">
                            <button type="submit" class="btn btn-primary btn-lg">
                                <i class="bi bi-check-circle"></i> Đặt hàng
                            </button>
                            <a asp-controller="Cart" asp-action="Index" class="btn btn-outline-secondary">
                                <i class="bi bi-arrow-left"></i> Quay lại giỏ hàng
                            </a>
                        </div>
                    </form>
                </div>
            </div>
        </div>

        <div class="col-lg-4">
            <div class="card shadow-sm">
                <div class="card-header bg-info text-white">
                    <h5 class="mb-0">Tóm tắt đơn hàng</h5>
                </div>
                <div class="card-body">
                    @{
                        var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(Context.Session, "cart") ?? new List<CartItem>();
                        var total = cart.Sum(c => c.Quantity * c.Price);
                    }
                    @foreach (var item in cart)
                    {
                        <div class="d-flex justify-content-between mb-2">
                            <span>@item.ProductName x @item.Quantity</span>
                            <strong>@((item.Quantity * item.Price).ToString("N0")) đ</strong>
                        </div>
                    }
                    <hr>
                    <div class="d-flex justify-content-between">
                        <h5>Tổng cộng:</h5>
                        <h5 class="text-danger">@total.ToString("N0") đ</h5>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

---

## 📊 Test Data Available

### Login Credentials:
```
Admin:
- Email: admin@bookstore.com
- Password: Admin@123

Customers:
- Email: customer1@gmail.com
- Password: Customer@123
```

### Database Contains:
- ✅ 5 users (1 admin + 4 customers)
- ✅ 8 categories
- ✅ 32 products (Vietnamese books)
- ✅ 50 demo orders (for reports)
- ✅ ~100 order details

---

## 🧪 Testing Checklist

After completing remaining tasks:

### User Authentication
- [ ] Register new user with password
- [ ] Login with hashed password
- [ ] Logout
- [ ] Try wrong password (should fail)

### Shopping Flow
- [ ] Browse products
- [ ] Add to cart
- [ ] Update cart quantities
- [ ] Proceed to checkout
- [ ] Fill shipping form
- [ ] Complete order
- [ ] View order confirmation

### Admin Functions
- [ ] Login as admin
- [ ] View dashboard with statistics
- [ ] Manage products (CRUD)
- [ ] View orders
- [ ] Update order status

### Reports & Dashboard
- [ ] Revenue by month
- [ ] Top selling products
- [ ] Order statistics
- [ ] Customer list

---

## 📝 Implementation Order

**Today (3-4 hours total)**:

1. ⏰ **30 min**: Update AccountController with password hashing
2. ⏰ **45 min**: Implement checkout flow with validation
3. ⏰ **30 min**: Add toast notifications
4. ⏰ **30 min**: Add cart badge & improve navbar
5. ⏰ **45 min**: Improve checkout UI
6. ⏰ **30 min**: Test entire flow

---

## 🚀 Ready to Continue?

Tôi có thể:
1. ✅ Update AccountController ngay bây giờ
2. ✅ Implement checkout flow
3. ✅ Improve UI components
4. ✅ Tất cả cùng lúc

Bạn muốn tôi làm gì tiếp theo? 🎯
