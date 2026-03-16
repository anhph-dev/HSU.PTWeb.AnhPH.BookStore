using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HSU.PTWeb.AnhPH.BookStore.Data;
using HSU.PTWeb.AnhPH.BookStore.Helpers;
using HSU.PTWeb.AnhPH.BookStore.Models;
using HSU.PTWeb.AnhPH.BookStore.ViewModels;
using Microsoft.EntityFrameworkCore;
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

        private async Task LoadAddressDataAsync()
        {
            var cities = await _context.Cities
                .Where(c => c.IsActive)
                .OrderBy(c => c.CityName)
                .Select(c => c.CityName)
                .ToListAsync();

            var wards = await _context.Wards
                .Where(w => w.IsActive && w.City.IsActive)
                .Select(w => new { w.City.CityName, w.WardName })
                .ToListAsync();

            var wardByCity = wards
                .GroupBy(x => x.CityName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.WardName).OrderBy(x => x).ToList());

            ViewBag.Cities = cities;
            ViewBag.WardByCity = wardByCity;
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
            User dbUser = null;
            if (int.TryParse(userIdClaim, out var userId))
                dbUser = _context.Users.Find(userId);

            var model = new CheckoutViewModel
            {
                Email           = dbUser?.Email ?? User.FindFirstValue(ClaimTypes.Email) ?? "",
                RecipientName   = dbUser?.FullName ?? User.FindFirstValue(ClaimTypes.Name) ?? "",
                PhoneNumber     = dbUser?.PhoneNumber ?? "",
                ShippingAddress = dbUser?.Address ?? "",
                City            = dbUser?.City ?? "",
                Ward            = dbUser?.Ward ?? "",
                PaymentMethod   = "COD"
            };

            ViewBag.Cart  = cart;
            ViewBag.Total = cart.Sum(c => c.Price * c.Quantity);
            await LoadAddressDataAsync();

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
                await LoadAddressDataAsync();
                return View(model);
            }

            // Validate stock availability
            var errors = new List<string>();
            foreach (var cartItem in cart)
            {
                var product = await _context.Products.FindAsync(cartItem.ProductId);
                if (product == null)
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
                TempData["ErrorMessage"] = string.Join("<br/>", errors);
                ViewBag.Cart = cart;
                ViewBag.Total = cart.Sum(c => c.Price * c.Quantity);
                await LoadAddressDataAsync();
                return View(model);
            }

            // Get user
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục";
                return RedirectToAction("Login", "Account");
            }

            // Create order
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(c => c.Price * c.Quantity),
                Status = "Pending",
                RecipientName = model.RecipientName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                ShippingAddress = model.ShippingAddress,
                City = model.City,
                Ward = model.Ward,
                Notes = model.Notes ?? string.Empty,
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = "Pending"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create order details and update stock
            foreach (var c in cart)
            {
                var detail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    UnitPrice = c.Price
                };
                _context.OrderDetails.Add(detail);

                // Deduct stock
                var product = await _context.Products.FindAsync(c.ProductId);
                if (product != null)
                {
                    product.Stock -= c.Quantity;
                    product.SoldCount += c.Quantity;
                }
            }

            await _context.SaveChangesAsync();

            // Clear cart
            HttpContext.Session.Remove(CartSessionKey);

            TempData["SuccessMessage"] = "Đặt hàng thành công! Cảm ơn bạn đã mua hàng.";
            return RedirectToAction("Confirm", new { id = order.OrderId });
        }

        // Trang xác nhận đơn hàng
        public async Task<IActionResult> Confirm(int id)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
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
                City = order.City,
                Ward = order.Ward,
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
