using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HSU.PTWeb.AnhPH.BookStore.Data;
using HSU.PTWeb.AnhPH.BookStore.Helpers;
using HSU.PTWeb.AnhPH.BookStore.Models;
using HSU.PTWeb.AnhPH.BookStore.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace HSU.PTWeb.AnhPH.BookStore.Controllers
{
    // Controller xử lý đặt hàng (yêu cầu đăng nhập)
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        // Khóa session để lưu giỏ hàng (phù hợp với CartController)
        private const string CartSessionKey = "cart";

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        private async Task LoadCheckoutAddressDataAsync()
        {
            ViewBag.Cities = await _context.Cities
                .Where(c => c.IsActive)
                .OrderBy(c => c.CityName)
                .Select(c => new SelectListItem
                {
                    Value = c.CityId.ToString(),
                    Text = c.CityName
                })
                .ToListAsync();

            ViewBag.Wards = Enumerable.Empty<SelectListItem>();
        }

        [HttpGet]
        public async Task<IActionResult> GetWardsByCity(int cityId)
        {
            var wards = await _context.Wards
                .Where(w => w.CityId == cityId && w.IsActive)
                .OrderBy(w => w.WardName)
                .Select(w => new
                {
                    wardId = w.WardId,
                    wardName = w.WardName
                })
                .ToListAsync();

            return Json(wards);
        }

        // Hiển thị trang Checkout - tự động điền thông tin từ profile user
        public async Task<IActionResult> Checkout()
        {
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, CartSessionKey) ?? new List<CartItem>();

            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            // Lấy thông tin user để điền sẵn vào form
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            User? dbUser = null;
            if (int.TryParse(userIdClaim, out var userId))
            {
                dbUser = await _context.Users
                    .Include(u => u.City)
                    .Include(u => u.Ward)
                    .FirstOrDefaultAsync(u => u.UserId == userId);
            }

            var model = new CheckoutViewModel
            {
                Email = dbUser?.Email ?? User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                RecipientName = dbUser?.FullName ?? User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                PhoneNumber = dbUser?.PhoneNumber ?? string.Empty,
                ShippingAddress = dbUser?.Address ?? string.Empty,
                CityId = dbUser?.CityId,
                WardId = dbUser?.WardId,
                City = dbUser?.City?.CityName,
                Ward = dbUser?.Ward?.WardName,
                PaymentMethod = "COD"
            };

            ViewBag.Cart = cart;
            ViewBag.Total = cart.Sum(c => c.Price * c.Quantity);
            await LoadCheckoutAddressDataAsync();

            return View(model);
        }

        // Xử lý tạo Order và OrderDetails từ giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = SessionHelper.GetObjectFromJson<List<CartItem>>(HttpContext.Session, CartSessionKey) ?? new List<CartItem>();

            if (!cart.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Cart = cart;
                ViewBag.Total = cart.Sum(c => c.Price * c.Quantity);
                await LoadCheckoutAddressDataAsync();
                return View(model);
            }

            // Validate city/ward
            var selectedCity = await _context.Cities
                .FirstOrDefaultAsync(c => c.CityId == model.CityId && c.IsActive);
            if (selectedCity == null)
            {
                ModelState.AddModelError(nameof(model.CityId), "Tỉnh/Thành phố không hợp lệ");
                ViewBag.Cart = cart;
                ViewBag.Total = cart.Sum(c => c.Price * c.Quantity);
                await LoadCheckoutAddressDataAsync();
                return View(model);
            }

            var selectedWard = await _context.Wards
                .FirstOrDefaultAsync(w => w.WardId == model.WardId && w.IsActive);
            if (selectedWard == null || selectedWard.CityId != selectedCity.CityId)
            {
                ModelState.AddModelError(nameof(model.WardId), "Phường/Xã không hợp lệ");
                ViewBag.Cart = cart;
                ViewBag.Total = cart.Sum(c => c.Price * c.Quantity);
                await LoadCheckoutAddressDataAsync();
                return View(model);
            }

            // Get user
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục";
                return RedirectToAction("Login", "Account");
            }

            await using var tx = await _context.Database.BeginTransactionAsync();
            var createdOrderId = 0;
            try
            {
                var productIds = cart.Select(x => x.ProductId).Distinct().ToList();
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.ProductId))
                    .ToDictionaryAsync(p => p.ProductId);

                var errors = new List<string>();
                foreach (var cartItem in cart)
                {
                    if (!products.TryGetValue(cartItem.ProductId, out var product))
                    {
                        errors.Add($"Sản phẩm '{cartItem.ProductName}' không tồn tại.");
                    }
                    else if (product.Stock < cartItem.Quantity)
                    {
                        errors.Add($"Sản phẩm '{cartItem.ProductName}' chỉ còn {product.Stock} trong kho (bạn đặt {cartItem.Quantity}).");
                    }
                }

                if (errors.Any())
                {
                    await tx.RollbackAsync();
                    TempData["ErrorMessage"] = string.Join("<br/>", errors);
                    ViewBag.Cart = cart;
                    ViewBag.Total = cart.Sum(c => c.Price * c.Quantity);
                    await LoadCheckoutAddressDataAsync();
                    return View(model);
                }

                var order = new Order
                {
                    UserId = userId,
                    AppUserId = null,
                    Channel = "Online",
                    CityId = selectedCity.CityId,
                    WardId = selectedWard.WardId,
                    OrderDate = DateTime.Now,
                    TotalAmount = cart.Sum(c => c.Price * c.Quantity),
                    Status = "Pending",
                    RecipientName = model.RecipientName,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    ShippingAddress = model.ShippingAddress,
                    Notes = model.Notes ?? string.Empty,
                    PaymentMethod = model.PaymentMethod,
                    PaymentStatus = "Pending"
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                createdOrderId = order.OrderId;

                foreach (var c in cart)
                {
                    _context.OrderDetails.Add(new OrderDetail
                    {
                        OrderId = order.OrderId,
                        ProductId = c.ProductId,
                        Quantity = c.Quantity,
                        UnitPrice = c.Price
                    });

                    var product = products[c.ProductId];
                    product.Stock -= c.Quantity;
                    product.SoldCount += c.Quantity;
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                TempData["ErrorMessage"] = "Không thể tạo đơn hàng. Vui lòng thử lại.";
                ViewBag.Cart = cart;
                ViewBag.Total = cart.Sum(c => c.Price * c.Quantity);
                await LoadCheckoutAddressDataAsync();
                return View(model);
            }

            // Clear cart
            HttpContext.Session.Remove(CartSessionKey);

            TempData["SuccessMessage"] = "Đặt hàng thành công! Cảm ơn bạn đã mua hàng.";
            return RedirectToAction("Confirm", new { id = createdOrderId });
        }

        // Trang xác nhận đơn hàng
        public async Task<IActionResult> Confirm(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.CityNavigation)
                .Include(o => o.WardNavigation)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (order == null) return NotFound();

            return View(order);
        }

        // Hiển thị danh sách đơn hàng của user hiện tại với phân trang và lọc
        public async Task<IActionResult> Index(string status, DateTime? fromDate, DateTime? toDate, int page = 1)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var query = _context.Orders.Where(o => o.UserId == userId);

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
                ViewData["Status"] = status;
            }

            if (fromDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= fromDate.Value.Date);
                ViewData["FromDate"] = fromDate.Value.ToString("yyyy-MM-dd");
            }

            if (toDate.HasValue)
            {
                var end = toDate.Value.Date.AddDays(1);
                query = query.Where(o => o.OrderDate < end);
                ViewData["ToDate"] = toDate.Value.ToString("yyyy-MM-dd");
            }

            var pageSize = 10;
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            page = Math.Max(1, Math.Min(page, totalPages == 0 ? 1 : totalPages));

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrderListItemViewModel
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status
                })
                .ToListAsync();

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;

            return View(orders);
        }

        // Hiển thị chi tiết đơn hàng cho user hiện tại
        public async Task<IActionResult> Details(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.CityNavigation)
                .Include(o => o.WardNavigation)
                .FirstOrDefaultAsync(o => o.OrderId == id && o.UserId == userId);

            if (order == null) return NotFound();

            var vm = new OrderDetailsViewModel
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                RecipientName = order.RecipientName,
                PhoneNumber = order.PhoneNumber,
                Email = order.Email,
                ShippingAddress = order.ShippingAddress,
                City = order.CityNavigation?.CityName,
                Ward = order.WardNavigation?.WardName,
                Notes = order.Notes,
                PaymentMethod = order.PaymentMethod,
                PaymentStatus = order.PaymentStatus,
                Items = order.OrderDetails.Select(d => new OrderDetailItemViewModel
                {
                    ProductName = d.Product?.ProductName ?? "-",
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice
                }).ToList()
            };

            return View(vm);
        }
    }
}
