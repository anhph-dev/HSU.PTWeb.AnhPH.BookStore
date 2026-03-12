# 🎯 REFACTORING NEXT STEPS

## ✅ Completed (Just Now)

1. ✅ Removed debug console logging from `DbInitializer.cs`
2. ✅ Cleaned up `Program.cs` try-catch blocks
3. ✅ Deleted unused files:
   - `wwwroot/test-images.html`
   - `Views/Admin/CreateProduct.cshtml` (duplicate)
   - `Views/Admin/EditProduct.cshtml` (duplicate)
   - `Views/Admin/Products.cshtml` (duplicate)

---

## 🔥 HIGH PRIORITY - Do Next

### 1. Security Implementation (2-3 hours)

#### Install BCrypt Package
```bash
dotnet add package BCrypt.Net-Next
```

#### Create Password Service
Need to create:
- `Services/IPasswordHasher.cs`
- `Services/PasswordHasher.cs`

#### Update Models
- `Models/User.cs` - Add `PasswordHash` property
- Update database schema

#### Update Controllers
- `Controllers/AccountController.cs`:
  - Register: Hash password before save
  - Login: Verify hashed password
  - Add validation

---

### 2. Checkout Flow Improvements (3-4 hours)

#### Create ViewModels
- `ViewModels/CheckoutViewModel.cs`
- `ViewModels/ShippingAddressViewModel.cs`

#### Update Controllers
- `Controllers/OrderController.cs`:
  - Add validation
  - Improve order creation logic
  - Add stock deduction
  - Add order confirmation

#### Update Views
- `Views/Order/Checkout.cshtml`:
  - Add shipping address form
  - Add payment method selection
  - Add order summary
  - Add validation

---

### 3. UI/UX Improvements (2-3 hours)

#### Layout Improvements
- `Views/Shared/_Layout.cshtml`:
  - Add cart badge with live count
  - Improve navbar dropdown
  - Add user menu
  - Make fully responsive

#### Product Cards
- `Views/Product/Index.cshtml`:
  - Better hover effects
  - Improved image display
  - Better badge positioning

#### Admin Dashboard
- `Areas/Admin/Views/Dashboard/Index.cshtml`:
  - Add statistics cards
  - Add charts
  - Recent orders table

---

## 📝 DETAILED ACTION ITEMS

### Action 1: Install & Setup BCrypt

**Command:**
```bash
dotnet add package BCrypt.Net-Next
```

**Create File**: `Services/IPasswordHasher.cs`
```csharp
namespace HSU.PTWeb.AnhPH.BookStore.Services
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string hashedPassword);
    }
}
```

**Create File**: `Services/PasswordHasher.cs`
```csharp
namespace HSU.PTWeb.AnhPH.BookStore.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }
    }
}
```

**Register in Program.cs:**
```csharp
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
```

---

### Action 2: Update User Model

**File**: `Models/User.cs`

**Changes:**
1. Add `[StringLength(255)]` to `PasswordHash`
2. Mark `Password` as `[Obsolete]` (keep for migration)
3. Add email validation
4. Add phone validation

**Migration:**
```bash
dotnet ef migrations add AddPasswordHashField
dotnet ef database update
```

---

### Action 3: Update AccountController

**File**: `Controllers/AccountController.cs`

**Register Action Changes:**
1. Inject `IPasswordHasher`
2. Hash password before saving
3. Add duplicate email check
4. Add ModelState validation
5. Add success message

**Login Action Changes:**
1. Find user by email
2. Verify hashed password
3. Add rate limiting (prevent brute force)
4. Add "Remember Me" option

---

### Action 4: Add CSRF Protection

**All POST Actions:**
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult SomeAction(Model model)
{
    // ...
}
```

**In Views:**
```html
<form method="post">
    @Html.AntiForgeryToken()
    <!-- form fields -->
</form>
```

---

### Action 5: Improve Checkout Flow

**Create**: `ViewModels/CheckoutViewModel.cs`
```csharp
public class CheckoutViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [Display(Name = "Họ và tên")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [Display(Name = "Số điện thoại")]
    public string PhoneNumber { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
    [Display(Name = "Địa chỉ giao hàng")]
    public string ShippingAddress { get; set; }

    [Display(Name = "Ghi chú")]
    public string Notes { get; set; }

    [Required]
    [Display(Name = "Phương thức thanh toán")]
    public string PaymentMethod { get; set; } // COD, BankTransfer

    public List<CartItem> CartItems { get; set; }
    public decimal TotalAmount { get; set; }
}
```

---

### Action 6: Update Order Model

**File**: `Models/Order.cs`

**Add Fields:**
```csharp
public string FullName { get; set; }
public string PhoneNumber { get; set; }
public string Email { get; set; }
public string ShippingAddress { get; set; }
public string PaymentMethod { get; set; } // COD, BankTransfer, VNPay
public string PaymentStatus { get; set; } // Pending, Paid, Failed
public DateTime? PaidAt { get; set; }
```

**Migration:**
```bash
dotnet ef migrations add AddOrderShippingFields
dotnet ef database update
```

---

### Action 7: UI Improvements

#### Add Toast Notifications

**File**: `Views/Shared/_Layout.cshtml`

**Add before `</body>`:**
```html
<!-- Toast Container -->
<div class="toast-container position-fixed top-0 end-0 p-3">
    @if (TempData["SuccessMessage"] != null)
    {
        <div class="toast show" role="alert">
            <div class="toast-header bg-success text-white">
                <strong class="me-auto">✅ Thành công</strong>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
            </div>
            <div class="toast-body">
                @TempData["SuccessMessage"]
            </div>
        </div>
    }
    @if (TempData["ErrorMessage"] != null)
    {
        <div class="toast show" role="alert">
            <div class="toast-header bg-danger text-white">
                <strong class="me-auto">❌ Lỗi</strong>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast"></button>
            </div>
            <div class="toast-body">
                @TempData["ErrorMessage"]
            </div>
        </div>
    }
</div>

<script>
    // Auto-hide toasts after 5 seconds
    setTimeout(() => {
        document.querySelectorAll('.toast').forEach(toast => {
            toast.classList.remove('show');
        });
    }, 5000);
</script>
```

#### Add Cart Badge

**In Navbar:**
```html
<li class="nav-item">
    <a class="nav-link position-relative" asp-controller="Cart" asp-action="Index">
        <i class="bi bi-cart3"></i> Giỏ hàng
        @{
            var cartCount = /* Get cart count from session */;
            if (cartCount > 0)
            {
                <span class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">
                    @cartCount
                </span>
            }
        }
    </a>
</li>
```

---

## 🧪 Testing Checklist

After each change, test:

### Security
- [ ] Register new user with password
- [ ] Login with hashed password
- [ ] Try wrong password (should fail)
- [ ] Try duplicate email (should fail)
- [ ] Password validation works

### Checkout
- [ ] Add items to cart
- [ ] Go to checkout
- [ ] Fill shipping form with validation
- [ ] Select payment method
- [ ] Confirm order
- [ ] Check order in database
- [ ] Check stock is deducted

### UI
- [ ] Toast notifications appear
- [ ] Cart badge shows count
- [ ] Responsive on mobile
- [ ] All forms have CSRF tokens
- [ ] Loading states on buttons

---

## 🚀 Implementation Priority

**TODAY (Most Important):**
1. ✅ Code cleanup (Done!)
2. 🔐 Password hashing (2-3 hours)
3. 📝 Checkout validation (1-2 hours)

**THIS WEEK:**
4. 🎨 UI improvements (2-3 hours)
5. ✅ CSRF protection (30 min)
6. 📧 Email confirmation (2 hours)

**NICE TO HAVE:**
7. 📊 Admin dashboard charts
8. 🔍 Advanced search/filtering
9. ⭐ Product reviews
10. 💳 Payment gateway integration

---

## 📦 Packages to Install

```bash
# Security
dotnet add package BCrypt.Net-Next

# Email (future)
dotnet add package MailKit

# Charts (future)
dotnet add package Chart.js

# PDF generation (future)
dotnet add package iTextSharp
```

---

## 🎓 Learning Resources

- BCrypt.Net: https://github.com/BcryptNet/bcrypt.net
- ASP.NET Security: https://docs.microsoft.com/aspnet/core/security
- Validation: https://docs.microsoft.com/aspnet/core/mvc/models/validation
- Bootstrap 5: https://getbootstrap.com/docs/5.0

---

**Ready to implement?** Start with Password Hashing! 🔐

Would you like me to:
1. Implement password hashing now?
2. Work on checkout improvements?
3. Focus on UI/UX?
4. All of the above step-by-step?

Let me know and I'll guide you through! 🚀
