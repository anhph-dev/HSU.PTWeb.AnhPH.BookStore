using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using HSU.PTWeb.AnhPH.BookStore.Data;
using HSU.PTWeb.AnhPH.BookStore.Models;
using HSU.PTWeb.AnhPH.BookStore.ViewModels;
using HSU.PTWeb.AnhPH.BookStore.Services;
using Microsoft.EntityFrameworkCore;

namespace HSU.PTWeb.AnhPH.BookStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public AccountController(AppDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        private async Task LoadAddressSelectDataAsync(string cityBagName, string wardJsonBagName)
        {
            var cityOptions = await _context.Cities
                .Where(c => c.IsActive)
                .OrderBy(c => c.CityName)
                .Select(c => new SelectListItem
                {
                    Value = c.CityId.ToString(),
                    Text = c.CityName
                })
                .ToListAsync();

            var wards = await _context.Wards
                .Where(w => w.IsActive && w.City.IsActive)
                .Select(w => new { w.CityId, w.WardId, w.WardName })
                .ToListAsync();

            var wardByCity = wards
                .GroupBy(x => x.CityId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderBy(x => x.WardName)
                        .Select(x => new { value = x.WardId, text = x.WardName })
                        .ToList());

            ViewData[cityBagName] = cityOptions;
            ViewData[wardJsonBagName] = System.Text.Json.JsonSerializer.Serialize(wardByCity);
        }

        // Hiển thị form đăng ký
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            await LoadAddressSelectDataAsync("RegisterCities", "RegisterWardByCityJson");
            return View();
        }

        // Xử lý đăng ký người dùng mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["ReturnUrl"] = returnUrl;
                await LoadAddressSelectDataAsync("RegisterCities", "RegisterWardByCityJson");
                return View(model);
            }

            // Kiểm tra email đã tồn tại chưa
            if (await _context.Users.AnyAsync(u => u.Email == model.UserName))
            {
                ModelState.AddModelError("", "Email đã được sử dụng, vui lòng chọn email khác");
                ViewData["ReturnUrl"] = returnUrl;
                await LoadAddressSelectDataAsync("RegisterCities", "RegisterWardByCityJson");
                return View(model);
            }

            var selectedCity = await _context.Cities
                .FirstOrDefaultAsync(c => c.CityId == model.CityId && c.IsActive);
            if (selectedCity == null)
            {
                ModelState.AddModelError(nameof(model.CityId), "Tỉnh/Thành phố không hợp lệ");
                ViewData["ReturnUrl"] = returnUrl;
                await LoadAddressSelectDataAsync("RegisterCities", "RegisterWardByCityJson");
                return View(model);
            }

            var selectedWard = await _context.Wards
                .FirstOrDefaultAsync(w => w.WardId == model.WardId && w.IsActive);
            if (selectedWard == null || selectedWard.CityId != selectedCity.CityId)
            {
                ModelState.AddModelError(nameof(model.WardId), "Phường/Xã không hợp lệ");
                ViewData["ReturnUrl"] = returnUrl;
                await LoadAddressSelectDataAsync("RegisterCities", "RegisterWardByCityJson");
                return View(model);
            }

            var user = new User
            {
                Email = model.UserName,
                PasswordHash = _passwordHasher.HashPassword(model.Password),
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                CityId = selectedCity.CityId,
                WardId = selectedWard.WardId,
                Role = "Customer",
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login", new { returnUrl });
        }

        // Hiển thị form đăng nhập
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Xử lý đăng nhập, tạo cookie với claims (Name, Email, Role)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.UserName);

            if (user == null || !_passwordHasher.VerifyPassword(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng");
                return View(model);
            }

            // Kiểm tra tài khoản có bị khoá không
            if (user.IsLocked)
            {
                // Nếu khoá tạm thời và đã hết hạn thì tự mở khoá
                if (user.LockedUntil.HasValue && user.LockedUntil.Value <= DateTime.Now)
                {
                    user.IsLocked = false;
                    user.LockedUntil = null;
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var msg = user.LockedUntil.HasValue
                        ? $"Tài khoản bị khoá đến {user.LockedUntil.Value:dd/MM/yyyy HH:mm}"
                        : "Tài khoản đã bị khoá. Vui lòng liên hệ quản trị viên.";
                    ModelState.AddModelError("", msg);
                    return View(model);
                }
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,               user.FullName ?? user.Email),
                new Claim(ClaimTypes.Email,              user.Email),
                new Claim(ClaimTypes.Role,               user.Role ?? "Customer"),
                new Claim(ClaimTypes.NameIdentifier,     user.UserId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe
                    ? DateTimeOffset.UtcNow.AddDays(30)
                    : DateTimeOffset.UtcNow.AddHours(2)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            TempData["SuccessMessage"] = $"Chào mừng {user.FullName}!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Admin → Dashboard, Customer → trang chủ
            if (user.Role == "Admin")
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            return RedirectToAction("Index", "Product");
        }

        // Đăng xuất người dùng
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Đã đăng xuất thành công";
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
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var user = await _context.Users
                .Include(u => u.City)
                .Include(u => u.Ward)
                .FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return NotFound();

            var vm = new ProfileViewModel
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                CityId = user.CityId,
                WardId = user.WardId,
                Email = user.Email,
                Role = user.Role,
                CreatedDate = user.CreatedDate
            };

            await LoadAddressSelectDataAsync("ProfileCities", "ProfileWardByCityJson");
            return View(vm);
        }

        // Cập nhật thông tin cá nhân
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdClaim, out var userId)) return Unauthorized();

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
            {
                model.Email = user.Email;
                model.Role = user.Role;
                model.CreatedDate = user.CreatedDate;
                await LoadAddressSelectDataAsync("ProfileCities", "ProfileWardByCityJson");
                return View(model);
            }

            var selectedCity = await _context.Cities
                .FirstOrDefaultAsync(c => c.CityId == model.CityId && c.IsActive);
            if (selectedCity == null)
            {
                ModelState.AddModelError(nameof(model.CityId), "Tỉnh/Thành phố không hợp lệ");
                model.Email = user.Email;
                model.Role = user.Role;
                model.CreatedDate = user.CreatedDate;
                await LoadAddressSelectDataAsync("ProfileCities", "ProfileWardByCityJson");
                return View(model);
            }

            var selectedWard = await _context.Wards
                .FirstOrDefaultAsync(w => w.WardId == model.WardId && w.IsActive);
            if (selectedWard == null || selectedWard.CityId != selectedCity.CityId)
            {
                ModelState.AddModelError(nameof(model.WardId), "Phường/Xã không hợp lệ");
                model.Email = user.Email;
                model.Role = user.Role;
                model.CreatedDate = user.CreatedDate;
                await LoadAddressSelectDataAsync("ProfileCities", "ProfileWardByCityJson");
                return View(model);
            }

            // Cập nhật thông tin cá nhân
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.CityId = selectedCity.CityId;
            user.WardId = selectedWard.WardId;

            // Đổi mật khẩu nếu có nhập
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
                user.PasswordHash = _passwordHasher.HashPassword(model.NewPassword);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("Profile");
        }
    }
}
