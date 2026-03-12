# 🎯 Admin Area Quick Reference

## 📍 URLs

| Feature | URL | Method | Auth |
|---------|-----|--------|------|
| Dashboard | `/Admin` | GET | Admin |
| Revenue Report | `/Admin/Dashboard/Revenue` | GET | Admin |
| List Products | `/Admin/Products` | GET | Admin |
| Create Product | `/Admin/Products/Create` | GET/POST | Admin |
| Edit Product | `/Admin/Products/Edit/{id}` | GET/POST | Admin |
| Delete Product | `/Admin/Products/Delete/{id}` | POST | Admin |
| List Orders | `/Admin/Orders` | GET | Admin |
| Order Details | `/Admin/Orders/Details/{id}` | GET | Admin |
| Update Order Status | `/Admin/Orders/UpdateStatus` | POST | Admin |

## 🎨 Views Location

```
Areas/Admin/Views/
├── Dashboard/
│   ├── Index.cshtml        (Dashboard overview)
│   └── Revenue.cshtml      (Revenue report with chart)
├── Products/
│   ├── Index.cshtml        (List with search/filter)
│   ├── Create.cshtml       (Create form)
│   └── Edit.cshtml         (Edit form)
└── Orders/
    ├── Index.cshtml        (List with search/filter)
    └── Details.cshtml      (Order details + status update)
```

## 🔧 Controllers

### DashboardController
- `Index()` - Statistics overview
- `Revenue(DateTime? date)` - Revenue report

### ProductsController
- `Index(search, categoryId, page)` - List products
- `Create()` - Show create form
- `Create(Product, IFormFile)` - Save product
- `Edit(int id)` - Show edit form
- `Edit(Product, IFormFile)` - Update product
- `Delete(int id)` - Delete product (POST)

### OrdersController
- `Index(search, status, fromDate, toDate, page)` - List orders
- `Details(int id)` - Show order details
- `UpdateStatus(orderId, status)` - Update order status (POST)

## 🔑 Key Features

### Dashboard
- ✅ Total products, orders, customers count
- ✅ Today's revenue
- ✅ 5 recent orders
- ✅ 5 low stock products (≤10)

### Products
- ✅ Pagination (10 items/page)
- ✅ Search by name
- ✅ Filter by category
- ✅ Image upload with preview
- ✅ Stock management
- ✅ Delete with confirmation

### Orders
- ✅ Pagination
- ✅ Search by customer name/email
- ✅ Filter by status (Mới, Đã vận chuyển, Đã thanh toán)
- ✅ Filter by date range
- ✅ Update status inline or in details page
- ✅ View order details with line items

### Revenue
- ✅ Select date
- ✅ Daily, monthly, yearly totals
- ✅ Chart.js line chart (7 days)
- ✅ VND formatting

## 💡 Code Examples

### Link to Admin page from _Layout
```razor
<a asp-area="Admin" asp-controller="Dashboard" asp-action="Index">Admin Dashboard</a>
```

### Link between Admin pages
```razor
<a asp-area="Admin" asp-controller="Products" asp-action="Index">Back to Products</a>
```

### Form in Admin area
```razor
<form asp-area="Admin" asp-controller="Products" asp-action="Create" method="post" enctype="multipart/form-data">
    @Html.AntiForgeryToken()
    <!-- form fields -->
</form>
```

### Post action with confirmation
```razor
<form asp-area="Admin" asp-controller="Products" asp-action="Delete" asp-route-id="@Model.ProductId" method="post" class="d-inline">
    @Html.AntiForgeryToken()
    <button type="submit" class="btn btn-danger" onclick="return confirm('Delete?')">
        Delete
    </button>
</form>
```

## 🚨 Common Issues

### 404 Not Found
- **Check**: Có đúng `[Area("Admin")]` trong controller không?
- **Check**: URL có `/Admin/` prefix không?
- **Check**: Route có được config trong `Program.cs` không?

### View not found
- **Check**: View có đúng path `Areas/Admin/Views/{Controller}/{Action}.cshtml` không?
- **Check**: Có `_ViewStart.cshtml` trong `Areas/Admin/Views/` không?

### Links not working
- **Check**: Có dùng `asp-area="Admin"` trong tag helpers không?
- **Fix**: Thêm `asp-area="Admin"` vào tất cả `<a>` và `<form>` tags

### Authorization failed
- **Check**: User có role "Admin" không?
- **Check**: Controller có `[Authorize(Roles = "Admin")]` không?

## 📱 Navigation

### From Public to Admin
```razor
@if (User.IsInRole("Admin"))
{
    <a asp-area="Admin" asp-controller="Dashboard" asp-action="Index">Go to Admin</a>
}
```

### From Admin to Public
```razor
<a asp-area="" asp-controller="Product" asp-action="Index">Back to Store</a>
```

Note: `asp-area=""` để trở về public area (no area).

## 🎓 Tips

1. **Always use `asp-area="Admin"`** in Admin views
2. **Use `@Html.AntiForgeryToken()`** in all POST forms
3. **Use TempData** for success/error messages
4. **Use POST + confirmation** for delete actions
5. **Add loading states** on submit buttons
6. **Validate file uploads** (size, extension)
7. **Check ModelState** before saving
8. **Use try-catch** for database operations
