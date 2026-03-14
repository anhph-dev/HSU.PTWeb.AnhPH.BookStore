using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HSU.PTWeb.AnhPH.BookStore.Data;
using HSU.PTWeb.AnhPH.BookStore.Models;

namespace HSU.PTWeb.AnhPH.BookStore.Areas.Admin.Controllers
{
    // Chỉ Admin mới có quyền truy cập
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;
        private const int PageSize = 10;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Users - Danh sách tất cả người dùng với tìm kiếm & lọc
        public async Task<IActionResult> Index(string search, string role, string status, int page = 1)
        {
            var query = _context.Users.AsQueryable();

            // Tìm kiếm theo tên hoặc email
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
                ViewData["Search"] = search;
            }

            // Lọc theo role
            if (!string.IsNullOrWhiteSpace(role))
            {
                query = query.Where(u => u.Role == role);
                ViewData["Role"] = role;
            }

            // Lọc theo trạng thái khoá
            if (status == "locked")
                query = query.Where(u => u.IsLocked);
            else if (status == "active")
                query = query.Where(u => !u.IsLocked);
            ViewData["Status"] = status;

            var total = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(total / (double)PageSize);
            page = Math.Max(1, Math.Min(page, totalPages == 0 ? 1 : totalPages));

            var users = await query
                .OrderByDescending(u => u.CreatedDate)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(u => new UserListItemViewModel
                {
                    UserId      = u.UserId,
                    FullName    = u.FullName,
                    Email       = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Role        = u.Role,
                    IsLocked    = u.IsLocked,
                    CreatedDate = u.CreatedDate,
                    OrderCount  = u.Orders.Count()
                })
                .ToListAsync();

            ViewData["Page"]       = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["Total"]      = total;

            return View(users);
        }

        // GET: Admin/Users/Details/5 - Xem thông tin chi tiết 1 user
        public async Task<IActionResult> Details(int id)
        {
            var user = await _context.Users
                .Include(u => u.Orders)
                    .ThenInclude(o => o.OrderDetails)
                        .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Admin/Users/Lock/5 - Khoá tài khoản user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Lock(int id, int days = 0)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Không cho phép khoá chính mình
            var currentUserId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);
            if (user.UserId == currentUserId)
            {
                TempData["ErrorMessage"] = "Không thể khoá chính tài khoản của mình!";
                return RedirectToAction(nameof(Index));
            }

            user.IsLocked   = true;
            // Khoá vĩnh viễn nếu days = 0, ngược lại khoá tạm thời
            user.LockedUntil = days > 0 ? DateTime.Now.AddDays(days) : null;

            await _context.SaveChangesAsync();

            var msg = days > 0 ? $"{days} ngày" : "vĩnh viễn";
            TempData["SuccessMessage"] = $"Đã khoá tài khoản {user.Email} ({msg})";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Users/Unlock/5 - Mở khoá tài khoản user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unlock(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsLocked    = false;
            user.LockedUntil = null;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã mở khoá tài khoản {user.Email}";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Users/ChangeRole/5 - Thay đổi role của user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(int id, string newRole)
        {
            if (newRole != "Admin" && newRole != "Customer")
            {
                TempData["ErrorMessage"] = "Role không hợp lệ!";
                return RedirectToAction(nameof(Index));
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            // Không cho phép thay đổi role của chính mình
            var currentUserId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);
            if (user.UserId == currentUserId)
            {
                TempData["ErrorMessage"] = "Không thể thay đổi role của chính mình!";
                return RedirectToAction(nameof(Details), new { id });
            }

            var oldRole  = user.Role;
            user.Role    = newRole;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã đổi role {user.Email} từ {oldRole} → {newRole}";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    // ViewModel cho danh sách user
    public class UserListItemViewModel
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedDate { get; set; }
        public int OrderCount { get; set; }
    }
}
