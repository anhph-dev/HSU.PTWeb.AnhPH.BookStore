using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HSU.PTWeb.AnhPH.BookStore.Data;
using HSU.PTWeb.AnhPH.BookStore.Models;

namespace HSU.PTWeb.AnhPH.BookStore.Controllers
{
    // Controller quản lý admin (quản lý sản phẩm, đơn hàng)
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private const int DefaultPageSize = 10;

        public AdminController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // Danh sách sản phẩm với phân trang và tìm kiếm
        public async Task<IActionResult> Products(string search, int? categoryId, int page = 1, int pageSize = DefaultPageSize)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // Tìm kiếm theo tên
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
                ViewData["Search"] = search;
            }

            // Lọc theo category
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
                ViewData["CategoryId"] = categoryId.Value;
            }

            // Tổng số và phân trang
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, page);
            if (page > totalPages) page = totalPages == 0 ? 1 : totalPages;

            var products = await query
                .OrderBy(p => p.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;
            ViewData["Categories"] = await _context.Categories.ToListAsync();

            return View(products);
        }

        // Hiển thị form tạo sản phẩm
        public IActionResult CreateProduct()
        {
            ViewData["Categories"] = _context.Categories.ToList();
            return View();
        }

        // Xử lý tạo sản phẩm
        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Categories"] = _context.Categories.ToList();
                return View(model);
            }

            // Upload ảnh nếu có
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "images", "products");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await imageFile.CopyToAsync(stream);
                }
                model.ImageUrl = "products/" + fileName;
            }

            model.CreatedDate = DateTime.UtcNow;
            _context.Products.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("Products");
        }

        // Hiển thị form edit sản phẩm
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewData["Categories"] = _context.Categories.ToList();
            return View(product);
        }

        // Xử lý edit sản phẩm
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Categories"] = _context.Categories.ToList();
                return View(model);
            }

            var product = await _context.Products.FindAsync(model.ProductId);
            if (product == null) return NotFound();

            product.ProductName = model.ProductName;
            product.Description = model.Description;
            product.Price = model.Price;
            product.Stock = model.Stock;
            product.CategoryId = model.CategoryId;

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "images", "products");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    await imageFile.CopyToAsync(stream);
                }
                product.ImageUrl = "products/" + fileName;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Products");
        }

        // Xóa sản phẩm
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Products");
        }

        // Danh sách đơn hàng với phân trang, tìm kiếm và lọc
        public async Task<IActionResult> Orders(string search, string status, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = DefaultPageSize)
        {
            var query = _context.Orders.Include(o => o.User).AsQueryable();

            // Tìm kiếm theo email user
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(o => o.User.Email.Contains(search) || o.User.FullName.Contains(search));
                ViewData["Search"] = search;
            }

            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.Status == status);
                ViewData["Status"] = status;
            }

            // Lọc theo khoảng ngày
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

            // Phân trang
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, page);
            if (page > totalPages) page = totalPages == 0 ? 1 : totalPages;

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewData["Page"] = page;
            ViewData["TotalPages"] = totalPages;
            ViewData["PageSize"] = pageSize;

            return View(orders);
        }

        // Báo cáo doanh thu: hiển thị tổng doanh thu ngày/tháng/năm và dữ liệu cho Chart.js
        public async Task<IActionResult> Revenue(DateTime? date)
        {
            var selected = date ?? DateTime.UtcNow.Date;

            // Doanh thu trong ngày
            var dayStart = selected.Date;
            var dayEnd = dayStart.AddDays(1);
            var totalDay = await _context.Orders
                .Where(o => o.OrderDate >= dayStart && o.OrderDate < dayEnd)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            // Doanh thu trong tháng
            var monthStart = new DateTime(selected.Year, selected.Month, 1);
            var monthEnd = monthStart.AddMonths(1);
            var totalMonth = await _context.Orders
                .Where(o => o.OrderDate >= monthStart && o.OrderDate < monthEnd)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            // Doanh thu trong năm
            var yearStart = new DateTime(selected.Year, 1, 1);
            var yearEnd = yearStart.AddYears(1);
            var totalYear = await _context.Orders
                .Where(o => o.OrderDate >= yearStart && o.OrderDate < yearEnd)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            // Dữ liệu cho biểu đồ: 7 ngày trước
            var labels = new List<string>();
            var values = new List<decimal>();
            for (int i = 6; i >= 0; i--)
            {
                var d = selected.Date.AddDays(-i);
                var s = d;
                var e = d.AddDays(1);
                var v = await _context.Orders.Where(o => o.OrderDate >= s && o.OrderDate < e).SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;
                labels.Add(d.ToString("yyyy-MM-dd"));
                values.Add(v);
            }

            ViewData["SelectedDate"] = selected.ToString("yyyy-MM-dd");
            ViewData["TotalDay"] = totalDay;
            ViewData["TotalMonth"] = totalMonth;
            ViewData["TotalYear"] = totalYear;
            ViewData["ChartLabels"] = labels;
            ViewData["ChartValues"] = values;

            return View();
        }

        // Xem chi tiết đơn hàng
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // Cập nhật trạng thái đơn hàng
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return NotFound();
            order.Status = status;
            await _context.SaveChangesAsync();
            return RedirectToAction("Orders");
        }
    }
}
