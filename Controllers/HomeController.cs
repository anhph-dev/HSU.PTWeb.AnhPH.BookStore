using System.Diagnostics;
using HSU.PTWeb.AnhPH.BookStore.Data;
using HSU.PTWeb.AnhPH.BookStore.Models;
using HSU.PTWeb.AnhPH.BookStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HSU.PTWeb.AnhPH.BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IWebHostEnvironment _env;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, IPasswordHasher passwordHasher, IWebHostEnvironment env)
        {
            _logger = logger;
            _context = context;
            _passwordHasher = passwordHasher;
            _env = env;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Product");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // DEVELOPMENT ONLY: Reset admin password and create if not exists
        public async Task<IActionResult> SetupAdmin()
        {
            if (!_env.IsDevelopment())
                return NotFound();

            var results = new List<string>();

            // Hash passwords
            var adminHash = _passwordHasher.HashPassword("Admin@123");
            var customerHash = _passwordHasher.HashPassword("Customer@123");

            results.Add($"Admin hash: {adminHash}");
            results.Add($"Customer hash: {customerHash}");

            // Upsert admin
            var admin = await _context.Users.FirstOrDefaultAsync(u => u.Email == "admin@bookstore.com");
            if (admin == null)
            {
                admin = new User
                {
                    Email = "admin@bookstore.com",
                    FullName = "Quản trị viên",
                    Role = "Admin",
                    PhoneNumber = "0901234567",
                    CreatedDate = DateTime.Now
                };
                _context.Users.Add(admin);
                results.Add("Created admin user");
            }
            else
            {
                results.Add("Updated admin user");
            }

            admin.PasswordHash = adminHash;

            // Upsert customers
            var customerEmails = new[]
            {
                ("customer1@gmail.com", "Nguyễn Văn A", "0912345678"),
                ("customer2@gmail.com", "Trần Thị B",   "0923456789"),
                ("customer3@gmail.com", "Lê Văn C",     "0934567890"),
                ("customer4@gmail.com", "Phạm Thị D",   "0945678901"),
            };

            foreach (var (email, name, phone) in customerEmails)
            {
                var customer = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (customer == null)
                {
                    customer = new User
                    {
                        Email = email,
                        FullName = name,
                        Role = "Customer",
                        PhoneNumber = phone,
                        CreatedDate = DateTime.Now.AddDays(-30)
                    };
                    _context.Users.Add(customer);
                    results.Add($"Created customer: {email}");
                }
                else
                {
                    results.Add($"Updated customer: {email}");
                }

                customer.PasswordHash = customerHash;
            }

            await _context.SaveChangesAsync();

            var html = $"""
                <html><head><title>Setup Admin</title>
                <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css">
                </head><body class="container mt-5">
                <div class="alert alert-success">
                    <h4>✅ Setup completed!</h4>
                    <ul>
                        {string.Join("", results.Select(r => $"<li>{System.Net.WebUtility.HtmlEncode(r)}</li>"))}
                    </ul>
                </div>
                <div class="card">
                    <div class="card-body">
                        <h5>Login Credentials:</h5>
                        <table class="table table-bordered">
                            <tr><th>Role</th><th>Email</th><th>Password</th></tr>
                            <tr class="table-danger"><td>Admin</td><td>admin@bookstore.com</td><td>Admin@123</td></tr>
                            <tr><td>Customer</td><td>customer1@gmail.com</td><td>Customer@123</td></tr>
                            <tr><td>Customer</td><td>customer2@gmail.com</td><td>Customer@123</td></tr>
                            <tr><td>Customer</td><td>customer3@gmail.com</td><td>Customer@123</td></tr>
                            <tr><td>Customer</td><td>customer4@gmail.com</td><td>Customer@123</td></tr>
                        </table>
                        <a href="/Account/Login" class="btn btn-primary">Go to Login →</a>
                    </div>
                </div>
                </body></html>
                """;

            return Content(html, "text/html");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
