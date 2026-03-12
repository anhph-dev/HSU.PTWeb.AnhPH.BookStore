# 🔧 COMPREHENSIVE REFACTORING PLAN

## 📋 Table of Contents
1. [Security & Authentication](#security)
2. [Code Cleanup](#cleanup)
3. [Payment Flow](#payment)
4. [UI/UX Improvements](#ui)
5. [Best Practices](#best-practices)

---

## 🔐 1. Security & Authentication

### Current Issues:
- ❌ **Plain-text passwords** in database
- ❌ No password hashing (BCrypt/PBKDF2)
- ❌ No email validation
- ❌ No password strength requirements
- ❌ No CSRF protection on forms

### Action Items:

#### 1.1 Install Security Package
```bash
dotnet add package BCrypt.Net-Next
```

#### 1.2 Create Password Hasher Service
**File**: `Services/PasswordHasher.cs`
```csharp
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
```

#### 1.3 Update User Model
**File**: `Models/User.cs`
- Remove plain password field
- Add `PasswordHash` property
- Add validation attributes

#### 1.4 Update Registration
**File**: `Controllers/AccountController.cs`
- Hash password before saving
- Add email validation
- Add password strength validation
- Add duplicate email check

#### 1.5 Update Login
- Verify hashed password
- Add rate limiting (prevent brute force)
- Add "Remember Me" functionality

---

## 🧹 2. Code Cleanup

### 2.1 Remove Debug Code

**Files to Clean:**
- `Data/DbInitializer.cs` - Remove console logging
- `Program.cs` - Remove debug try-catch blocks
- All Controllers - Remove console output

### 2.2 Remove Unused Files

**Files to Delete:**
```
- Views/Admin/CreateProduct.cshtml (old, use Areas version)
- Views/Admin/EditProduct.cshtml (old)
- Views/Admin/Products.cshtml (old)
- Views/Cart/Checkout.cshtml (duplicate)
- All temporary .ps1 scripts (keep only production ones)
- All temporary .sql scripts
- wwwroot/test-images.html (debug page)
```

### 2.3 Consolidate ViewModels

**Current Issues:**
- Mixed ViewModels and Models
- No validation attributes

**Action:**
- Create `ViewModels/` folder
- Move all ViewModels there
- Add comprehensive validation

### 2.4 Remove GUID Image Files

**Files to Delete:**
```
wwwroot/images/products/1cd0d052-0ac1-45a8-b7c3-7aa3aafba412.jpeg
wwwroot/images/products/fd9c4da5-0ddf-4be6-a22d-9d39175a69f2.jpeg
```

---

## 💳 3. Payment & Checkout Flow

### Current Issues:
- ❌ No payment validation
- ❌ No shipping address validation
- ❌ No order confirmation email
- ❌ Stock not properly deducted

### 3.1 Shipping Address Model

**File**: `Models/ShippingAddress.cs`
```csharp
public class ShippingAddress
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    public string FullName { get; set; }

    [Required, Phone]
    public string PhoneNumber { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Address { get; set; }

    [Required]
    public string City { get; set; }

    [Required]
    public string District { get; set; }

    public string Ward { get; set; }
    public string Notes { get; set; }
}
```

### 3.2 Order ViewModel

**File**: `ViewModels/CheckoutViewModel.cs`
```csharp
public class CheckoutViewModel
{
    public ShippingAddress ShippingAddress { get; set; }
    public List<CartItem> CartItems { get; set; }
    public decimal TotalAmount { get; set; }
    public string PaymentMethod { get; set; }
}
```

### 3.3 Payment Methods

Support:
- ✅ COD (Cash on Delivery)
- ✅ Bank Transfer
- 🔜 VNPay integration (future)
- 🔜 Momo integration (future)

### 3.4 Order Processing Flow

```
1. Cart → 2. Checkout Form → 3. Review Order → 4. Confirm → 5. Success
   ↓             ↓                  ↓              ↓           ↓
 Session    Validation        Calculate Total   Save DB    Email
```

---

## 🎨 4. UI/UX Improvements

### 4.1 Consistent Layout

**File**: `Views/Shared/_Layout.cshtml`

**Changes:**
- ✅ Modern navbar with dropdown
- ✅ Shopping cart badge (live count)
- ✅ User profile dropdown
- ✅ Responsive design
- ✅ Toast notifications

### 4.2 Product Card Design

**Improvements:**
- Better image aspect ratio (3:4)
- Hover effects
- Rating stars display
- Stock indicators
- Discount badges

### 4.3 Checkout UI

**Improvements:**
- Step-by-step wizard
- Address autocomplete
- Order summary sidebar
- Payment method selection with icons
- Loading states

### 4.4 Admin Dashboard

**Improvements:**
- Statistics cards
- Charts (revenue, orders)
- Recent orders table
- Quick actions

### 4.5 Responsive Design

- Mobile-first approach
- Hamburger menu on mobile
- Touch-friendly buttons
- Optimized images

---

## ✅ 5. Best Practices Implementation

### 5.1 Dependency Injection

**Services to Register:**
```csharp
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IOrderService, OrderService>();
```

### 5.2 Repository Pattern

**Create:**
- `IProductRepository`
- `IOrderRepository`
- `IUserRepository`

### 5.3 Error Handling

**Global Error Handler:**
```csharp
app.UseExceptionHandler("/Error");
app.UseStatusCodePagesWithReExecute("/Error/{0}");
```

### 5.4 Logging

**Add Logging:**
```csharp
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
    config.AddEventLog();
});
```

### 5.5 Configuration

**appsettings.json:**
```json
{
  "Email": {
    "SmtpServer": "",
    "SmtpPort": 587,
    "Username": "",
    "Password": ""
  },
  "Payment": {
    "VNPay": {
      "TmnCode": "",
      "HashSecret": ""
    }
  }
}
```

---

## 📊 6. Database Cleanup

### 6.1 Migration Strategy

**Create new migration:**
```bash
dotnet ef migrations add AddPasswordHash
dotnet ef migrations add AddShippingAddressFields
```

### 6.2 Data Migration

**Migrate existing passwords:**
```sql
-- Warning: This will invalidate all existing passwords
-- Users will need to reset passwords or re-register
UPDATE Users SET PasswordHash = NULL, Password = NULL;
```

---

## 🧪 7. Testing

### 7.1 Manual Testing Checklist

- [ ] User Registration with validation
- [ ] User Login with hashed password
- [ ] Product browsing & filtering
- [ ] Add to cart functionality
- [ ] Checkout process (full flow)
- [ ] Order confirmation
- [ ] Admin: Create/Edit/Delete products
- [ ] Admin: View orders
- [ ] Admin: Update order status
- [ ] Responsive design on mobile

### 7.2 Test Data

**Create test accounts:**
```sql
-- Admin: admin@bookstore.com / Admin@123
-- Customer: customer@test.com / Customer@123
```

---

## 📁 8. File Organization

### Recommended Structure:
```
HSU.PTWeb.AnhPH.BookStore/
├── Areas/
│   └── Admin/
│       ├── Controllers/
│       ├── Views/
│       └── ViewModels/
├── Controllers/
├── Data/
│   ├── AppDbContext.cs
│   └── DbInitializer.cs
├── Models/
│   ├── Domain/          # Entities
│   └── ViewModels/      # ViewModels
├── Services/
│   ├── IPasswordHasher.cs
│   ├── PasswordHasher.cs
│   ├── IEmailService.cs
│   └── EmailService.cs
├── Helpers/
│   └── SessionHelper.cs
├── Views/
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── images/
└── appsettings.json
```

---

## 🚀 9. Implementation Order

### Phase 1: Security (Priority: HIGH)
1. Install BCrypt package
2. Create PasswordHasher service
3. Update User model
4. Update registration/login
5. Migrate existing users

**Time: 2-3 hours**

### Phase 2: Code Cleanup (Priority: HIGH)
1. Remove debug code
2. Delete unused files
3. Organize ViewModels
4. Clean up images folder

**Time: 1-2 hours**

### Phase 3: Checkout Flow (Priority: MEDIUM)
1. Create ShippingAddress model
2. Update checkout controller
3. Add payment methods
4. Implement order confirmation

**Time: 3-4 hours**

### Phase 4: UI Improvements (Priority: MEDIUM)
1. Update _Layout
2. Improve product cards
3. Enhance checkout UI
4. Responsive design fixes

**Time: 2-3 hours**

### Phase 5: Best Practices (Priority: LOW)
1. Add repositories
2. Global error handling
3. Logging
4. Email service

**Time: 2-3 hours**

---

## ✅ Total Estimated Time: 10-15 hours

---

## 🎯 Quick Wins (Can do now, <30 min each)

1. ✅ Remove console.WriteLine() debug statements
2. ✅ Delete unused files
3. ✅ Update ImageUrl in database (already done)
4. ✅ Add [ValidateAntiForgeryToken] to POST actions
5. ✅ Add loading spinners on buttons
6. ✅ Fix mobile responsive issues
7. ✅ Add meta tags (SEO)

---

## 📝 Notes

- Keep existing functionality working during refactor
- Test after each phase
- Commit to Git after each major change
- Document breaking changes
- Keep backward compatibility where possible

---

## 🔗 References

- ASP.NET Core Security: https://docs.microsoft.com/aspnet/core/security
- BCrypt.Net: https://github.com/BcryptNet/bcrypt.net
- Bootstrap 5: https://getbootstrap.com/docs/5.0
- Entity Framework Core: https://docs.microsoft.com/ef/core

---

**Last Updated**: 2026-03-12
**Status**: 🟡 In Progress
