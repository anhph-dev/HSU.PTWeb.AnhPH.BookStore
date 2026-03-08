using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HSU.PTWeb.AnhPH.BookStore.Data;
using HSU.PTWeb.AnhPH.BookStore.Helpers;
using HSU.PTWeb.AnhPH.BookStore.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HSU.PTWeb.AnhPH.BookStore.ViewModels;

namespace HSU.PTWeb.AnhPH.BookStore.Controllers
{
    // Controller xử lý đặt hàng (yêu cầu đăng nhập)
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        // Khóa session để lưu giỏ hàng (phù hợp với CartController)
        private const string CartSessionKey = "cart";
        private const int DefaultPageSize = 10;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        // Hiển thị trang Checkout (kiểm tra giỏ hàng)
        public IActionResult Checkout()
        {
            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, CartSessionKey) ?? new List<CartItem>();
            return View(cart);
        }

        // Xử lý tạo Order và OrderDetails từ giỏ hàng
        [HttpPost]
        public async Task<IActionResult> PlaceOrder()
        {
            var cart = SessionHelper.GetObject<List<CartItem>>(HttpContext.Session, CartSessionKey) ?? new List<CartItem>();
            if (!cart.Any()) return RedirectToAction("Index", "Cart");

            // Lấy thông tin user từ claims
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            var order = new Order
            {
                UserId = user?.UserId ?? 0,
                OrderDate = DateTime.UtcNow,
                TotalAmount = cart.Sum(c => c.Price * c.Quantity),
                Status = "Mới"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Tạo các OrderDetail
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
            }

            await _context.SaveChangesAsync();

            // Clear session cart
            SessionHelper.SetObject(HttpContext.Session, CartSessionKey, new List<CartItem>());

            return RedirectToAction("Confirm", new { id = order.OrderId });
        }

        // Trang xác nhận đơn hàng
        public async Task<IActionResult> Confirm(int id)
        {
            // Load order cùng với chi tiết và thông tin product để hiển thị đầy đủ
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null) return NotFound();
            return View(order);
        }

        // Hiển thị danh sách đơn hàng của user hiện tại với phân trang và lọc
        public async Task<IActionResult> Index(string status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = DefaultPageSize)
        {
            // Lấy userId từ claim NameIdentifier
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            // Khởi tạo truy vấn lọc theo user
            var query = _context.Orders
                .Where(o => o.UserId == userId)
                .AsQueryable();

            // Lọc theo trạng thái nếu có
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
                ViewData["Status"] = status;
            }

            // Lọc theo khoảng ngày nếu có
            if (fromDate.HasValue)
            {
                query = query.Where(o => o.OrderDate >= fromDate.Value.Date);
                ViewData["FromDate"] = fromDate.Value.ToString("yyyy-MM-dd");
            }
            if (toDate.HasValue)
            {
                // include toàn bộ ngày toDate
                var end = toDate.Value.Date.AddDays(1);
                query = query.Where(o => o.OrderDate < end);
                ViewData["ToDate"] = toDate.Value.ToString("yyyy-MM-dd");
            }

            // Tổng số mục để phân trang
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            // Lấy trang hiện tại
            page = Math.Max(1, page);
            if (page > totalPages) page = totalPages == 0 ? 1 : totalPages;

            // Truy vấn dữ liệu với phân trang, sắp theo ngày giảm dần
            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map sang viewmodel
            var vm = orders.Select(o => new OrderListItemViewModel
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                Status = o.Status
            }).ToList();

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;

            return View(vm);
        }

        // Hiển thị chi tiết đơn hàng cho user hiện tại
        public async Task<IActionResult> Details(int id)
        {
            // Lấy userId từ claim
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            // Lấy order kèm chi tiết và product
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            // Kiểm tra quyền: order phải thuộc user hiện tại
            if (order.UserId != userId) return Unauthorized();

            // Map sang viewmodel
            var vm = new OrderDetailsViewModel
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
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
