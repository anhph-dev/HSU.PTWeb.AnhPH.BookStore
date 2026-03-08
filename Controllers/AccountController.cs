using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HSU.PTWeb.AnhPH.BookStore.Data;
using HSU.PTWeb.AnhPH.BookStore.Models;
using HSU.PTWeb.AnhPH.BookStore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HSU.PTWeb.AnhPH.BookStore.Controllers
{
    // Controller xử lý đăng ký/đăng nhập/đăng xuất
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // Hiển thị form đăng ký
        public IActionResult Register()
        {
            return View();
        }

        // Xử lý đăng ký người dùng mới
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Kiểm tra email (user name) đã tồn tại
            if (await _context.Users.AnyAsync(u => u.Email == model.UserName))
            {
                ModelState.AddModelError("", "Email/ tài khoản đã tồn tại");
                return View(model);
            }

            var user = new User
            {
                Email = model.UserName,
                Password = model.Password, // Lưu plain-text cho ví dụ; trong thực tế cần hash
                FullName = model.FullName,
                Role = "Customer",
                CreatedDate = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        // Hiển thị form đăng nhập
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Xử lý đăng nhập, tạo cookie với claims (Name, Email, Role)
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            // Tìm user theo email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.UserName && u.Password == model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                return View(model);
            }

            // Tạo claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName ?? user.Email),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "Customer"),
                // Lưu UserId vào claim để dễ lấy sau này
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Product");
        }

        // Đăng xuất người dùng
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Product");
        }

        // Trang khi bị denied
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Xem thông tin cá nhân (yêu cầu đăng nhập)
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            return View(user);
        }

        // Cập nhật thông tin cá nhân
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(User model)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            // Chỉ cho phép cập nhật tên và mật khẩu
            user.FullName = model.FullName;
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                user.Password = model.Password; // Demo: plain-text, thực tế cần hash
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Profile");
        }
    }
}
